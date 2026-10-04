using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

internal sealed class ScreenFile
{
    internal readonly string Name, Hash;
    internal byte[] Data;
    internal ScreenFile(string name, string hash) { Name = name; Hash = hash; }
}

internal static class ScreenCompatibility
{
    internal const string MarkerName = "syncrash-screen-installation.txt";
    private const string SmoothProfileHash = "17992547c419d5a92f6aa445eb06cdc0be5bbbdbf439132fc93c9859744bc92a";

    private static ScreenFile[] ProductionFiles(bool smooth = false)
    {
        // Install the loading proxy last so the game loads only after its dependencies are in place.
        var files = new[] {
            new ScreenFile("Syncrash-screen-LICENSE.txt", "3972dc9744f6499f0f9b2dbf76696f2ae7ad8af9b23dde66d6af86c9dfb36986"),
            new ScreenFile("dxwnd.dxw", "94bfff55becb095ed0730e2e65e5598cecdb02c0704a2c7165dcbd458eebb8b5"),
            new ScreenFile("dxwnd.dll", "829edcdfbc72e2c9b06d76f1ca64eed0279f0d01ab6bb54bde80cc53d66f7e89"),
            new ScreenFile("winmm.dll", "34a17195617d5a3a5471807053b6d566a591278dfc8c531cc89d84ce077cbcd6")
        };
        if (smooth) files[1] = new ScreenFile(files[1].Name, SmoothProfileHash);
        return files;
    }

    private static ScreenFile[] InstalledFiles(string directory)
    {
        string marker = Path.Combine(directory, MarkerName);
        if (!Exists(marker)) return ProductionFiles();
        var legacy = ProductionFiles();
        legacy[1] = new ScreenFile(legacy[1].Name, "d22373941271d906e49a5d66dde020be2e3bdc49cc3020ceea954420d838592a");
        legacy[2] = new ScreenFile(legacy[2].Name, "d46109c8cfa9111e33cb4136f605407beef4bbd809047b3a06792ce0e8adabc3");
        string hash = PatchEngine.HashFile(marker);
        foreach (ScreenFile[] files in new[] { ProductionFiles(), ProductionFiles(true), legacy })
            if (PatchEngine.HashBytes(Marker(files)) == hash) return files;
        throw new IOException("El registro de pantalla no corresponde a una versión admitida. Usa su aplicador original para retirarla.");
    }

