using System;
using System.IO;
using System.Reflection;

// Individual guard maintenance. Community admission is limited to this exact native delta.
internal static class CurseGuard
{
    internal const string BeforeHash = "af59a5bbb4956f50dd671a4a52753376b7b359b2028c0331a2db5fde6f64c965";
    internal const string AfterHash = "cd35437004a441b7e70a8f0fd606003dea6c1e084667a7d813736c5d82a456cc";
    private const string VanillaPak = "6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50";
    private const string CommunityPak = "f946faf95e31211da3443fec6956a811a42b701ecaf7291a2794dedf04cec81e";

    internal static bool IsCommand(string command)
    {
        return command == "--check-curse-guard" || command == "--apply-curse-guard" || command == "--remove-curse-guard";
    }

    internal static PatchStatus Execute(string command, string path)
    {
        if (!IsCommand(command)) throw new ArgumentException("Orden de protección de maldición desconocida.");
        string target = Path.GetFullPath(path);
        string pak = Path.Combine(Path.GetDirectoryName(target), "Packs", "data.pak");
        string pakHash = PatchEngine.HashFile(pak);
        if (pakHash != VanillaPak && pakHash != CommunityPak)
            throw new InvalidOperationException("La protección de maldición requiere el data.pak exacto de Steam vanilla o Community v12 definitivo revisado.");
        bool remove = command == "--remove-curse-guard";
        PatchSpec spec = new PatchSpec(remove ? AfterHash : BeforeHash, remove ? BeforeHash : AfterHash, pakHash,
            remove ? "5267caa51427dc679d9cc4e5be0a3351bc702dab98d1a1949b980bb276ed3c32" :
                "2c57f63766cae3ed4936f1fd16954e0a1b9ec606b3261bfe0c577f2a0a447d42",
            4460544, 4460544, true);
        byte[] recipe;
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(remove ? "Syncrash.CurseGuardRemove" : "Syncrash.CurseGuard"))
        {
            if (stream == null) throw new InvalidDataException("Falta la receta de protección de maldición.");
            using (var memory = new MemoryStream()) { stream.CopyTo(memory); recipe = memory.ToArray(); }
        }
        return command == "--check-curse-guard" ? PatchEngine.Check(target, spec, recipe, PatchEngine.RequireGameClosed) :
            PatchEngine.Apply(target, spec, recipe, PatchEngine.RequireGameClosed, null, null);
    }
}
