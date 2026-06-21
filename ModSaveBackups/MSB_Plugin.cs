using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace ModSaveBackups
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(PORTABLE_SAVES_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class MSB_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude.modsavebackups";
        public const string PLUGIN_NAME = "ModSaveBackups";
        public const string PLUGIN_VERSION = "1.2.0";

        public const string PORTABLE_SAVES_GUID = "com.nandbrew.PortableSaves";
        internal static BaseUnityPlugin PortableSavesPluginInstance { get; private set; }

        internal static MSB_Plugin Instance { get; private set; }
        private static ManualLogSource _logger;

        internal static void LogDebug(string message) => _logger.LogDebug(message);
        internal static void LogInfo(string message) => _logger.LogInfo(message);
        internal static void LogWarning(string message) => _logger.LogWarning(message);
        internal static void LogError(string message) => _logger.LogError(message);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _logger = Logger;

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                if (metadata.GUID.Equals(PORTABLE_SAVES_GUID))
                {
                    LogInfo("PortableSaves mod found");
                    var portableSavesType = plugin.Value.Instance.GetType();
                    var portableSavesPath = Traverse.Create(portableSavesType)
                        .Property("PortableSavePath")
                        .GetValue<string>();
                    ModSave.SetBasePath(portableSavesPath);
                    break;
                }
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);
        }
    }
}
