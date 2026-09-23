using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

internal static class Syncrash
{
    private const string OriginalHash = "72b09d1abd4f311efe4213a9a1110185519bde4db4ee57769d346b475c748473";
    private const string PatchedHash = "752c95a475e62b0d61d88ec9fb7fabc07758cb217ab152d78651385a5de3d2cc";
    private const string VanillaDataHash = "6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50";
    private const string RecipeHash = "9388df692e9f0f0478f8060d625e643618cfc16dd28298c1cbe879aaa6bfc583";
    private const string ResourceName = "Syncrash.Recipe";

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--test")
        {
            try { Console.WriteLine(PatchGame(args[1])); return 0; }
            catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
        }
        if (args.Length == 1 && args[0] == "--detect-only")
        {
            foreach (string path in FindSteamGames()) Console.WriteLine(path);
            return 0;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new SyncrashWindow());
        return 0;
    }

    private static string HashFile(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }

    private static string HashBytes(byte[] bytes)
    {
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
    }

    private static byte[] HexBytes(string hex)
    {
        if (hex == null || hex.Length % 2 != 0 || !Regex.IsMatch(hex, "^[0-9a-f]*$"))
            throw new InvalidDataException("Receta de parche inválida.");
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }

    private static Recipe ReadRecipe()
    {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
        {
            if (stream == null) throw new InvalidDataException("Falta la receta interna de Syncrash.");
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                byte[] raw = memory.ToArray();
                if (HashBytes(raw) != RecipeHash) throw new InvalidDataException("La receta interna no coincide con Syncrash v1.");
                memory.Position = 0;
                var serializer = new DataContractJsonSerializer(typeof(Recipe));
                var recipe = serializer.ReadObject(memory) as Recipe;
                if (recipe == null || recipe.Schema != "syncrash-exact-delta-v1" ||
                    recipe.SourceSha != OriginalHash || recipe.ResultSha != PatchedHash ||
                    recipe.SourceBytes != 4456448 || recipe.ResultBytes != 4460544 ||
                    recipe.Patches == null || recipe.AppendNonzero == null)
                    throw new InvalidDataException("Receta interna incompatible.");
                return recipe;
            }
        }
    }

    private static byte[] BuildImage(byte[] source, Recipe recipe)
    {
        if (source.Length != recipe.SourceBytes || HashBytes(source) != OriginalHash)
            throw new InvalidDataException("El ejecutable no es el Steam original admitido.");
        var result = new byte[recipe.ResultBytes];
        Buffer.BlockCopy(source, 0, result, 0, source.Length);
        int lastEnd = 0;
        var all = new List<PatchEntry>();
        all.AddRange(recipe.Patches);
        all.AddRange(recipe.AppendNonzero);
        foreach (PatchEntry entry in all)
        {
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
        if (HashBytes(result) != PatchedHash)
            throw new InvalidDataException("La V2 reconstruida no coincide con el hash verificado.");
        return result;
    }

    private static void RequireGameClosed()
    {
        if (Process.GetProcessesByName("gbr").Length != 0)
            throw new InvalidOperationException("Cierra Imperivm antes de aplicar el parche.");
    }

    internal static string PatchGame(string exePath)
    {
        string target = Path.GetFullPath(exePath);
        if (!string.Equals(Path.GetFileName(target), "gbr.exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(target))
            throw new FileNotFoundException("Selecciona el gbr.exe de Imperivm Steam.");
        string gameRoot = Path.GetDirectoryName(target);
        string pak = Path.Combine(gameRoot, "Packs", "data.pak");
        if (!File.Exists(pak) || HashFile(pak) != VanillaDataHash)
            throw new InvalidOperationException("Packs/data.pak no coincide con Steam vanilla admitido. No se ha parcheado nada.");
        RequireGameClosed();
        string currentHash = HashFile(target);
        if (currentHash != OriginalHash && currentHash != PatchedHash)
            throw new InvalidOperationException("gbr.exe no coincide con el Steam original admitido. No se ha parcheado nada.");

        Recipe recipe = ReadRecipe();
        byte[] source = File.ReadAllBytes(target);
        if (HashBytes(source) != currentHash)
            throw new IOException("El ejecutable cambió durante la comprobación. Inténtalo de nuevo con Steam cerrado.");
        // Reapply the verified image even when this exact version is installed.
        byte[] image = currentHash == OriginalHash ? BuildImage(source, recipe) : source;
        if (HashBytes(image) != PatchedHash) throw new InvalidDataException("Resultado del parche no válido.");
        string stage = Path.Combine(gameRoot, "gbr.syncrash-stage-" + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllBytes(stage, image);
            if (HashFile(stage) != PatchedHash) throw new IOException("El archivo temporal no coincide con V2.");
            RequireGameClosed();
            if (HashFile(target) != currentHash || HashFile(pak) != VanillaDataHash)
                throw new IOException("Steam cambió archivos durante el parcheo. No se ha sustituido gbr.exe.");
            File.Replace(stage, target, null); // Same-directory replacement; no backup is created.
            if (HashFile(target) != PatchedHash)
                throw new IOException("El archivo final no coincide con V2. Recupera el original desde Steam.");
            return "Syncrash aplicado. Ya puedes abrir Imperivm desde Steam.";
        }
        finally
        {
            if (File.Exists(stage)) File.Delete(stage);
        }
    }

    private static string VdfValue(string text, string key)
    {
        Match found = Regex.Match(text, "\"" + Regex.Escape(key) + "\"\\s*\"([^\"]+)\"", RegexOptions.IgnoreCase);
        return found.Success ? found.Groups[1].Value.Replace("\\\\", "\\") : null;
    }

    private static void AddLibrary(HashSet<string> libraries, string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        try
        {
            string full = Path.GetFullPath(path.Replace('/', '\\'));
            if (Directory.Exists(Path.Combine(full, "steamapps"))) libraries.Add(full);
        }
        catch (ArgumentException) { }
        catch (NotSupportedException) { }
    }

    internal static List<string> FindSteamGames()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key != null)
                {
                    AddLibrary(roots, key.GetValue("SteamPath") as string);
                    AddLibrary(roots, key.GetValue("InstallPath") as string);
                }
            }
        }
        catch (System.Security.SecurityException) { }
        AddLibrary(roots, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"));
        AddLibrary(roots, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam"));
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            try
            {
                if (drive.DriveType == DriveType.Fixed && drive.IsReady)
                {
                    AddLibrary(roots, Path.Combine(drive.RootDirectory.FullName, "SteamLibrary"));
                    AddLibrary(roots, Path.Combine(drive.RootDirectory.FullName, "Steam"));
                }
            }
            catch (IOException) { }
        }
        // Steam keeps additional library roots in libraryfolders.vdf.
        foreach (string root in new List<string>(roots))
        {
            string vdf = Path.Combine(root, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) continue;
            try
            {
                string text = File.ReadAllText(vdf);
                foreach (Match match in Regex.Matches(text, "\"path\"\\s*\"([^\"]+)\"", RegexOptions.IgnoreCase))
                    AddLibrary(roots, match.Groups[1].Value.Replace("\\\\", "\\"));
            }
            catch (IOException) { }
        }
        var games = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string root in roots)
        {
            string installDir = "Imperivm - Great Battles of Rome HD";
            string manifest = Path.Combine(root, "steamapps", "appmanifest_752580.acf");
            if (File.Exists(manifest))
            {
                try
                {
                    string value = VdfValue(File.ReadAllText(manifest), "installdir");
                    if (!string.IsNullOrEmpty(value) && value != "." && value != ".." &&
                        value.IndexOfAny(new[] { '\\', '/', ':' }) < 0) installDir = value;
                }
                catch (IOException) { }
            }
            string game = Path.Combine(root, "steamapps", "common", installDir, "gbr.exe");
            if (File.Exists(game)) games.Add(Path.GetFullPath(game));
        }
        return new List<string>(games);
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
