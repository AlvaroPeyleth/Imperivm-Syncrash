using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

internal static partial class SyncrashTests
{
    private static VoiceCatalog VoiceFixture()
    {
        var catalog = new VoiceCatalog { Format = 1, Languages = new List<VoiceLanguage>() };
        int value = 20;
        foreach (string name in new[] { "spanish", "italian", "english" })
        {
            var language = new VoiceLanguage { Language = name, Files = new List<VoiceFile>() };
            // English has an additional destination; exercise deleting surplus files on switching back.
            for (int i = 1; i <= (name == "english" ? 3 : 2); i++)
            {
                byte[] bytes = { (byte)value++, (byte)i, 42 };
                language.Files.Add(new VoiceFile { Destination = "CURRENTLANG/VOICES/TEST/" + i + ".WAV",
                    Source = "TEST/PHRASE" + i + ".WAV", Size = bytes.Length, Data = bytes, Hash = PatchEngine.HashBytes(bytes) });
            }
            catalog.Languages.Add(language);
        }
        return catalog;
    }

    private static bool Voices(Fixture f, VoiceCatalog c, string language, Action<int> fault = null)
    {
        return VoiceCompatibility.Configure(f.Root, c, language == null ? null : VoiceCompatibility.Language(c, language), delegate { }, delegate { }, fault);
    }

    private static string VoicePath(Fixture f, VoiceFile file) { return VoiceCompatibility.SafePath(f.Root, file.Destination); }

