# ModSaveBackups

Intended to be used mainly by modders as a dependency, but can also be used by players. 
Incorporates the ModSave class from [SailwindModdingHelper](https://github.com/AppSailwindMods/SailwindModdingHelper) by App24 so now 
this can be more thought of as ModSave with Backups.

Using this mod will allow you to create a save file for you Sailwind mod data separate from the main save file for the game. 
This mod will also create backups of that mod save file when the game makes backups of the main save file. If you load a backup 
save then the corresponding backup mod save file will be loaded.

## Usage

Make a Serializable save container class to hold the data you wish to save: 

```c#
[Serializable]
public class ExampleSaveContainer
{
    public Dictionary<string, int> exampleDict;
    public bool exampleBool;
}
```

Then make a patch to save the data on save and a patch to load the data on load:

```c#
[HarmonyPatch(typeof(SaveLoadManager))]
private class SaveLoadManagerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch("SaveModData")]
    public static void DoSaveGamePatch()
    {
        var saveContainer = new ExampleSaveContainer();
        saveContainer.exampleDict = MyModGameObject.instance.exampleDict.ToDictionary(
            entry => entry.Key,
            entry => entry.Value);

        saveContainer.exampleBool = MyModGameObject.instance.exampleBool;

        ModSave.Save(Plugin.instance.Info, saveContainer);
    }

    [HarmonyPostfix]
    [HarmonyPatch("LoadModData")]
    public static void LoadModDataPatch()
    {
        if (!ModSave.Load(Plugin.instance.Info, out ExampleSaveContainer saveContainer))
        {
            Plugin.logger.LogWarning("Save file loading failed. If this is the first time loading this save with this mod, this is normal.");
            return;
        }

        if (saveContainer.portBadges != null)
        {
            LoadDictionary(saveContainer.portBadges, PortsVisitedUI.instance.portBadges);
            foreach (KeyValuePair<string, int> item in saveContainer.exampleDict)
            {
                if (MyModGameObject.instance.exampleDict.ContainsKey(item.Key))
                {
                    MyModGameObject.instance.exampleDict[item.Key] = item.Value;
                    continue;
                }
                Plugin.logger.LogWarning($"LoadData: {item.Key} not found in game");
            }
        }

        if (saveContainer.exampleBool != null)
            MyModGameObject.instance.exampleBool = saveContainer.exampleBool
    }
}
```

### Requires
* [BepInEx 5.4.23](https://github.com/BepInEx/BepInEx/releases)

### Installation
Place the ModSaveBackups.dll into the Sailwind/BepInEx/Plugins folder.