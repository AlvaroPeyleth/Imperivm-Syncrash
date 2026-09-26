using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

internal static class Syncrash
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SyncrashWindow());
            return 0;
        }
        var utf8 = new UTF8Encoding(false);
        try { Console.OutputEncoding = utf8; }
        catch (IOException)
        {
            // A WinForms executable can have redirected streams without a console handle.
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), utf8) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError(), utf8) { AutoFlush = true });
        }
        if (args.Length == 1 && args[0] == "--detect-only")
        {
            try
            {
                foreach (string path in FindSteamGames()) Console.WriteLine(path);
                return 0;
            }
            catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
        }
        if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]) &&
            args[0] == "--export-screen-sources")
        {
            try { ScreenCompatibility.ExportScreenSources(args[1]); Console.WriteLine("Licencias y fuentes guardadas."); return 0; }
            catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
        }
        if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]) &&
            (args[0] == "--check" || args[0] == "--check-screen" || args[0] == "--apply" ||
             args[0] == "--apply-with-screen" || args[0] == "--apply-with-smoothing" || args[0] == "--remove-screen"))
        {
            try
            {
                if (args[0] == "--remove-screen")
                {
                    Console.WriteLine(ScreenCompatibility.RemoveGame(args[1]));
                    return 0;
                }
                if (args[0] == "--check")
                {
                    PatchStatus state = PatchEngine.CheckGame(args[1]);
                    Console.WriteLine(state == PatchStatus.AlreadyInstalled ? "Parche de memoria y cierres instalado. No se ha modificado ningún archivo." :
                        state == PatchStatus.UpgradeAdmitted ? "Syncrash anterior admitido para actualizar. No se ha modificado ningún archivo." :
                        "Instalación Steam original admitida. No se ha modificado ningún archivo.");
                    return state == PatchStatus.AlreadyInstalled ? 3 : 0;
                }
                if (args[0] == "--check-screen")
                {
                    bool complete;
                    Console.WriteLine(ScreenCompatibility.CheckGame(args[1], out complete));
                    return complete ? 3 : 0;
                }
                bool changed;
                Console.WriteLine(PatchGame(args[1], args[0] == "--apply-with-screen" || args[0] == "--apply-with-smoothing", args[0] == "--apply-with-smoothing", out changed));
                return changed ? 0 : 3;
            }
            catch (PatchBusyException error) { Console.Error.WriteLine(error.Message); return 4; }
            catch (Exception error) { Console.Error.WriteLine(error.Message); return 1; }
        }
        Console.Error.WriteLine("Uso: Syncrash.exe [--detect-only | --check <gbr.exe> | --apply <gbr.exe> | --apply-with-screen <gbr.exe> | --apply-with-smoothing <gbr.exe> | --check-screen <gbr.exe> | --remove-screen <gbr.exe> | --export-screen-sources <nuevo.zip>]. --apply instala solo memoria y cierres; --apply-with-screen añade pantalla adaptable. --test ya no aplica el parche.");
        return 2;
    }

    internal static string PatchGame(string exePath, bool adaptiveScreen)
    {
        bool changed;
        return PatchGame(exePath, adaptiveScreen, out changed);
    }

    internal static string PatchGame(string exePath, bool adaptiveScreen, bool smooth)
    {
        bool changed;
        return PatchGame(exePath, adaptiveScreen, smooth, out changed);
    }

    internal static string PatchGame(string exePath, bool adaptiveScreen, out bool changed)
    {
        return PatchGame(exePath, adaptiveScreen, false, out changed);
    }

    internal static string PatchGame(string exePath, bool adaptiveScreen, bool smooth, out bool changed)
    {
        if (smooth && !adaptiveScreen) throw new ArgumentException("El suavizado necesita pantalla adaptable.");
        if (adaptiveScreen) return ScreenCompatibility.ApplyGame(exePath, smooth, out changed);
        // The base patch neither needs nor changes optional screen files, even if already installed.
        changed = PatchEngine.ApplyGame(exePath) != PatchStatus.AlreadyInstalled;
        return (changed ? "Parche de memoria y cierres instalado." : "El parche de memoria y cierres ya está instalado.") +
            " La configuración de pantalla no se ha cambiado.";
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
