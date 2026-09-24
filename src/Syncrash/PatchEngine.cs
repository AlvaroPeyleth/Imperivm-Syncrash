using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

internal enum PatchStatus { OriginalAdmitted, AlreadyInstalled, Applied }

internal sealed class PatchSpec
{
    internal readonly string SourceHash, ResultHash, PakHash, RecipeHash;
    internal readonly int SourceBytes, ResultBytes;

    internal PatchSpec(string sourceHash, string resultHash, string pakHash,
        string recipeHash, int sourceBytes, int resultBytes)
    {
        SourceHash = sourceHash;
        ResultHash = resultHash;
        PakHash = pakHash;
        RecipeHash = recipeHash;
        SourceBytes = sourceBytes;
        ResultBytes = resultBytes;
    }
}

internal sealed class PatchBusyException : IOException
{
    internal PatchBusyException() : base("Otra instancia de Syncrash está aplicando el parche a esta ruta. Espera a que termine.") { }
}

internal sealed class PatchOperationException : IOException
{
    internal PatchOperationException(string message, Exception cause) : base(message, cause) { }
}

internal static class PatchEngine
{
    // Production values are deliberately fixed. Synthetic tests call Check/Apply with a separate spec.
    private static readonly PatchSpec Production = new PatchSpec(
        "72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473",
        "752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc",
        "6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50",
        "9388df692e9f0f0478f8060d625e643618cfc16dd28298c1cbe879aaa6bfc583",
        4456448, 4460544);

    internal static PatchStatus CheckGame(string path)
    {
        return Check(path, Production, LoadEmbeddedRecipe(), RequireGameClosed);
    }

    internal static PatchStatus ApplyGame(string path)
    {
        return Apply(path, Production, LoadEmbeddedRecipe(), RequireGameClosed, null, null);
    }

    private static byte[] LoadEmbeddedRecipe()
    {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash.Recipe"))
        {
            if (stream == null) throw new InvalidDataException("Falta la receta interna de Syncrash.");
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }

    private static string HashFile(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var sha = SHA256.Create()) return Hex(sha.ComputeHash(stream));
    }

    internal static string HashBytes(byte[] bytes)
    {
        using (var sha = SHA256.Create()) return Hex(sha.ComputeHash(bytes));
    }

    private static string Hex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    private static byte[] HexBytes(string hex)
    {
        if (hex == null || hex.Length % 2 != 0 || !Regex.IsMatch(hex, "^[0-9a-f]*$"))
            throw new InvalidDataException("Receta de parche inválida.");
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }

    internal static Recipe ReadRecipe(byte[] raw, PatchSpec spec)
    {
        if (raw == null || HashBytes(raw) != spec.RecipeHash)
            throw new InvalidDataException("La receta interna no coincide con la versión admitida.");
        using (var memory = new MemoryStream(raw))
        {
            var serializer = new DataContractJsonSerializer(typeof(Recipe));
            Recipe recipe;
            try { recipe = serializer.ReadObject(memory) as Recipe; }
            catch (SerializationException error) { throw new InvalidDataException("Receta interna truncada o inválida.", error); }
            if (recipe == null || recipe.Schema != "syncrash-exact-delta-v1" ||
                recipe.SourceSha != spec.SourceHash || recipe.ResultSha != spec.ResultHash ||
                recipe.SourceBytes != spec.SourceBytes || recipe.ResultBytes != spec.ResultBytes ||
                recipe.Patches == null || recipe.AppendNonzero == null)
                throw new InvalidDataException("Receta interna incompatible.");
            return recipe;
        }
    }

    internal static byte[] BuildImage(byte[] source, Recipe recipe, PatchSpec spec)
    {
        if (source.Length != spec.SourceBytes || HashBytes(source) != spec.SourceHash)
            throw new InvalidDataException("El ejecutable no es el Steam original admitido.");
        if (recipe.ResultBytes < source.Length || recipe.ResultBytes != spec.ResultBytes)
            throw new InvalidDataException("Longitud de salida inválida.");
        var result = new byte[recipe.ResultBytes];
        Buffer.BlockCopy(source, 0, result, 0, source.Length);
        int lastEnd = 0;
        var all = new List<PatchEntry>();
        all.AddRange(recipe.Patches);
        all.AddRange(recipe.AppendNonzero);
        foreach (PatchEntry entry in all)
        {
            if (entry == null) throw new InvalidDataException("Entrada de receta vacía.");
            byte[] before = HexBytes(entry.Before);
            byte[] after = HexBytes(entry.After);
            if (entry.Offset < lastEnd || entry.Offset < 0 || before.Length != after.Length ||
                (long)entry.Offset + after.Length > result.Length)
                throw new InvalidDataException("Rangos de receta inválidos o solapados.");
            for (int i = 0; i < after.Length; i++)
            {
                if (result[entry.Offset + i] != before[i])
                    throw new InvalidDataException("Los bytes originales no coinciden con la receta.");
                result[entry.Offset + i] = after[i];
            }
            lastEnd = entry.Offset + after.Length;
        }
        if (HashBytes(result) != spec.ResultHash)
            throw new InvalidDataException("La imagen reconstruida no coincide con el hash verificado.");
        return result;
    }

    private static void RequireGameClosed()
    {
        Process[] processes;
        try { processes = Process.GetProcessesByName("gbr"); }
        catch (Exception error) { throw new InvalidOperationException("No se pudo comprobar si Imperivm está cerrado. No se ha aplicado el parche.", error); }
        try
        {
            if (processes.Length != 0)
                throw new InvalidOperationException("Cierra Imperivm antes de aplicar el parche.");
        }
        finally { foreach (Process process in processes) process.Dispose(); }
    }

