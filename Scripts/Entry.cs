using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace Character_ZZK.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "Character_ZZK";

    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        Logger.Info("Character_ZZK initializing...");

        Assembly assembly = Assembly.GetExecutingAssembly();

        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        Logger.Info("Character_ZZK initialized successfully.");
    }
}