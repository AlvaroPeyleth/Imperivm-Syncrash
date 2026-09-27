using System;
using System.IO;
using System.Threading.Tasks;

internal static partial class SyncrashTests
{
    private static void EmbeddedSources()
    {
        using (var f = new Fixture())
        {
            Throws<ArgumentException>(() => Syncrash.PatchGame(f.Target, false, true));
            string target = Path.Combine(f.Root, "sources.zip");
            var assembly = System.Reflection.Assembly.ReflectionOnlyLoad(File.ReadAllBytes(appPath));
            bool embedded = Array.IndexOf(assembly.GetManifestResourceNames(), "Syncrash.ScreenSources") >= 0;
            Assert(Cli("--export-screen-sources \"" + target + "\"") == (embedded ? 0 : 1), "export exit code");
            if (embedded)
            {
                byte[] data = File.ReadAllBytes(target);
                using (Stream stream = assembly.GetManifestResourceStream("Syncrash.ScreenSourcesHash"))
                using (var reader = new StreamReader(stream))
                    Assert(PatchEngine.HashBytes(data) == reader.ReadToEnd().Trim(), "source archive hash mismatch");
                Assert(data.Length > 4 && data[0] == 'P' && data[1] == 'K', "not a source ZIP");
            }
            else Assert(!File.Exists(target), "base build left an export");
            File.WriteAllText(target, "user file");
            Assert(Cli("--export-screen-sources \"" + target + "\"") == 1, "overwrote existing target");
            Assert(File.ReadAllText(target) == "user file", "existing export was modified");
        }
    }

    private static ScreenFile[] ScreenFixture()
    {
        string[] names = { "Syncrash-screen-LICENSE.txt", "dxwnd.dxw", "dxwnd.dll", "winmm.dll" };
        var files = new ScreenFile[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            byte[] data = { 11, (byte)i, 29 };
            files[i] = new ScreenFile(names[i], PatchEngine.HashBytes(data)) { Data = data };
        }
        return files;
    }

