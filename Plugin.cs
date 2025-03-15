using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace Chipabu
{
    [BepInPlugin("dev.qixils.chipabu", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource PatchLogger = BepInEx.Logging.Logger.CreateLogSource("ChipabuPatch");

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            var harmony = new Harmony("dev.qixils.chipabu.patch");
            harmony.PatchAll();
        }

        // public static TableSoItemLang[] newLangs = new TableSoItemLang[] {
        //     new TableSoItemLang() {
        //         id = 871901,
        //         japanese = "stupid boy bitch",
        //         mchinese = "stupid boy bitch",
        //         schinese = "stupid boy bitch",
        //         schinese_censored = "stupid boy bitch",
        //         english = "stupid boy bitch"
        //     },
        //     new TableSoItemLang() {
        //         id = 872901,
        //         japanese = "please fix him",
        //         mchinese = "please fix him",
        //         schinese = "please fix him",
        //         schinese_censored = "please fix him",
        //         english = "please fix him"
        //     },
        //     new TableSoItemLang() {
        //         id = 873901,
        //         japanese = "get a boy",
        //         mchinese = "get a boy",
        //         schinese = "get a boy",
        //         schinese_censored = "get a boy",
        //         english = "get a boy"
        //     },
        //     new TableSoItemLang() {
        //         id = 871902,
        //         japanese = "cutie girl patootie",
        //         mchinese = "cutie girl patootie",
        //         schinese = "cutie girl patootie",
        //         schinese_censored = "cutie girl patootie",
        //         english = "cutie girl patootie"
        //     },
        //     new TableSoItemLang() {
        //         id = 872902,
        //         japanese = "shes so yuri",
        //         mchinese = "shes so yuri",
        //         schinese = "shes so yuri",
        //         schinese_censored = "shes so yuri",
        //         english = "shes so yuri"
        //     },
        //     new TableSoItemLang() {
        //         id = 873902,
        //         japanese = "have a good one",
        //         mchinese = "have a good one",
        //         schinese = "have a good one",
        //         schinese_censored = "have a good one",
        //         english = "have a good one"
        //     }
        // };

        // public static TableSOItemPlayerBuff[] newBuffs = new TableSOItemPlayerBuff[] {
        //     new TableSOItemPlayerBuff() {
        //         id = 1901,
        //         flagType = 0,
        //         belongType = 0,
        //         belongID = 0,
        //         gainChatID = 0,
        //         loseChatID = 0,
        //         descGain_id = 0,
        //         descLose_id = 0,
        //         descRound_id = 0,
        //         descCalculate_id = 0,
        //         strFuncGain = "DelTreasure:19001",
        //         strFuncLose = "ConsumeTreasure:19001",
        //         strFuncRound = "",
        //         strFuncCalculate = "",
        //         useTimes = 999999,
        //         durationRound = 48,
        //         validDialog_id = 0
        //     },
        //     new TableSOItemPlayerBuff() {
        //         id = 1902,
        //         flagType = 0,
        //         belongType = 0,
        //         belongID = 0,
        //         gainChatID = 0,
        //         loseChatID = 0,
        //         descGain_id = 0,
        //         descLose_id = 0,
        //         descRound_id = 0,
        //         descCalculate_id = 0,
        //         strFuncGain = "DelTreasure:19002",
        //         strFuncLose = "ConsumeTreasure:19002",
        //         strFuncRound = "",
        //         strFuncCalculate = "",
        //         useTimes = 999999,
        //         durationRound = 48,
        //         validDialog_id = 0
        //     }
        // };

        // public static TableSoItemTreasure[] newTreasure = new TableSoItemTreasure[] {
        //     new TableSoItemTreasure() {
        //         id = 19001,
        //         type = 1, // Unused?
        //         consumable = 1,
        //         name_id = 871901,
        //         icon = "treasure_4",
        //         desc_id = 872901,
        //         effectDesc_id = 873901,
        //         strBuffs = "1901",
        //         cost = 100
        //     },
        //     new TableSoItemTreasure() {
        //         id = 19002,
        //         type = 1, // Unused?
        //         consumable = 1,
        //         name_id = 871902,
        //         icon = "treasure_4",
        //         desc_id = 872902,
        //         effectDesc_id = 873902,
        //         strBuffs = "1902",
        //         cost = 100
        //     }
        // };
    }

    [HarmonyPatch(typeof(TranslateMgr))]
    [HarmonyPatch(nameof(TranslateMgr.LoadTables))]
    class BannedWordsPatch {
        static void Postfix(ref string[] ___m_banWords) {
            Plugin.PatchLogger.LogInfo("Blocking banned words");
            ___m_banWords = new string[0];
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
                        Plugin.PatchLogger.LogWarning($"Aborting stat buffs at {familyID} (goal: {addTo-1})");
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
                Plugin.PatchLogger.LogError("Failed to apply stat buffs");
                Plugin.PatchLogger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(NewRecord))]
    [HarmonyPatch("GetSexWhenNewBirth")]
    class ForceGender {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        const int VK_B = 0x42;
        const int VK_G = 0x47;

        static void Postfix(ref int __result) {
            if ((GetAsyncKeyState(VK_B) & 0x8000) != 0) {
                Plugin.PatchLogger.LogInfo($"Be boy");
                __result = 1;
            }
            else if ((GetAsyncKeyState(VK_G) & 0x8000) != 0) {
                Plugin.PatchLogger.LogInfo($"Be girl");
                __result = 2;
            }
        }
    }

    // [HarmonyPatch(typeof(Table<TableLang, TableSOLang, TableSoItemLang>))]
    // [HarmonyPatch(MethodType.Constructor, new Type[]{ typeof(TableSOLang) })]
    // class AddGenderHeirloomsText {
    //     static void Prefix(ref TableSOLang _data) {
    //         try {
    //             if (_data == null || _data.GetType() != typeof(TableSOLang)) {
    //                 Plugin.PatchLogger.LogWarning($"Lang skipping {_data} {_data?.GetType().Name ?? "null"}");
    //                 return;
    //             }
    //             Plugin.PatchLogger.LogInfo($"Adding {Plugin.newLangs.Length} language items");
    //             foreach (var newItem in Plugin.newLangs) { newItem.OnParse(); }
    //             TableSoItemLang[] joinedItems = new TableSoItemLang[Plugin.newLangs.Length + (_data.Items?.Length ?? 0)];
    //             Plugin.newLangs.CopyTo(joinedItems, 0);
    //             _data.Items?.CopyTo(joinedItems, Plugin.newLangs.Length);

    //             TableSOLang newResult = ScriptableObject.CreateInstance<TableSOLang>();
    //             newResult.Items = joinedItems;
    //             _data = newResult;
    //         } catch (Exception e) {
    //             Plugin.PatchLogger.LogError("Lang crash");
    //             Plugin.PatchLogger.LogError(e);
    //         }
    //     }
    // }

    // [HarmonyPatch(typeof(Table<TableLang, TableSOLang, TableSoItemLang>))]
    // [HarmonyPatch(MethodType.Constructor, new Type[]{ typeof(TableSOLang) })]
    // class AddGenerHeirloomsBuffs {
    //     static void Prefix(ref TableSOPlayerBuff _data) {
    //         try {
    //             if (_data == null || _data.GetType() != typeof(TableSOPlayerBuff)) {
    //                 Plugin.PatchLogger.LogWarning($"Buff skipping");
    //                 return;
    //             }
    //             Plugin.PatchLogger.LogInfo($"Adding {Plugin.newBuffs.Length} buff items");
    //             foreach (var newItem in Plugin.newBuffs) { newItem.OnParse(); }
    //             TableSOItemPlayerBuff[] joinedItems = new TableSOItemPlayerBuff[Plugin.newBuffs.Length + (_data.Items?.Length ?? 0)];
    //             Plugin.newBuffs.CopyTo(joinedItems, 0);

    //             TableSOPlayerBuff newResult = ScriptableObject.CreateInstance<TableSOPlayerBuff>();
    //             newResult.Items = joinedItems;
    //             _data = newResult;
    //         } catch (Exception e) {
    //             Plugin.PatchLogger.LogError("Buff crash");
    //             Plugin.PatchLogger.LogError(e);
    //         }
    //     }
    // }

    // [HarmonyPatch]
    // class AddGenderHeirlooms {
    //     static System.Reflection.MethodBase TargetMethod() {
    //         // refer to C# reflection documentation:
    //         return typeof(ResMgr).GetMethod(nameof(ABMgr.LoadRes)).MakeGenericMethod(typeof(TableSOTreasure));
    //     }

    //     static void Postfix(TableSOTreasure __result) {
    //         try {
    //             if (__result == null || __result.GetType() != typeof(TableSOTreasure)) {
    //                 Plugin.PatchLogger.LogWarning($"Treasure skipping");
    //                 return;
    //             }
    //             Plugin.PatchLogger.LogInfo($"Adding {Plugin.newTreasure.Length} treasure items");
    //             foreach (var newItem in Plugin.newTreasure) { newItem.OnParse(); }
    //             TableSoItemTreasure[] joinedItems = new TableSoItemTreasure[Plugin.newTreasure.Length + (__result.Items?.Length ?? 0)];
    //             Plugin.newTreasure.CopyTo(joinedItems, 0);
    //             __result.Items?.CopyTo(joinedItems, Plugin.newTreasure.Length);
    //             __result.Items = joinedItems;
    //         } catch (Exception e) {
    //             Plugin.PatchLogger.LogError("Treasure crash");
    //             Plugin.PatchLogger.LogError(e);
    //         }
    //     }
    // }
}
