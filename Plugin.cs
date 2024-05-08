using System;
using BepInEx;
using HarmonyLib;

namespace Chipabu
{
    [BepInPlugin("dev.qixils.chipabu", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            var harmony = new Harmony("dev.qixils.chipabu.patch");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(TranslateMgr))]
    [HarmonyPatch(nameof(TranslateMgr.LoadTables))]
    class BannedWordsPatch {
        static readonly AccessTools.FieldRef<TranslateMgr, string[]> banWordsRef =
        AccessTools.FieldRefAccess<TranslateMgr, string[]>("m_banWords");

        static void Postfix(TranslateMgr __instance) {
            banWordsRef(__instance) = new string[0];
        }
    }

    [HarmonyPatch(typeof(CompositeWnd))]
    [HarmonyPatch("Get_value_sign")]
    class FixSignsPatch {
        static void Postfix(ref int value, ref string __result) {
            string prefix = value > 0 ? "+" : "";
            __result = prefix + value.ToString() + " ";
        }
    }
}