    private static string GameDirectory(string path)
    {
        string target = Path.GetFullPath(path);
        if (!string.Equals(Path.GetFileName(target), "gbr.exe", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Selecciona el gbr.exe del juego.");
        return Path.GetDirectoryName(target);
    }

    internal static byte[] Marker(ScreenFile[] files)
    {
        var text = new StringBuilder("Syncrash-screen-v1\n");
        foreach (ScreenFile file in files) text.Append(file.Name).Append(' ').Append(file.Hash).Append('\n');
        return new UTF8Encoding(false).GetBytes(text.ToString());
    }

    private static bool Exists(string path)
    {
        if (Directory.Exists(path)) throw new IOException("Hay una carpeta donde se esperaba un archivo: " + Path.GetFileName(path));
        if (!File.Exists(path)) return false;
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new IOException("No se modifican enlaces de archivos: " + Path.GetFileName(path));
        return true;
    }

    // Missing registered files may come from an interrupted install or removal.
    // Preserve changed or unregistered files, even when their names match.
    internal static bool Inspect(string directory, ScreenFile[] files)
    {
        string marker = Path.Combine(directory, MarkerName);
        bool owned = Exists(marker);
        if (owned && PatchEngine.HashFile(marker) != PatchEngine.HashBytes(Marker(files)))
                throw new IOException("Hay otra configuración de pantalla instalada. Desmarca pantalla y aplica para retirarla; después marca y aplica las nuevas opciones. Los archivos se han conservado.");
        bool complete = owned;
        foreach (ScreenFile file in files)
        {
            string path = Path.Combine(directory, file.Name);
            if (!Exists(path)) { complete = false; continue; }
            if (!owned)
                throw new IOException("Ya existe " + file.Name + " sin un registro de Syncrash. No se ha sustituido. Retira primero el otro mod o ensayo de pantalla.");
            if (PatchEngine.HashFile(path) != file.Hash)
                throw new IOException("Ha cambiado " + file.Name + ". No se ha sustituido ni eliminado. Conserva el archivo y revisa la instalación.");
        }
        return complete;
    }

    private static Mutex Lock(string directory)
    {
        return new Mutex(false, @"Local\Syncrash-Screen-" +
            PatchEngine.HashBytes(Encoding.UTF8.GetBytes(Path.GetFullPath(directory).ToUpperInvariant())));
    }

    private static void Acquire(Mutex mutex)
    {
        bool acquired;
        try { acquired = mutex.WaitOne(0); }
        catch (AbandonedMutexException) { acquired = true; }
        if (!acquired) throw new PatchBusyException();
    }

    private static void Publish(string path, byte[] data)
    {
        string stage = path + ".syncrash-" + Guid.NewGuid().ToString("N") + ".tmp";
        bool created = false;
        try
        {
            using (var output = new FileStream(stage, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                output.Write(data, 0, data.Length);
                output.Flush(true);
            }
            if (PatchEngine.HashFile(stage) != PatchEngine.HashBytes(data))
                throw new IOException("No coincide el archivo temporal de pantalla.");
            File.Move(stage, path); // Never replace an existing file, including a racing writer.
        }
        finally { if (created && File.Exists(stage)) File.Delete(stage); }
    }

    internal static bool HasEmbeddedScreen()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var names = new HashSet<string>(assembly.GetManifestResourceNames());
        foreach (ScreenFile file in ProductionFiles())
            if (!names.Contains("Syncrash.Screen." + file.Name)) return false;
        return names.Contains("Syncrash.Screen.SmoothProfile") && names.Contains("Syncrash.ScreenSources") && names.Contains("Syncrash.ScreenSourcesHash");
    }

    private static byte[] ReadEmbedded(string name)
    {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
        {
            if (stream == null)
                throw new IOException("Esta compilación no incluye pantalla adaptable. Usa el EXE completo de Syncrash para añadirla.");
            using (var output = new MemoryStream()) { stream.CopyTo(output); return output.ToArray(); }
        }
    }

    internal static void ExportScreenSources(string destination)
    {
        byte[] data = ReadEmbedded("Syncrash.ScreenSources");
        string expected = Encoding.ASCII.GetString(ReadEmbedded("Syncrash.ScreenSourcesHash")).Trim();
        if (PatchEngine.HashBytes(data) != expected) throw new IOException("No coincide el archivo interno de licencias y fuentes.");
        if (File.Exists(destination) || Directory.Exists(destination))
            throw new IOException("El destino ya existe. Elige otro nombre para conservarlo.");
        Publish(Path.GetFullPath(destination), data);
    }

    private static void LoadBundle(ScreenFile[] files)
    {
        if (!HasEmbeddedScreen())
            throw new IOException("Esta compilación no incluye pantalla adaptable. Usa el EXE completo de Syncrash para añadirla.");
        foreach (ScreenFile file in files)
        {
            file.Data = ReadEmbedded(file.Name == "dxwnd.dxw" && file.Hash == SmoothProfileHash ?
                "Syncrash.Screen.SmoothProfile" : "Syncrash.Screen." + file.Name);
            if (PatchEngine.HashBytes(file.Data) != file.Hash)
                throw new IOException("El componente interno de pantalla " + file.Name + " no coincide con el admitido.");
        }
    }

    internal static string ApplyGame(string path)
    {
        bool changed;
        return ApplyGame(path, out changed);
    }

    internal static string ApplyGame(string path, out bool changed)
    {
        return ApplyGame(path, false, out changed);
    }

    internal static string ApplyGame(string path, bool smooth, out bool changed)
    {
        PatchEngine.CheckGame(path);
        ScreenFile[] files = ProductionFiles(smooth);
        LoadBundle(files); // Validate the complete bundle before patching the executable.
        changed = Install(GameDirectory(path), files, () => PatchEngine.ApplyGame(path),
            () => PatchEngine.CheckGame(path), null);
        return changed ? "Syncrash y pantalla adaptable instalados" + (smooth ? " con suavizado" : "") + ". Abre Imperivm."
            : "Syncrash y pantalla adaptable ya están instalados. No se han modificado archivos.";
    }

    internal static string CheckGame(string path, out bool complete)
    {
        PatchStatus status = PatchEngine.CheckGame(path);
        string directory = GameDirectory(path);
        ScreenFile[] files = InstalledFiles(directory);
        bool screen = Inspect(directory, files);
        complete = status == PatchStatus.AlreadyInstalled && screen;
        string game = status == PatchStatus.AlreadyInstalled ? "Parche LAA instalado." :
            status == PatchStatus.UpgradeAdmitted ? "Puedes actualizar esta versión anterior de Syncrash." : "La instalación original de Steam es compatible.";
        return game + (screen ? " Pantalla adaptable instalada" + (files[1].Hash == SmoothProfileHash ? " con suavizado." : ".") : " Pantalla adaptable opcional no instalada o incompleta.") +
            " No se ha modificado ningún archivo.";
    }

    internal static string RemoveGame(string path)
    {
        PatchEngine.CheckGame(path);
        string directory = GameDirectory(path);
        Remove(directory, InstalledFiles(directory), () => PatchEngine.CheckGame(path), null);
        return "Archivos de pantalla retirados. Se conserva el parche de memoria y cierres; el juego recupera su modo de pantalla original.";
    }

    internal static string ConfigureGame(string path, bool enabled, out bool changed)
    {
        if (enabled) return ApplyGame(path, true, out changed);
        PatchEngine.CheckGame(path);
        string directory = GameDirectory(path);
        changed = Remove(directory, InstalledFiles(directory), () => PatchEngine.CheckGame(path), null,
            () => PatchEngine.ApplyGame(path));
        return "Ampliación de memoria y protección de cierres instaladas. Pantalla adaptable desactivada.";
    }

    // Tests pass fixture files; production uses the pinned hashes.
    internal static bool Install(string directory, ScreenFile[] files, Func<PatchStatus> patch,
        Action gameClosed, Action<int> beforePublish)
    {
        using (Mutex mutex = Lock(directory))
        {
            Acquire(mutex);
            try
            {
                gameClosed();
                bool complete = Inspect(directory, files);
                foreach (ScreenFile file in files)
                    if (file.Data == null || PatchEngine.HashBytes(file.Data) != file.Hash)
                        throw new IOException("Componente de pantalla incompleto o alterado: " + file.Name);
                PatchStatus patched = patch();
                if (complete) return patched != PatchStatus.AlreadyInstalled;
                try
                {
                    gameClosed();
                    string marker = Path.Combine(directory, MarkerName);
                    if (!Exists(marker)) Publish(marker, Marker(files));
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (beforePublish != null) beforePublish(i);
                        gameClosed();
                        Inspect(directory, files);
                        string destination = Path.Combine(directory, files[i].Name);
                        if (!Exists(destination)) Publish(destination, files[i].Data);
                    }
                    if (!Inspect(directory, files)) throw new IOException("Faltan archivos tras instalar la pantalla.");
                    return true;
                }
                catch (Exception error)
                {
                    throw new IOException("El parche del ejecutable quedó instalado, pero la pantalla no se completó. " +
                        "Vuelve a aplicar o desmarca pantalla y aplica para retirar los archivos registrados. Detalle: " + error.Message, error);
                }
            }
            finally { mutex.ReleaseMutex(); }
        }
    }

    internal static bool Remove(string directory, ScreenFile[] files, Action gameClosed, Action<int> beforeDelete,
        Func<PatchStatus> patch = null)
    {
        using (Mutex mutex = Lock(directory))
        {
            Acquire(mutex);
            try
            {
                gameClosed();
                Inspect(directory, files); // Check the whole file set before deleting the first file.
                string marker = Path.Combine(directory, MarkerName);
                bool owned = Exists(marker);
                bool patched = patch != null && patch() != PatchStatus.AlreadyInstalled;
                if (!owned) return patched;
                try
                {
                    // Remove the loading proxy first; keep the record until removal finishes.
                    for (int i = files.Length - 1; i >= 0; i--)
                    {
                        if (beforeDelete != null) beforeDelete(i);
                        gameClosed();
                        Inspect(directory, files);
                        string file = Path.Combine(directory, files[i].Name);
                        if (Exists(file)) File.Delete(file);
                    }
                    Inspect(directory, files);
                    File.Delete(marker);
                    return true;
                }
                catch (Exception error)
                {
                    throw new IOException("La retirada de pantalla no se completó. Conserva el registro y vuelve a intentarlo con el juego cerrado. Detalle: " + error.Message, error);
                }
            }
            finally { mutex.ReleaseMutex(); }
        }
    }
}