    private static void AssertVoices(Fixture f, VoiceCatalog c, string language)
    {
        foreach (VoiceFile file in VoiceCompatibility.Language(c, language).Files)
            Assert(PatchEngine.HashFile(VoicePath(f, file)) == file.Hash, "wrong language payload");
        Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && Hex(File.ReadAllBytes(f.Pak)) == Hex(f.PakData), "voices modified game resources");
    }

    private static void VoiceLifecycle()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture();
            Assert(!Voices(f, c, null), "empty removal changed files");
            Assert(Voices(f, c, "spanish"), "first installation");
            AssertVoices(f, c, "spanish");
            string first = VoicePath(f, c.Languages[0].Files[0]);
            DateTime timestamp = new DateTime(2020, 1, 1);
            File.SetLastWriteTimeUtc(first, timestamp);
            Assert(!Voices(f, c, "spanish") && File.GetLastWriteTimeUtc(first) == timestamp, "idempotence wrote audio");
            foreach (string name in new[] { "english", "italian", "spanish" })
            {
                Assert(Voices(f, c, name), "language switch"); AssertVoices(f, c, name);
            }
            Assert(!File.Exists(VoicePath(f, c.Languages[2].Files[2])), "extra English file remained");
            Assert(Voices(f, c, null), "remove"); Assert(!Voices(f, c, null), "repeat remove");
            Assert(!File.Exists(first) && !File.Exists(Path.Combine(f.Root, VoiceCompatibility.Marker)), "owned files remain");
        }
    }

    private static void VoiceForeign()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); var lang = c.Languages[0]; string first = VoicePath(f, lang.Files[0]);
            Directory.CreateDirectory(Path.GetDirectoryName(first)); File.WriteAllBytes(first, lang.Files[0].Data);
            bool baseCalled = false;
            Throws<IOException>(() => VoiceCompatibility.Configure(f.Root, c, lang, delegate { }, () => baseCalled = true, null));
            Assert(!baseCalled && File.Exists(first), "adopted foreign identical file or patched base");
            Assert(!Voices(f, c, null) && File.Exists(first), "removed foreign file");
            File.Delete(first); Voices(f, c, "spanish");
            string second = VoicePath(f, lang.Files[1]); File.WriteAllText(second, "user edit");
            foreach (string name in new[] { "italian", "spanish", null }) Throws<IOException>(() => Voices(f, c, name));
            Assert(PatchEngine.HashFile(first) == lang.Files[0].Hash && File.ReadAllText(second) == "user edit", "partial removal before collision");
        }
    }

    private static void VoiceRecovery()
    {
        foreach (string next in new[] { "spanish", "italian", "english", null })
            for (int fail = 0; fail <= 2; fail++)
                using (var f = new Fixture())
                {
                    var c = VoiceFixture(); Voices(f, c, "spanish");
                    Throws<IOException>(() => Voices(f, c, "english", i => { if (i == fail) throw new IOException("power loss simulation"); }));
                    Assert(File.Exists(Path.Combine(f.Root, VoiceCompatibility.Marker)), "lost journal");
                    Voices(f, c, next);
                    if (next != null) AssertVoices(f, c, next);
                    else foreach (var lang in c.Languages) foreach (var file in lang.Files) Assert(!File.Exists(VoicePath(f, file)), "interrupted install/remove left files");
                }
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); Voices(f, c, "english");
            Throws<IOException>(() => Voices(f, c, null, i => { if (i == 1) throw new IOException("interrupted removal"); }));
            Voices(f, c, "italian"); AssertVoices(f, c, "italian");
        }
    }

    private static void VoiceMissingAndModifiedPending()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); Voices(f, c, "spanish");
            string first = VoicePath(f, c.Languages[0].Files[0]); File.Delete(first);
            Assert(Voices(f, c, "spanish"), "missing file not restored"); AssertVoices(f, c, "spanish");
            Throws<IOException>(() => Voices(f, c, "english", i => { if (i == 1) throw new IOException("fault"); }));
            File.WriteAllText(first, "preserve");
            Throws<IOException>(() => Voices(f, c, null)); Assert(File.ReadAllText(first) == "preserve", "removed changed pending file");
        }
    }

    private static void VoiceLegacyMigration()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); var lang = c.Languages[0];
            Voices(f, c, "spanish");
            string marker = Path.Combine(f.Root, VoiceCompatibility.Marker);
            using (var output = File.Create(marker)) new DataContractJsonSerializer(typeof(VoiceRecord)).WriteObject(output,
                new VoiceRecord { Owner = "syncrash-voice-trial-v1", LegacyLanguage = "spanish", LegacyFiles = lang.Files });
            DateTime stamp = new DateTime(2020, 1, 1); string first = VoicePath(f, lang.Files[0]); File.SetLastWriteTimeUtc(first, stamp);
            Assert(Voices(f, c, "spanish"), "legacy not migrated");
            Assert(File.GetLastWriteTimeUtc(first) == stamp, "migration rewrote matching audio");
            AssertVoices(f, c, "spanish"); Voices(f, c, "english"); AssertVoices(f, c, "english");
        }
    }

    private static void VoiceBadRecord()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); Voices(f, c, "spanish");
            string marker = Path.Combine(f.Root, VoiceCompatibility.Marker);
            foreach (string record in new[] { "{}", "{", "{\"owner\":\"other\"}",
                "{\"owner\":\"syncrash-voices-v2\",\"status\":\"installed\",\"languages\":[\"spanish\"],\"target_language\":\"english\"}" })
            {
                File.WriteAllText(marker, record);
                Throws<Exception>(() => Voices(f, c, null)); AssertVoices(f, c, "spanish");
            }
            Throws<InvalidDataException>(() => VoiceCompatibility.SafePath(f.Root, "../outside.wav"));
            Throws<InvalidDataException>(() => VoiceCompatibility.SafePath(f.Root, "Currentlang/voices/x:stream"));
        }
    }

    private static void VoiceClosedAndConcurrent()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); bool called = false;
            Throws<InvalidOperationException>(() => VoiceCompatibility.Configure(f.Root, c, c.Languages[0],
                () => { throw new InvalidOperationException("game open"); }, () => called = true, null));
            Assert(!called && !Directory.Exists(Path.Combine(f.Root, VoiceCompatibility.StateDirectory)), "running game changed");
            VoiceCompatibility.Configure(f.Root, c, c.Languages[0], delegate { }, delegate { }, i => {
                if (i == 0) Task.Run(() => Throws<PatchBusyException>(() => Voices(f, c, "italian"))).Wait();
            });
            AssertVoices(f, c, "spanish");
        }
    }

    private static void VoiceSettings()
    {
        using (var f = new Fixture())
        {
            string ini = Path.Combine(f.Root, "Settings.ini");
            File.WriteAllText(ini, "; comment\n[Other]\nDefault=French\n[Language]\nDefault = Italian\n");
            Assert(VoiceCompatibility.SelectedLanguage(f.Root) == "italian", "wrong INI section");
            File.AppendAllText(ini, "Default=English\n"); Throws<InvalidDataException>(() => VoiceCompatibility.SelectedLanguage(f.Root));
            File.WriteAllText(ini, "[Other]\nDefault=Spanish\n"); Throws<InvalidDataException>(() => VoiceCompatibility.SelectedLanguage(f.Root));
            Throws<InvalidDataException>(() => VoiceCompatibility.Language(VoiceFixture(), "french"));
        }
    }

    private static void VoicePakValidation()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); var lang = c.Languages[0];
            Directory.CreateDirectory(Path.Combine(f.Root, "local"));
            string pak = Path.Combine(f.Root, "local/spanish.pak");
            byte[] bytes;
            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                byte[] header = new byte[40]; Array.Copy(Encoding.ASCII.GetBytes("HMMSYS PackFile\n\x1a"), header, 17);
                Array.Copy(BitConverter.GetBytes((uint)lang.Files.Count), 0, header, 32, 4); writer.Write(header);
                int offset = 40;
                foreach (var file in lang.Files) offset += 10 + file.Source.Length;
                foreach (var file in lang.Files)
                {
                    writer.Write((byte)file.Source.Length); writer.Write((byte)0); writer.Write(Encoding.ASCII.GetBytes(file.Source));
                    writer.Write((uint)offset); writer.Write((uint)file.Size); offset += file.Size;
                }
                foreach (var file in lang.Files) writer.Write(file.Data);
                bytes = mem.ToArray();
            }
            File.WriteAllBytes(pak, bytes); lang.PakHash = PatchEngine.HashBytes(bytes);
            VoiceCompatibility.LoadAudio(f.Root, lang, PatchEngine.HashBytes(f.PakData));
            lang.Files[0].Data = new byte[] { 7 };
            Throws<InvalidDataException>(() => Voices(f, c, "spanish"));
            VoiceCompatibility.LoadAudio(f.Root, lang, PatchEngine.HashBytes(f.PakData));
            Assert(Voices(f, c, "spanish"), "valid pack failed");
            bytes[40+2+lang.Files[0].Source.Length] = 0; File.WriteAllBytes(pak, bytes);
            Throws<InvalidDataException>(() => VoiceCompatibility.LoadAudio(f.Root, lang, PatchEngine.HashBytes(f.PakData)));
            lang.PakHash = PatchEngine.HashBytes(bytes); // Test bounds independently from the outer hash.
            Throws<InvalidDataException>(() => VoiceCompatibility.LoadAudio(f.Root, lang, PatchEngine.HashBytes(f.PakData)));
        }
    }

    private static void VoiceEmbeddedCatalog()
    {
        VoiceCatalog catalog = VoiceCompatibility.Catalog();
        Assert(catalog.Format == 1 && catalog.Languages.Count == 3, "embedded catalog schema");
        foreach (VoiceLanguage language in catalog.Languages)
        {
            Assert(language.Files.Count == (language.Language == "english" ? 393 : 188), "coverage changed");
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (VoiceFile file in language.Files) Assert(names.Add(file.Destination) && file.Size > 0, "duplicate mapping");
        }
        using (var f = new Fixture())
        {
            Assert(Cli("--check-voices \"" + f.Target + "\"") == 1, "check accepted missing Settings");
            Assert(!Directory.Exists(Path.Combine(f.Root, VoiceCompatibility.StateDirectory)), "read-only check created state");
        }
    }

    private static void VoiceBaseFailure()
    {
        using (var f = new Fixture())
        {
            var c = VoiceFixture();
            Throws<IOException>(() => VoiceCompatibility.Configure(f.Root, c, c.Languages[0], delegate { },
                () => { throw new IOException("base failure"); }, null));
            Assert(!File.Exists(Path.Combine(f.Root, VoiceCompatibility.Marker)), "base failure published audio journal");
            foreach (VoiceFile file in c.Languages[0].Files) Assert(!File.Exists(VoicePath(f, file)), "base failure installed audio");
        }
    }

    private static void VoiceJunctionAndRace()
    {
        using (var f = new Fixture())
        using (var outside = new Fixture())
        {
            string junction = Path.Combine(f.Root, "CURRENTLANG");
            var start = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/d /c mklink /J \"" + junction + "\" \"" + outside.Root + "\"")
                { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
            using (var process = System.Diagnostics.Process.Start(start))
            {
                process.StandardOutput.ReadToEnd(); process.StandardError.ReadToEnd(); process.WaitForExit();
                Assert(process.ExitCode == 0, "could not construct junction fixture");
            }
            try
            {
                var c = VoiceFixture(); Throws<IOException>(() => Voices(f, c, "spanish"));
                Assert(!Directory.Exists(Path.Combine(outside.Root, "VOICES")), "wrote through junction");
            }
            finally { Directory.Delete(junction); } // Delete only the link, never its target.
        }
        using (var f = new Fixture())
        {
            var c = VoiceFixture(); string first = VoicePath(f, c.Languages[0].Files[0]);
            Throws<IOException>(() => Voices(f, c, "spanish", i => {
                if (i == 0) { Directory.CreateDirectory(Path.GetDirectoryName(first)); File.WriteAllText(first, "racing user file"); }
            }));
            Assert(File.ReadAllText(first) == "racing user file", "late collision overwritten");
        }
    }
}
