using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppSystem;
using UnhollowerRuntimeLib;

namespace RichPresence
{
    [BepInPlugin("com.chasetug.RichPresence", "RichPresence", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource log;
        private static Harmony _harmony;
        
        public override void Load()
        {
            log = Log;
            ClassInjector.RegisterTypeInIl2Cpp<RichPresenceManager>();
            PlayFabManager.add_OnLoginSuccess((Action) RichPresenceManager.Setup);
            
            _harmony = new Harmony("com.chasetug.RichPresence"); 
            _harmony.PatchAll();
        }
    }
}   