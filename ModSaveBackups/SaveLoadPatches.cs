using BepInEx;
using HarmonyLib;

namespace ModSaveBackups
{
    internal class SaveLoadPatches
    {
        private static int backupIndex;

        [HarmonyPatch(typeof(ModSave))]
        private class ModSavePatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("Save")]
            public static void SavePatch(PluginInfo pluginInfo)
            {
                ModSaveSlots.PushBackups(pluginInfo);
                backupIndex = 0;
            }

            [HarmonyPrefix]
            [HarmonyPatch("GetSaveModFile")]
            public static bool GetSaveModFilePatch(ref string __result, int slot, PluginInfo pluginInfo)
            {
                if (backupIndex == 0) return true;

                __result = ModSaveSlots.GetBackupSaveModFile(pluginInfo, slot, backupIndex);
                return false;
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager))]
        private class SaveLoadManagerPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void AwakePatch()
            {
                backupIndex = 0;
            }

            [HarmonyPrefix]
            [HarmonyPatch("LoadGame")]
            public static void LoadGamePatch(int backupIndex)
            {
                SaveLoadPatches.backupIndex = backupIndex;
            }
        }
    }
}
