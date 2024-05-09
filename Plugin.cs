using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

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

    [HarmonyPatch(typeof(SaverRoleCoreInfo))]
    [HarmonyPatch(nameof(SaverRoleCoreInfo.ResetDataForNewGenaration))]
    class AddAllRoundValues {
        static void Postfix(SaverRoleCoreInfo __instance) {
            try {
                List<NewFamilyInfo> families = SingleTon<NewRecordMgr>.Instance.CurSaver.GetFamilyInfos(SingleTon<NewRecordMgr>.Instance.CurrentRecord.ConfigID, SingleTon<NewRecordMgr>.Instance.CurrentRecord.RecordIndex);
                families.Sort((a, b) => int.Parse(a.Values[EFamilyDataType.IntGeneration]).CompareTo(int.Parse(b.Values[EFamilyDataType.IntGeneration])));
                int addTo = SingleTon<CollectionMgr>.Instance.LastGenarationLevel;
                for (int i = 1; i < addTo; i++)
                {
                    int familyID = i-1;
                    if (familyID >= families.Count) {
                        Logger.CreateLogSource("ChipabuPatch").LogWarning($"Aborting stat buffs at {familyID} (goal: {addTo-1})");
                        break;
                    }

                    int endingID = families[familyID].GetValueAsInt(EFamilyDataType.IntEndingID);
                    TableEnding ending = SingleTon<TableMgr>.Instance.TableEnding.GetItem(endingID);

                    if (ending == null) continue;

                    __instance.iq_round += ending.Family_iq_round;
                    __instance.eq_round += ending.Family_eq_round;
                    __instance.imagination_round += ending.Family_imagination_round;
                    __instance.memory_round += ending.Family_memory_round;
                    __instance.stamination_round += ending.Family_stamination_round;
                }
            } catch (Exception e) {
                Logger.CreateLogSource("ChipabuPatch").LogError("Failed to apply stat buffs");
                Logger.CreateLogSource("ChipabuPatch").LogError(e);
            }
        }
    }
}
