using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

[DataContract]
internal sealed class VoiceFile
{
    [DataMember(Name = "destination")] public string Destination { get; set; }
    [DataMember(Name = "source")] public string Source { get; set; }
    [DataMember(Name = "size")] public int Size { get; set; }
    [DataMember(Name = "sha256")] public string Hash { get; set; }
    internal byte[] Data;
}

[DataContract]
internal sealed class VoiceLanguage
{
    [DataMember(Name = "language")] public string Language { get; set; }
    [DataMember(Name = "pak_sha256")] public string PakHash { get; set; }
    [DataMember(Name = "files")] public List<VoiceFile> Files { get; set; }
}

[DataContract]
internal sealed class VoiceCatalog
{
    [DataMember(Name = "format")] public int Format { get; set; }
    [DataMember(Name = "data_pak_sha256")] public string DataHash { get; set; }
    [DataMember(Name = "languages")] public List<VoiceLanguage> Languages { get; set; }
}

[DataContract]
internal sealed class VoiceRecord
{
    [DataMember(Name = "owner")] public string Owner { get; set; }
    [DataMember(Name = "status")] public string Status { get; set; }
    [DataMember(Name = "languages")] public List<string> Languages { get; set; }
    [DataMember(Name = "target_language")] public string Target { get; set; }
    // Only used to validate and migrate the earlier private prototype.
    [DataMember(Name = "language", EmitDefaultValue = false)] public string LegacyLanguage { get; set; }
    [DataMember(Name = "files", EmitDefaultValue = false)] public List<VoiceFile> LegacyFiles { get; set; }
}

internal static class VoiceCompatibility
{
    internal const string StateDirectory = ".syncrash-voices";
    internal const string Marker = StateDirectory + "/manifest.json";
    private const string Owner = "syncrash-voices-v2";
    private const string CatalogHash = "d5d347d4d7184fce57b002c7ea696dcac42d6deaed4a3510e111bcfd7b3e96d2";

    private static T ReadJson<T>(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
    }

    private static byte[] Json<T>(T value)
    {
        using (var stream = new MemoryStream())
        {
            new DataContractJsonSerializer(typeof(T)).WriteObject(stream, value);
            return stream.ToArray();
        }
    }

    internal static VoiceCatalog Catalog()
    {
        using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash.Voices"))
        using (var memory = new MemoryStream())
        {
            if (resource == null) throw new InvalidDataException("Faltan las correspondencias de voces.");
            resource.CopyTo(memory);
            byte[] bytes = memory.ToArray();
            if (PatchEngine.HashBytes(bytes) != CatalogHash) throw new InvalidDataException("El mapa de voces no coincide con el revisado.");
            return ReadJson<VoiceCatalog>(bytes);
        }
    }

    internal static VoiceLanguage Language(VoiceCatalog catalog, string name)
    {
        foreach (VoiceLanguage language in catalog.Languages)
            if (language.Language == name) return language;
        throw new InvalidDataException("Idioma de voces no admitido. Selecciona español, italiano o inglés en el juego.");
    }

