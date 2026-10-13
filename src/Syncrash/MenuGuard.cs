using System;
using System.IO;
using System.Reflection;
internal static class MenuGuard
{
    internal const string OldHash = "cd35437004a441b7e70a8f0fd606003dea6c1e084667a7d813736c5d82a456cc", NewHash = "f34b4684bf65eab26fa7e6f65ec5ffd196846d7fb4f0c0501a620cf9f280db04", WithoutCurseHash = "ad10ac2ffbc6dfad3029585798b302dfeb11cd70a141f2924485f680125d42a1";
    internal const string Rc1Hash = "0c3d71821ac06a55f937e6d9ab38550e6ce14aa6f2acde213a13d2bb880a1c79", Rc1WithoutCurseHash = "c62ac2f66e687413b4568095d9f5a87e15d7ff6a0ffcc188090dd587a11cbc77";
    internal static PatchSpec Upgrade(string pak)
    { return Spec(OldHash, NewHash, pak, "menu-guard");
    }
    internal static PatchSpec RestoreCurse(string pak)
    { return Spec(WithoutCurseHash, NewHash, pak, "curse-with-menu");
    }
    internal static PatchSpec Spec(string a,string b,string pak,string name)
    {
        string hash;
        switch(name)
        {
            case "recipe": hash="c4a11d6a7b3fb4f935be0f338e3d57c8b4fe676b51ea7204d4d2569c5eaf7bea";
            break;
            case "upgrade-v2": hash="d1706177fb230899c89a268611b5d097420c40f27d1d149471ba38af17b55447";
            break;
            case "upgrade-laa": hash="e32b83296a76b6b762ff0021a6afbdb1565ed0a1faa32a399b668c529c28363c";
            break;
            case "menu-guard": hash="be26199d58f0d9ffd52630d2c53c61a809897575bf390a531fd10958999e31c1";
            break;
            case "menu-guard-remove": hash="ffd5adfb808258481788423a2066ed6bbcb4929c0aa3a62329de1836a30107b5";
            break;
            case "menu-only-remove": hash="e8fcbf58ae73ed8082b55502f3a62f1ef84a0bf8ecc415fe94f79e383024a1b2";
            break;
            case "menu-only": hash="b9a26b2f86b4458c84a8bf06a599bcf6de3a1fdd9d2b6b019a2c45858087db13";
            break;
            case "curse-with-menu": hash="70807e164fdf94b5eaccb59399757888cf3ddb09eedff18fc976839a7363e871";
            break;
            case "curse-with-menu-remove": hash="1577af404951320ed490294bcf35d3b98521cff0ecb328a7abca540329c17f5a";
            break;
            case "menu-rc1": hash="bffbe6aba2fcaedf85c1474de5d79ae8d6e443f7c0924110327fbc805d07b947"; break;
            case "menu-only-rc1": hash="b5f589bd4aee91e71f4ca045ed6f9016d49136998b455c7d96d36473e6c2435c"; break;
            case "menu-rc1-remove": hash="b551310dbe311be1f8a9183fed9a28d75d735737a1c5612d3b8fe5adf160426c"; break;
            case "menu-only-rc1-remove": hash="051d53a723a229a2179e022bb82db5929f3557735686674c61bac32d31727780"; break;
            case "upgrade-rc1-minus": hash="2d3e70e7fef3c4550f78430baf4da862c70fb43ce2e1d8358cc013909bfe6a6e"; break;
            case "curse-rc1": hash="45eeb9933cc54fe85b2f45b9a1aaac34979b56fc4319467ed06729817641146d"; break;
            case "curse-rc1-remove": hash="bfe48f60cd0aa59d80cb6731601487a4ab261a82ece1b4470d2a735355b08c52"; break;
            default: throw new InvalidDataException("Receta desconocida.");
        }
        return new PatchSpec(a,b,pak,hash,4460544,4460544,true);
    }
    internal static bool IsCommand(string command)
    { return command == "--check-menu-guard" || command == "--apply-menu-guard" || command == "--remove-menu-guard";
    }
    internal static PatchStatus Execute(string command,string path)
    {
        if (!IsCommand(command) && !CurseGuard.IsCommand(command)) throw new ArgumentException("Orden de protección desconocida.");
        string current=PatchEngine.HashFile(path);
        string pak=PatchEngine.HashFile(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path)),"Packs","data.pak"));
        if(pak!="6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50" && pak!="f946faf95e31211da3443fec6956a811a42b701ecaf7291a2794dedf04cec81e") throw new InvalidDataException("data.pak no admitido.");
        bool curse=command.Contains("curse"), remove=command.Contains("remove");
        string a,b,name;
        if(curse)
        { a=WithoutCurseHash;
            b=NewHash;
            name="curse-with-menu";
        }
        else if(current==WithoutCurseHash || current==CurseGuard.BeforeHash)
        { a=CurseGuard.BeforeHash;
            b=WithoutCurseHash;
            name="menu-only";
        }
        else
        { a=OldHash;
            b=NewHash;
            name="menu-guard";
        }
        if (curse && (current == Rc1Hash || current == Rc1WithoutCurseHash))
        {
            a = Rc1WithoutCurseHash; b = Rc1Hash; name = "curse-rc1";
        }
        if (!curse && (current == Rc1Hash || current == Rc1WithoutCurseHash))
        {
            bool withoutCurse = current == Rc1WithoutCurseHash;
            a = current;
            b = remove ? (withoutCurse ? CurseGuard.BeforeHash : OldHash) : (withoutCurse ? WithoutCurseHash : NewHash);
            name = withoutCurse ? "menu-only-rc1" : "menu-rc1";
            if (remove) name += "-remove";
            remove = false;
        }
        if(remove)
        {string swap=a;a=b;
            b=swap;name += "-remove";}
        var spec=Spec(a,b,pak,name);
        byte[] raw;
        using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash."+name)) using(var ms=new MemoryStream())
        {
            if (stream == null) throw new InvalidDataException("Falta la receta de protección de sala.");
            stream.CopyTo(ms);
            raw=ms.ToArray();}
        return command.Contains("check") ? PatchEngine.Check(path,spec,raw,PatchEngine.RequireGameClosed) : PatchEngine.Apply(path,spec,raw,PatchEngine.RequireGameClosed,null,null);
    }
}
