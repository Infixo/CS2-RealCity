using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using System.Reflection;
using System;
using RealCity.Config;

namespace RealCity;

public class Mod : IMod
{
    // mod's instance and asset
    public static Mod instance { get; private set; }
    public static ExecutableAsset modAsset { get; private set; }

    // logging
    public static ILog log = LogManager.GetLogger($"{nameof(RealCity)}").SetShowsErrorsInUI(false);

    public static void Log(string text) => log.Info(text);

    public static void LogIf(string text)
    {
        if (setting.Logging) log.Info(text);
    }

    public static Setting setting { get; private set; }

    public void OnLoad(UpdateSystem updateSystem)
    {
        instance = this;

        log.Info(nameof(OnLoad));

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
        {
            log.Info($"{asset.name} mod asset at {asset.path}");
            modAsset = asset;
            //DumpObjectData(asset);
        }

        setting = new Setting(this);
        setting.RegisterInOptionsUI();
        GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(setting));

        AssetDatabase.global.LoadSettings(nameof(RealCity), setting, new Setting(this));

        // READ CONFIG DATA
        ConfigToolXml.LoadConfig(asset.path);

        // APPLY CONFIG
        ConfigTool.Apply();
    }

    public void OnDispose()
    {
        log.Info(nameof(OnDispose));
        if (setting != null)
        {
            setting.UnregisterInOptionsUI();
            setting = null;
        }
    }

    public static void DumpObjectData(object objectToDump)
    {
        //string className = objectToDump.GetType().Name;
        //Mod.log.Info($"{prefab.name}.{objectToDump.name}.CLASS: {className}");
        Mod.log.Info($"Object: {objectToDump}");

        // Fields
        Type type = objectToDump.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            //if (field.Name != "isDirty" && field.Name != "active" && field.Name != "components")
            Mod.log.Info($" {field.Name}: {field.GetValue(objectToDump)}");
        }

        // Properties
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            Mod.log.Info($" {property.Name}: {property.GetValue(objectToDump)}");
        }
    }

}