    // Check every ancestor, including the installation directory and dangling links.
    internal static string SafePath(string directory, string relative)
    {
        if (string.IsNullOrEmpty(relative) || relative.Contains("\\")) throw new InvalidDataException("Ruta de voces inválida.");
        foreach (string part in relative.Split('/'))
            if (part.Length == 0 || part == "." || part == ".." || part.EndsWith(".") || part.EndsWith(" ") || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new InvalidDataException("Ruta de voces inválida.");
        string full = Path.GetFullPath(Path.Combine(directory, relative.Replace('/', Path.DirectorySeparatorChar)));
        string current = Path.GetPathRoot(full);
        foreach (string part in full.Substring(current.Length).Split(Path.DirectorySeparatorChar))
        {
            current = Path.Combine(current, part);
            try
            {
                if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("No se modifican voces a través de enlaces o junctions: " + current);
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
        }
        return full;
    }

    private static bool Exists(string path)
    {
        if (Directory.Exists(path)) throw new IOException("Hay una carpeta donde se esperaba un archivo: " + path);
        return File.Exists(path);
    }

    internal static string SelectedLanguage(string directory)
    {
        string selected = null, section = "";
        foreach (string raw in File.ReadAllLines(SafePath(directory, "Settings.ini")))
        {
            string line = raw.Trim();
            if (line.StartsWith(";") || line.StartsWith("#")) continue;
            if (line.StartsWith("[") && line.EndsWith("]")) { section = line.Substring(1, line.Length - 2).Trim(); continue; }
            int equals = line.IndexOf('=');
            if (section.Equals("Language", StringComparison.OrdinalIgnoreCase) && equals >= 0 &&
                line.Substring(0, equals).Trim().Equals("Default", StringComparison.OrdinalIgnoreCase))
            {
                if (selected != null) throw new InvalidDataException("Settings.ini contiene varios idiomas predeterminados.");
                selected = line.Substring(equals + 1).Trim().ToLowerInvariant();
            }
        }
        if (string.IsNullOrEmpty(selected)) throw new InvalidDataException("No se encuentra el idioma en Settings.ini. Configúralo en el juego antes de reparar las voces.");
        return selected;
    }

    // FileShare.Read keeps the inspected PAK immutable while extracting all payloads.
    internal static void LoadAudio(string directory, VoiceLanguage language, string dataHash)
    {
        if (PatchEngine.HashFile(SafePath(directory, "Packs/data.pak")) != dataHash)
            throw new InvalidDataException("data.pak no coincide con la versión de voces admitida.");
        using (var stream = new FileStream(SafePath(directory, "local/" + language.Language + ".pak"), FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var reader = new BinaryReader(stream, Encoding.GetEncoding(28591)))
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
                if (BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant() != language.PakHash)
                    throw new InvalidDataException("El PAK de " + language.Language + " está modificado o no es compatible. No se han cambiado las voces.");
            stream.Position = 0;
            byte[] header = reader.ReadBytes(40);
            if (header.Length != 40 || Encoding.ASCII.GetString(header, 0, 17) != "HMMSYS PackFile\n\x1a")
                throw new InvalidDataException("Cabecera PAK inválida.");
            uint count = BitConverter.ToUInt32(header, 32);
            if (count == 0 || count > 100000) throw new InvalidDataException("Índice PAK inválido.");
            var index = new Dictionary<string, long[]>(StringComparer.OrdinalIgnoreCase);
            string previous = "";
            for (int i = 0; i < count; i++)
            {
                int length = reader.ReadByte(), prefix = reader.ReadByte();
                if (prefix > length || prefix > previous.Length) throw new InvalidDataException("Nombre PAK inválido.");
                byte[] tail = reader.ReadBytes(length - prefix);
                if (tail.Length != length - prefix) throw new EndOfStreamException();
                string name = previous.Substring(0, prefix) + Encoding.GetEncoding(28591).GetString(tail);
                long offset = reader.ReadUInt32(), size = reader.ReadUInt32();
                if (offset + size > stream.Length || index.ContainsKey(name)) throw new InvalidDataException("Recurso PAK inválido.");
                index.Add(name, new[] { offset, size });
                previous = name;
            }
            foreach (long[] entry in index.Values)
                if (entry[0] < stream.Position) throw new InvalidDataException("El contenido invade el índice PAK.");
            foreach (VoiceFile file in language.Files)
            {
                long[] entry;
                if (!index.TryGetValue(file.Source, out entry) || entry[1] != file.Size || file.Size <= 0 || file.Size > 16 * 1024 * 1024)
                    throw new InvalidDataException("No coincide el recurso de voz: " + file.Source);
                stream.Position = entry[0];
                file.Data = reader.ReadBytes(file.Size);
                if (file.Data.Length != file.Size || PatchEngine.HashBytes(file.Data) != file.Hash)
                    throw new InvalidDataException("La grabación no coincide con la correspondencia revisada.");
            }
        }
    }

    private static VoiceRecord ReadRecord(string directory, VoiceCatalog catalog)
    {
        string path = SafePath(directory, Marker);
        if (!Exists(path)) return null;
        if (new FileInfo(path).Length > 512 * 1024) throw new InvalidDataException("Registro de voces demasiado grande.");
        VoiceRecord record = ReadJson<VoiceRecord>(File.ReadAllBytes(path));
        if (record == null) throw new InvalidDataException("Registro de voces vacío.");
        if (record.Owner == "syncrash-voice-trial-v1")
        {
            VoiceLanguage legacy = Language(catalog, record.LegacyLanguage);
            if (record.LegacyLanguage == "english" || record.LegacyFiles == null || record.LegacyFiles.Count != legacy.Files.Count)
                throw new InvalidDataException("Ensayo anterior no reconocido.");
            var expected = Files(catalog, new[] { record.LegacyLanguage });
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VoiceFile file in record.LegacyFiles)
            {
                List<VoiceFile> candidates;
                if (file == null || file.Destination == null || !seen.Add(file.Destination) || !expected.TryGetValue(file.Destination, out candidates) ||
                    file.Hash != candidates[0].Hash || file.Source != candidates[0].Source || file.Size != candidates[0].Size)
                    throw new InvalidDataException("El ensayo anterior no coincide con las voces revisadas.");
            }
            return new VoiceRecord { Owner = record.Owner, Status = "installed", Languages = new List<string> { legacy.Language }, Target = legacy.Language };
        }
        if (record.Owner != Owner || (record.Status != "pending" && record.Status != "installed") ||
            record.Languages == null || record.Languages.Count == 0 || record.Languages.Count > 3 || record.Target == null)
            throw new InvalidDataException("Registro de voces desconocido. Se han conservado los archivos.");
        var languages = new HashSet<string>();
        foreach (string name in record.Languages)
        {
            Language(catalog, name);
            if (!languages.Add(name)) throw new InvalidDataException("Idiomas duplicados en el registro.");
        }
        if ((record.Target != "" && !languages.Contains(record.Target)) ||
            (record.Status == "installed" && (languages.Count != 1 || record.Target == "")))
            throw new InvalidDataException("Estado de voces contradictorio.");
        return record;
    }

    private static Dictionary<string, List<VoiceFile>> Files(VoiceCatalog catalog, IEnumerable<string> languages)
    {
        var result = new Dictionary<string, List<VoiceFile>>(StringComparer.OrdinalIgnoreCase);
        foreach (string name in languages)
            foreach (VoiceFile file in Language(catalog, name).Files)
            {
                List<VoiceFile> list;
                if (!result.TryGetValue(file.Destination, out list)) result.Add(file.Destination, list = new List<VoiceFile>());
                list.Add(file);
            }
        return result;
    }

    private static bool Matches(string path, List<VoiceFile> files)
    {
        string hash = PatchEngine.HashFile(path);
        foreach (VoiceFile file in files) if (hash == file.Hash) return true;
        return false;
    }

    // Validate the ENTIRE union before writing, deleting, or invoking the base patch.
    private static bool Inspect(string directory, VoiceCatalog catalog, VoiceRecord record, VoiceLanguage target)
    {
        var owned = Files(catalog, record == null ? new string[0] : record.Languages.ToArray());
        foreach (var item in owned)
        {
            string path = SafePath(directory, item.Key);
            if (Exists(path) && !Matches(path, item.Value))
                throw new IOException("Se ha modificado una voz registrada. Se conserva: " + item.Key);
        }
        bool complete = record != null && record.Status == "installed" && target != null && record.Target == target.Language;
        if (target != null)
            foreach (VoiceFile file in target.Files)
            {
                string path = SafePath(directory, file.Destination);
                if (!Exists(path)) { complete = false; continue; }
                if (!owned.ContainsKey(file.Destination))
                    throw new IOException("Ya existe una voz ajena a Syncrash. Se conserva: " + file.Destination);
                if (PatchEngine.HashFile(path) != file.Hash) complete = false;
            }
        return complete;
    }

    private static void Publish(string directory, string relative, byte[] data, bool replace)
    {
        string target = SafePath(directory, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(target));
        string stage = SafePath(directory, StateDirectory + "/stage-" + Guid.NewGuid().ToString("N") + ".tmp");
        bool created = false;
        try
        {
            using (var output = new FileStream(stage, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                output.Write(data, 0, data.Length);
                output.Flush(true);
            }
            if (PatchEngine.HashFile(stage) != PatchEngine.HashBytes(data)) throw new IOException("Fallo al preparar el archivo de voces.");
            SafePath(directory, relative);
            if (replace) File.Replace(stage, target, null);
            else File.Move(stage, target); // Refuse an unowned file appearing after inspection.
        }
        finally { if (created && File.Exists(stage)) File.Delete(stage); }
    }

    // Hooks/catalog injection exist for isolated tests. Production uses fixed embedded metadata.
    internal static bool Configure(string directory, VoiceCatalog catalog, VoiceLanguage target,
        Action checkClosed, Action applyBase, Action<int> afterWrite)
    {
        checkClosed();
        string folder = SafePath(directory, StateDirectory);
        if (target == null && !Directory.Exists(folder)) { applyBase(); return false; }
        Directory.CreateDirectory(folder);
        using (var gate = new FileStream(SafePath(directory, StateDirectory + "/lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            try { gate.Lock(0, 1); }
            catch (IOException) { throw new PatchBusyException(); }
            try
            {
                checkClosed();
                VoiceRecord record = ReadRecord(directory, catalog);
                bool complete = Inspect(directory, catalog, record, target);
                if (target != null)
                    foreach (VoiceFile file in target.Files)
                        if (file.Data == null || PatchEngine.HashBytes(file.Data) != file.Hash)
                            throw new InvalidDataException("Contenido de voz incompleto o alterado.");
                applyBase();
                if (complete && record.Owner == Owner) return false;
                if (record == null && target == null) return false;
                checkClosed();
                Inspect(directory, catalog, record, target);
                var languages = record == null ? new List<string>() : new List<string>(record.Languages);
                if (target != null && !languages.Contains(target.Language)) languages.Add(target.Language);
                var pending = new VoiceRecord { Owner = Owner, Status = "pending", Languages = languages, Target = target == null ? "" : target.Language };
                Publish(directory, Marker, Json(pending), record != null);
                try
                {
                    int step = 0;
                    if (afterWrite != null) afterWrite(step++);
                    var wanted = new Dictionary<string, VoiceFile>(StringComparer.OrdinalIgnoreCase);
                    if (target != null) foreach (VoiceFile file in target.Files) wanted.Add(file.Destination, file);
                    foreach (var item in Files(catalog, languages))
                    {
                        checkClosed();
                        string path = SafePath(directory, item.Key);
                        VoiceFile desired;
                        wanted.TryGetValue(item.Key, out desired);
                        if (Exists(path))
                        {
                            if (!Matches(path, item.Value)) throw new IOException("Una voz cambió durante la operación: " + item.Key);
                            if (desired != null && PatchEngine.HashFile(path) == desired.Hash) continue;
                            File.Delete(path);
                        }
                        if (desired != null) Publish(directory, item.Key, desired.Data, false);
                        if (afterWrite != null) afterWrite(step++);
                    }
                    checkClosed();
                    if (target == null) File.Delete(SafePath(directory, Marker));
                    else
                    {
                        // No success marker until every desired file has the correct bytes.
                        foreach (VoiceFile file in target.Files)
                            if (PatchEngine.HashFile(SafePath(directory, file.Destination)) != file.Hash) throw new IOException("Verificación final de voces fallida.");
                        Publish(directory, Marker, Json(new VoiceRecord { Owner = Owner, Status = "installed", Languages = new List<string> { target.Language }, Target = target.Language }), true);
                    }
                    return true;
                }
                catch (Exception error)
                {
                    throw new IOException("La operación de voces quedó incompleta. Mantén el juego cerrado y vuelve a aplicar; para retirarlas, desmarca Reparar voces de unidades y aplica. El registro permite recuperar la operación. " + error.Message, error);
                }
            }
            finally { gate.Unlock(0, 1); }
        }
    }

    private static string GameDirectory(string path)
    {
        string full = Path.GetFullPath(path);
        if (!Path.GetFileName(full).Equals("gbr.exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(full))
            throw new FileNotFoundException("Selecciona el gbr.exe de Imperivm.");
        return Path.GetDirectoryName(full);
    }

    internal static string ConfigureGame(string path, bool enabled, Action applyBase, out bool changed)
    {
        string directory = GameDirectory(path);
        PatchEngine.RequireGameClosed();
        VoiceCatalog catalog = Catalog();
        VoiceLanguage target = enabled ? Language(catalog, SelectedLanguage(directory)) : null;
        if (target != null) LoadAudio(directory, target, catalog.DataHash);
        string selected = target == null ? "" : target.Language;
        Action validate = delegate {
            PatchEngine.RequireGameClosed();
            if (target != null && SelectedLanguage(directory) != selected)
                throw new IOException("El idioma ha cambiado durante la operación. Vuelve a aplicar.");
        };
        changed = Configure(directory, catalog, target, validate, applyBase, null);
        return enabled ? "Voces reparadas: " + DisplayLanguage(selected) + ". Si cambias de idioma, vuelve a aplicar Syncrash antes de jugar." :
            "Reparación de voces desactivada; no quedan voces registradas instaladas.";
    }

    private static string DisplayLanguage(string value)
    {
        return value == "spanish" ? "español" : value == "italian" ? "italiano" : "inglés";
    }

    internal static string CheckGame(string path, out bool complete)
    {
        string directory = GameDirectory(path);
        VoiceCatalog catalog = Catalog();
        VoiceLanguage target = Language(catalog, SelectedLanguage(directory));
        LoadAudio(directory, target, catalog.DataHash);
        VoiceRecord record = ReadRecord(directory, catalog);
        complete = Inspect(directory, catalog, record, target);
        return complete ? "Voces instaladas y verificadas: " + DisplayLanguage(target.Language) + "." :
            "Voces pendientes de aplicar o actualizar para " + DisplayLanguage(target.Language) + ". No se ha modificado ningún archivo.";
    }
}
