using System.Reflection;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;

namespace ModSaveBackups
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude82.modsavebackups";
        public const string PLUGIN_NAME = "ModSaveBackups";
        public const string PLUGIN_VERSION = "1.1.0";

        internal static Plugin instance;
        internal static ManualLogSource logger;

        private void Awake()
        {
            instance = this;
            logger = Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);
        }
    }
}