    private static string TargetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Indica la ruta de gbr.exe.");
        string target = Path.GetFullPath(path);
        if (!string.Equals(Path.GetFileName(target), "gbr.exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(target))
            throw new FileNotFoundException("Selecciona el gbr.exe de Imperivm Steam.");
        return target;
    }

    private static PatchStatus CheckCore(string target, PatchSpec spec, byte[] rawRecipe, Action gameClosed)
    {
        string pak = Path.Combine(Path.GetDirectoryName(target), "Packs", "data.pak");
        if (!File.Exists(pak) || HashFile(pak) != spec.PakHash)
            throw new InvalidOperationException("Packs/data.pak no coincide con Steam vanilla admitido. No se ha parcheado nada.");
        gameClosed();
        string current = HashFile(target);
        if (current != spec.SourceHash && current != spec.ResultHash)
            throw new InvalidOperationException("gbr.exe no coincide con el Steam original o el resultado admitido. No se ha parcheado nada.");
        Recipe recipe = ReadRecipe(rawRecipe, spec);
        if (current == spec.SourceHash)
            BuildImage(File.ReadAllBytes(target), recipe, spec);
        return current == spec.ResultHash ? PatchStatus.AlreadyInstalled : PatchStatus.OriginalAdmitted;
    }

    internal static PatchStatus Check(string path, PatchSpec spec, byte[] rawRecipe, Action gameClosed)
    {
        return CheckCore(TargetPath(path), spec, rawRecipe, gameClosed);
    }

    private static string MutexName(string target)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(Path.GetFullPath(target).ToUpperInvariant());
        return @"Local\Syncrash-" + HashBytes(bytes);
    }

    private static string StateAfterFailure(string target, PatchSpec spec)
    {
        try
        {
            string current = HashFile(target);
            if (current == spec.SourceHash) return "El original sigue intacto.";
            if (current == spec.ResultHash) return "El resultado parece instalado, pero hubo un error: comprueba el archivo antes de jugar.";
        }
        catch (Exception) { }
        return "No se pudo confirmar el estado de gbr.exe. Compruébalo y, si es necesario, restaura los archivos con Steam.";
    }

    // Hooks are for synthetic fault/concurrency tests; shipped entry points always pass null.
    internal static PatchStatus Apply(string path, PatchSpec spec, byte[] rawRecipe,
        Action gameClosed, Action beforeReplace, Action afterReplace)
    {
        string target = TargetPath(path);
        using (var mutex = new Mutex(false, MutexName(target)))
        {
            bool acquired;
            try { acquired = mutex.WaitOne(0); }
            catch (AbandonedMutexException) { acquired = true; }
            if (!acquired) throw new PatchBusyException();
            try
            {
                PatchStatus status = CheckCore(target, spec, rawRecipe, gameClosed);
                if (status == PatchStatus.AlreadyInstalled) return status;

                Recipe recipe = ReadRecipe(rawRecipe, spec);
                byte[] source = File.ReadAllBytes(target);
                byte[] image = BuildImage(source, recipe, spec);
                string pak = Path.Combine(Path.GetDirectoryName(target), "Packs", "data.pak");
                string stage = Path.Combine(Path.GetDirectoryName(target),
                    "gbr.syncrash-stage-" + Guid.NewGuid().ToString("N") + ".tmp");
                bool stageCreated = false;
                Exception failure = null;
                try
                {
                    using (var output = new FileStream(stage, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        stageCreated = true;
                        output.Write(image, 0, image.Length);
                        output.Flush(true);
                    }
                    if (HashFile(stage) != spec.ResultHash)
                        throw new IOException("El archivo temporal no coincide con el resultado verificado.");
                    if (beforeReplace != null) beforeReplace();
                    gameClosed();
                    if (HashFile(target) != spec.SourceHash || HashFile(pak) != spec.PakHash)
                        throw new IOException("Steam cambió archivos durante el parcheo. No se ha sustituido gbr.exe.");
                    File.Replace(stage, target, null);
                    if (afterReplace != null) afterReplace();
                    if (HashFile(target) != spec.ResultHash)
                        throw new IOException("El archivo final no coincide con el resultado verificado.");
                }
                catch (Exception error) { failure = error; }
                if (stageCreated)
                {
                    try { File.Delete(stage); }
                    catch (Exception cleanup)
                    {
                        failure = failure == null ? cleanup : new AggregateException(failure, cleanup);
                    }
                }
                if (failure != null)
                    throw new PatchOperationException("No se pudo completar el parche. " +
                        StateAfterFailure(target, spec) + " Detalle: " + failure.Message, failure);
                return PatchStatus.Applied;
            }
            finally { mutex.ReleaseMutex(); }
        }
    }
}

[DataContract]
public sealed class Recipe
{
    [DataMember(Name = "schema")] public string Schema { get; set; }
    [DataMember(Name = "source_sha256")] public string SourceSha { get; set; }
    [DataMember(Name = "result_sha256")] public string ResultSha { get; set; }
    [DataMember(Name = "source_bytes")] public int SourceBytes { get; set; }
    [DataMember(Name = "result_bytes")] public int ResultBytes { get; set; }
    [DataMember(Name = "patches")] public List<PatchEntry> Patches { get; set; }
    [DataMember(Name = "append_nonzero")] public List<PatchEntry> AppendNonzero { get; set; }
}

[DataContract]
public sealed class PatchEntry
{
    [DataMember(Name = "offset")] public int Offset { get; set; }
    [DataMember(Name = "before")] public string Before { get; set; }
    [DataMember(Name = "after")] public string After { get; set; }
}
