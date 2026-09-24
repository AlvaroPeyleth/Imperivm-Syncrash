using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

internal static class SyncrashTests
{
    private static int passed;
    private static string appPath;

    private static int Main(string[] args)
    {
        if (args.Length != 1 || !File.Exists(args[0])) throw new ArgumentException("Provide the built Syncrash.exe path.");
        appPath = args[0];
        Run("synthetic reconstruction and exact output", Reconstruction);
        Run("read-only check and installed check", ReadOnlyCheck);
        Run("apply and idempotence without writes", ApplyAndIdempotence);
        Run("bad source and PAK rejected", BadInputs);
        Run("truncated and out-of-range recipe rejected", BadRecipe);
        Run("wrong expected output rejected", BadOutput);
        Run("game open and failed process check rejected", ProcessFailures);
        Run("failure before replacement preserves original", BeforeReplaceFailure);
        Run("replacement denied preserves original", ReplaceDenied);
        Run("failure after replacement reports installed state", AfterReplaceFailure);
        Run("same-target concurrent application excluded", ConcurrentApply);
        Run("CLI --check and retired --test do not write", CliReadOnly);
        Console.WriteLine("PASS: " + passed + " synthetic/Windows tests");
        return 0;
    }

    private static void Run(string name, Action test)
    {
        test();
        passed++;
        Console.WriteLine("PASS " + name);
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static T Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T error) { return error; }
        throw new Exception("Expected " + typeof(T).Name);
    }

    private static string Hex(byte[] data) { return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant(); }

    private sealed class Fixture : IDisposable
    {
        internal readonly string Root, Target, Pak;
        internal readonly byte[] Source = { 1, 2, 3, 4 };
        internal readonly byte[] Result = { 1, 9, 3, 4, 7, 8 };
        internal readonly byte[] PakData = { 5, 6, 7 };
        internal Recipe Recipe;
        internal PatchSpec Spec;
        internal byte[] Raw;

        internal Fixture()
        {
            Root = Path.Combine(Path.GetTempPath(), "Syncrash ensayo ñ con espacios " + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(Root, "Packs"));
            Target = Path.Combine(Root, "gbr.exe");
            Pak = Path.Combine(Root, "Packs", "data.pak");
            File.WriteAllBytes(Target, Source);
            File.WriteAllBytes(Pak, PakData);
            Recipe = new Recipe {
                Schema = "syncrash-exact-delta-v1",
                SourceSha = PatchEngine.HashBytes(Source), ResultSha = PatchEngine.HashBytes(Result),
                SourceBytes = Source.Length, ResultBytes = Result.Length,
                Patches = new List<PatchEntry> { new PatchEntry { Offset = 1, Before = "02", After = "09" } },
                AppendNonzero = new List<PatchEntry> { new PatchEntry { Offset = 4, Before = "0000", After = "0708" } }
            };
            RefreshRecipe();
        }

        internal void RefreshRecipe()
        {
            using (var memory = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(Recipe)).WriteObject(memory, Recipe);
                Raw = memory.ToArray();
            }
            Spec = new PatchSpec(PatchEngine.HashBytes(Source), Recipe.ResultSha,
                PatchEngine.HashBytes(PakData), PatchEngine.HashBytes(Raw), Source.Length, Result.Length);
        }

        internal int Stages() { return Directory.GetFiles(Root, "gbr.syncrash-stage-*.tmp").Length; }
        internal PatchStatus Check(Action process) { return PatchEngine.Check(Target, Spec, Raw, process); }
        internal PatchStatus Apply(Action process, Action before, Action after)
        {
            return PatchEngine.Apply(Target, Spec, Raw, process, before, after);
        }
        internal PatchStatus Apply() { return Apply(delegate { }, null, null); }

        public void Dispose()
        {
            if (!Path.GetFullPath(Root).StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Unsafe fixture cleanup path");
            File.SetAttributes(Target, FileAttributes.Normal);
            Directory.Delete(Root, true);
        }
    }

    private static void Reconstruction()
    {
        using (var f = new Fixture())
        {
            Recipe parsed = PatchEngine.ReadRecipe(f.Raw, f.Spec);
            Assert(Hex(PatchEngine.BuildImage(f.Source, parsed, f.Spec)) == Hex(f.Result), "wrong output");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source), "reconstruction wrote source");
        }
    }

    private static void ReadOnlyCheck()
    {
        using (var f = new Fixture())
        {
            DateTime old = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(f.Target, old);
            Assert(f.Check(delegate { }) == PatchStatus.OriginalAdmitted, "original check");
            Assert(File.GetLastWriteTimeUtc(f.Target) == old && f.Stages() == 0, "check wrote files");
            File.WriteAllBytes(f.Target, f.Result);
            File.SetLastWriteTimeUtc(f.Target, old);
            Assert(f.Check(delegate { }) == PatchStatus.AlreadyInstalled, "installed check");
            Assert(File.GetLastWriteTimeUtc(f.Target) == old && f.Stages() == 0, "installed check wrote files");
        }
    }

    private static void ApplyAndIdempotence()
    {
        using (var f = new Fixture())
        {
            Assert(f.Apply() == PatchStatus.Applied, "not applied");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Result), "installed bytes differ");
            DateTime old = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(f.Target, old);
            Assert(f.Apply() == PatchStatus.AlreadyInstalled, "not idempotent");
            Assert(File.GetLastWriteTimeUtc(f.Target) == old && f.Stages() == 0, "reapply wrote files");
        }
    }

    private static void BadInputs()
    {
        using (var f = new Fixture())
        {
            File.WriteAllBytes(f.Pak, new byte[] { 0 });
            Throws<InvalidOperationException>(delegate { f.Apply(); });
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "bad PAK wrote");
            File.WriteAllBytes(f.Pak, f.PakData);
            File.WriteAllBytes(f.Target, new byte[] { 8, 2, 3, 4 });
            Throws<InvalidOperationException>(delegate { f.Apply(); });
            Assert(f.Stages() == 0, "bad source staged");
        }
    }

    private static void BadRecipe()
    {
        using (var f = new Fixture())
        {
            f.Raw = new byte[] { (byte)'{' };
            f.Spec = new PatchSpec(f.Spec.SourceHash, f.Spec.ResultHash, f.Spec.PakHash,
                PatchEngine.HashBytes(f.Raw), f.Source.Length, f.Result.Length);
            Throws<Exception>(delegate { f.Apply(); });
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "truncated recipe wrote");
            f.Recipe.Patches[0].Offset = 999;
            f.RefreshRecipe();
            Throws<InvalidDataException>(delegate { f.Apply(); });
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "out-of-range recipe wrote");
        }
    }

    private static void BadOutput()
    {
        using (var f = new Fixture())
        {
            f.Recipe.ResultSha = PatchEngine.HashBytes(new byte[] { 9 });
            f.RefreshRecipe();
            Throws<InvalidDataException>(delegate { f.Apply(); });
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "bad output wrote");
        }
    }

    private static void ProcessFailures()
    {
        using (var f = new Fixture())
        {
            Throws<InvalidOperationException>(delegate { f.Apply(delegate { throw new InvalidOperationException("game open"); }, null, null); });
            Throws<IOException>(delegate { f.Apply(delegate { throw new IOException("process query failed"); }, null, null); });
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "process failure wrote");
        }
    }

    private static void BeforeReplaceFailure()
    {
        using (var f = new Fixture())
        {
            PatchOperationException error = Throws<PatchOperationException>(delegate {
                f.Apply(delegate { }, delegate { throw new IOException("simulated pre-replace failure"); }, null);
            });
            Assert(error.Message.Contains("original sigue intacto"), "wrong pre-replace state");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "pre-replace failure wrote");
        }
    }

    private static void AfterReplaceFailure()
    {
        using (var f = new Fixture())
        {
            PatchOperationException error = Throws<PatchOperationException>(delegate {
                f.Apply(delegate { }, null, delegate { throw new IOException("simulated post-replace failure"); });
            });
            Assert(error.Message.Contains("resultado parece instalado"), "wrong post-replace state");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Result) && f.Stages() == 0, "post-replace state differs");
        }
    }

    private static void ReplaceDenied()
    {
        using (var f = new Fixture())
        {
            PatchOperationException error = Throws<PatchOperationException>(delegate {
                f.Apply(delegate { }, delegate { File.SetAttributes(f.Target, FileAttributes.ReadOnly); }, null);
            });
            Assert(error.Message.Contains("original sigue intacto"), "wrong replace-denied state");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Source) && f.Stages() == 0, "denied replace wrote");
        }
    }

    private static void ConcurrentApply()
    {
        using (var f = new Fixture())
        using (var entered = new ManualResetEventSlim(false))
        using (var release = new ManualResetEventSlim(false))
        {
            Task<PatchStatus> first = Task.Run(delegate {
                return f.Apply(delegate { }, delegate { entered.Set(); release.Wait(); }, null);
            });
            Assert(entered.Wait(10000), "first apply did not reach stage");
            try
            {
                string alias = Path.Combine(f.Root, "Packs", "..", "gbr.exe");
                Throws<PatchBusyException>(delegate { PatchEngine.Apply(alias, f.Spec, f.Raw, delegate { }, null, null); });
            }
            finally { release.Set(); }
            Assert(first.Result == PatchStatus.Applied, "first apply failed");
            Assert(Hex(File.ReadAllBytes(f.Target)) == Hex(f.Result) && f.Stages() == 0, "concurrency result wrong");
        }
    }

    private static int Cli(string arguments)
    {
        var start = new ProcessStartInfo(appPath, arguments) { UseShellExecute = false, CreateNoWindow = true };
        using (Process child = Process.Start(start))
        {
            if (!child.WaitForExit(10000)) { child.Kill(); throw new Exception("CLI timed out"); }
            return child.ExitCode;
        }
    }

    private static void CliReadOnly()
    {
        using (var f = new Fixture())
        {
            DateTime old = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(f.Target, old);
            string quoted = "\"" + f.Target + "\"";
            Assert(Cli("--test " + quoted) == 2, "--test accepted");
            Assert(Cli("--check " + quoted) == 1, "production check accepted synthetic input");
            Assert(Cli("--apply") == 2, "ambiguous apply accepted");
            Assert(File.GetLastWriteTimeUtc(f.Target) == old && f.Stages() == 0, "CLI wrote synthetic input");
        }
    }
}