    private static void ScreenLifecycle()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            Assert(ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null), "not installed");
            Assert(ScreenCompatibility.Inspect(f.Root, files), "screen incomplete");
            DateTime old = new DateTime(2020, 1, 1);
            foreach (ScreenFile file in files) File.SetLastWriteTimeUtc(Path.Combine(f.Root,file.Name), old);
            Assert(!ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null), "idempotence");
            foreach (ScreenFile file in files) Assert(File.GetLastWriteTimeUtc(Path.Combine(f.Root,file.Name)) == old, "rewritten");
            ScreenCompatibility.Remove(f.Root, files, delegate { }, null);
            ScreenCompatibility.Remove(f.Root, files, delegate { }, null);
            Assert(!File.Exists(Path.Combine(f.Root,ScreenCompatibility.MarkerName)), "marker left");
            foreach (ScreenFile file in files) Assert(!File.Exists(Path.Combine(f.Root,file.Name)), "screen file left");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Result) && Hex(File.ReadAllBytes(f.Pak)) == Hex(f.PakData), "game changed by removal");
        }
    }

    private static void ScreenDeselection()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            Assert(ScreenCompatibility.Remove(f.Root, files, delegate { }, null, () => f.Apply()), "base not applied without screen");
            Assert(!ScreenCompatibility.Remove(f.Root, files, delegate { }, null, () => f.Apply()), "unchecked repeat changed files");
            ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null);
            string voices = Path.Combine(f.Root, "CurrentLang", "voices");
            Directory.CreateDirectory(voices);
            File.WriteAllText(Path.Combine(voices, "owned.wav"), "voices remain");
            File.WriteAllText(Path.Combine(f.Root, "unrelated.txt"), "user file");
            Assert(ScreenCompatibility.Remove(f.Root, files, delegate { }, null, () => f.Apply()), "screen not removed");
            Assert(!ScreenCompatibility.Remove(f.Root, files, delegate { }, null, () => f.Apply()), "removed twice");
            foreach (ScreenFile file in files) Assert(!File.Exists(Path.Combine(f.Root, file.Name)), "screen residue");
            Assert(!File.Exists(Path.Combine(f.Root, ScreenCompatibility.MarkerName)), "ownership residue");
            Assert(File.ReadAllText(Path.Combine(voices, "owned.wav")) == "voices remain", "voices changed");
            Assert(File.ReadAllText(Path.Combine(f.Root, "unrelated.txt")) == "user file", "unrelated changed");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Result) && Hex(File.ReadAllBytes(f.Pak)) == Hex(f.PakData), "base or PAK changed");
            Assert(ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null), "cannot enable again");
        }
    }

    private static void ScreenDeselectionFailure()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            File.WriteAllBytes(Path.Combine(f.Root, "winmm.dll"), files[3].Data);
            bool patched = false;
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root, files, delegate { }, null,
                () => { patched = true; return f.Apply(); }));
            Assert(!patched && Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source), "foreign screen patched base");
            File.Delete(Path.Combine(f.Root, "winmm.dll"));
            ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null);
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root, files, delegate { }, null,
                () => { throw new IOException("base rejected"); }));
            Assert(ScreenCompatibility.Inspect(f.Root, files), "base failure removed screen");
            File.WriteAllText(Path.Combine(f.Root, "dxwnd.dxw"), "edited");
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root, files, delegate { }, null,
                () => { patched = true; return f.Apply(); }));
            Assert(!patched, "changed screen patched base");
            foreach (ScreenFile file in files) Assert(File.Exists(Path.Combine(f.Root, file.Name)), "partial removal");
        }
    }

    private static void ScreenForeignFiles()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            string collision = Path.Combine(f.Root,"winmm.dll");
            File.WriteAllBytes(collision, files[3].Data); // Same bytes still need explicit ownership.
            bool called = false;
            Throws<IOException>(() => ScreenCompatibility.Install(f.Root, files,
                () => { called = true; return f.Apply(); }, delegate { }, null));
            Assert(!called && Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source), "collision patched game");
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root, files, delegate { }, null));
            Assert(File.Exists(collision), "foreign file removed");
            File.Delete(collision);
            File.WriteAllText(Path.Combine(f.Root,ScreenCompatibility.MarkerName), "foreign marker");
            Throws<IOException>(() => ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { }, null));
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source), "bad marker patched game");
        }
    }

    private static void ScreenInterrupted()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            IOException error = Throws<IOException>(() => ScreenCompatibility.Install(f.Root, files, () => f.Apply(), delegate { },
                i => { if (i == 2) throw new UnauthorizedAccessException("denied"); }));
            Assert(error.Message.Contains("ejecutable quedó instalado") && SyncrashWindow.DescribeApplyError(error).Contains("Acceso denegado"), "state/advice lost");
            Assert(!File.Exists(Path.Combine(f.Root,"winmm.dll")), "proxy installed before dependencies");
            Assert(!ScreenCompatibility.Inspect(f.Root,files), "partial called complete");
            Assert(Directory.GetFiles(f.Root,"*.tmp").Length == 0, "stage leaked");
            Assert(ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },null), "resume failed");
            ScreenCompatibility.Remove(f.Root,files,delegate { },null);
            Throws<IOException>(() => ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { }, i => { throw new IOException("interrupted"); }));
            ScreenCompatibility.Remove(f.Root,files,delegate { },null);
            Assert(!File.Exists(Path.Combine(f.Root,ScreenCompatibility.MarkerName)), "partial removal failed");
        }
    }

    private static void ScreenInvalidPayload()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            files[2].Data[0]++;
            Throws<IOException>(() => ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },null));
            Assert(Hex(File.ReadAllBytes(f.Target))==Hex(f.Source), "bad payload patched executable");
            files = ScreenFixture();
            Throws<InvalidOperationException>(() => ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),
                delegate { throw new InvalidOperationException("game running"); },null));
            Assert(!File.Exists(Path.Combine(f.Root,ScreenCompatibility.MarkerName)), "game-open check wrote");
        }
    }

    private static void ScreenChangedRemoval()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },null);
            File.WriteAllText(Path.Combine(f.Root,"dxwnd.dxw"),"changed config");
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root,files,delegate { },null));
            foreach (ScreenFile file in files) Assert(File.Exists(Path.Combine(f.Root,file.Name)),"partial removal before validation");
            Assert(File.ReadAllText(Path.Combine(f.Root,"dxwnd.dxw"))=="changed config","changed config lost");
        }
    }

    private static void ScreenInterruptedRemoval()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },null);
            Throws<IOException>(() => ScreenCompatibility.Remove(f.Root,files,delegate { },
                i => { if(i==2) throw new UnauthorizedAccessException("denied"); }));
            Assert(!File.Exists(Path.Combine(f.Root,"winmm.dll")),"proxy removal order");
            Assert(File.Exists(Path.Combine(f.Root,ScreenCompatibility.MarkerName)),"ownership lost");
            ScreenCompatibility.Remove(f.Root,files,delegate { },null);
            Assert(!ScreenCompatibility.Inspect(f.Root,files),"not removed");
        }
    }

    private static void ScreenConcurrent()
    {
        using (var f = new Fixture())
        {
            ScreenFile[] files = ScreenFixture();
            ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },i => {
                if(i != 0) return;
                Task other = Task.Run(() => Throws<PatchBusyException>(() =>
                    ScreenCompatibility.Install(f.Root,files,()=>f.Apply(),delegate { },null)));
                if(!other.Wait(5000)) throw new Exception("concurrent screen check timed out");
            });
            Assert(ScreenCompatibility.Inspect(f.Root,files),"concurrent check broke install");
        }
    }
}
