using System.IO;
using BepInEx;

namespace ModSaveBackups
{
    internal class ModSaveSlots
    {
        public static void PushBackups(PluginInfo pluginInfo)
        {
            int num = 5;
            string backupPath = GetBackupPath(pluginInfo, SaveSlots.currentSlot, num);
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            for (int num2 = num - 1; num2 > 0; num2--)
            {
                string backupPath2 = GetBackupPath(pluginInfo,SaveSlots.currentSlot, num2);
                if (File.Exists(backupPath2))
                {
                    File.Move(backupPath2, GetBackupPath(pluginInfo, SaveSlots.currentSlot, num2 + 1));
                }
            }

            string currentSavePath = GetCurrentSaveModFile(pluginInfo);
            if (File.Exists(currentSavePath))
            {
                File.Move(currentSavePath, GetBackupPath(pluginInfo, SaveSlots.currentSlot, 1));
            }
        }

        private static string GetBackupPath(PluginInfo pluginInfo, int slot, int backupIndex)
        {
            return Path.Combine(ModSave.GetSaveDirectory(slot), pluginInfo.Metadata.GUID  + "_backup" + backupIndex + ".save");            
        }

        public static string GetCurrentSaveModFile(PluginInfo pluginInfo)
        {
            return Path.Combine(ModSave.GetSaveDirectory(SaveSlots.currentSlot), pluginInfo.Metadata.GUID + ".save");
        }

        public static string GetBackupSaveModFile(PluginInfo pluginInfo, int slot, int backupIndex)
        {
            for (int i = backupIndex; i > 0; i--)
            {
                var backupFilePath = GetBackupPath(pluginInfo, slot, i);
                if (File.Exists(backupFilePath))
                {
                    Plugin.logger.LogDebug($"Loading SaveMod backup file path: {backupFilePath}");
                    return backupFilePath;
                }                    
            }
            Plugin.logger.LogDebug($"Loading current {pluginInfo} SaveMod file");
            return GetCurrentSaveModFile(pluginInfo);
        }
    }
}
