using BepInEx;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ModSaveBackups
{
    public static class ModSave
    {
        public static void Save(PluginInfo pluginInfo, object data)
        {
            if (!GameState.playing) return;

            Directory.CreateDirectory(GetSaveDirectory(SaveSlots.currentSlot));

            FileStream stream = File.Create(GetSaveModFile(SaveSlots.currentSlot, pluginInfo));
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (Exception ex)
            {
                Plugin.logger.LogError($"Could not serialize data '{pluginInfo.Metadata.GUID}'");
                Plugin.logger.LogError($"{pluginInfo.Metadata.GUID}: {ex.Message}");
            }
            stream.Close();
        }

        public static bool Load(PluginInfo pluginInfo, out object loadedObject)
        {
            loadedObject = null;
            if (!GameState.playing && !GameState.currentlyLoading) return false;
            
            if (!File.Exists(GetSaveModFile(SaveSlots.currentSlot, pluginInfo)))
            {
                Plugin.logger.LogError($"Could not find mod save file for '{pluginInfo.Metadata.GUID}'");
                return false;
            }

            FileStream stream = File.OpenRead(GetSaveModFile(SaveSlots.currentSlot, pluginInfo));

            if (stream.Length <= 0)
            {
                stream.Close();
                Plugin.logger.LogError($"File stream length is 0 '{pluginInfo.Metadata.GUID}'");
                return false;
            }

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object value = formatter.Deserialize(stream);
                stream.Close();
                loadedObject = value;
                return true;
            }
            catch (Exception ex)
            {
                stream.Close();
                Plugin.logger.LogError($"Could not deserialize mod save for '{pluginInfo.Metadata.GUID}'");
                Plugin.logger.LogError($"{pluginInfo.Metadata.GUID}: {ex.Message}");
                return false;
            }
        }

        public static bool Load<T>(PluginInfo pluginInfo, out T loadedObject)
        {
            loadedObject = default;
            var result = Load(pluginInfo, out var obj);
            if (result)
                loadedObject = (T)obj;
            return result;
        }

        internal static string GetSaveModFile(int slot, PluginInfo pluginInfo)
        {
            return Path.Combine(GetSaveDirectory(slot), $"{pluginInfo.Metadata.GUID}.save");
        }

        public static string GetSaveDirectory(int slot)
        {
            return Path.Combine(Application.persistentDataPath, $"slot{slot}");
        }
    }
}