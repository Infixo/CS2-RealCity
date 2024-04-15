using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using RealCity.Config;

namespace RealCity;

[FileLocation(nameof(RealCity))]
[SettingsUIGroupOrder(kToggleGroup, kButtonGroup)]
[SettingsUIShowGroupName(kToggleGroup, kButtonGroup)]
public class Setting : ModSetting
{
    public const string kSection = "Main";

    public const string kToggleGroup = "Options";
    public const string kButtonGroup = "Actions";

    public Setting(IMod mod) : base(mod)
    {
        SetDefaults();
    }

    /// <summary>
    /// Gets or sets a value indicating whether: Used to force saving of Modsettings if settings would result in empty Json.
    /// </summary>
    [SettingsUIHidden]
    public bool _Hidden { get; set; }

    //Logging = base.Config.Bind<bool>("Debug", "Logging", false, "Enables detailed logging.");
    //ConfigDump = base.Config.Bind<bool>("Debug", "ConfigDump", false, "Saves configuration to a secondary xml file.");
    [SettingsUISection(kSection, kToggleGroup)]
    public bool Logging { get; set; }

    [SettingsUISection(kSection, kToggleGroup)]
    public bool UseLocalConfig { get; set; }

    [SettingsUIButton]
    [SettingsUIConfirmation]
    [SettingsUISection(kSection, kButtonGroup)]
    public bool ApplyConfiguration { set { Mod.log.Info("ApplyConfiguration clicked"); ConfigTool.ReadAndApply(); } }

    public override void SetDefaults()
    {
        _Hidden = true;
        Logging = false;
        UseLocalConfig = false;
    }
}

public class LocaleEN : IDictionarySource
{
    private readonly Setting m_Setting;
    public LocaleEN(Setting setting)
    {
        m_Setting = setting;
    }

    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
    {
        return new Dictionary<string, string>
        {
            { m_Setting.GetSettingsLocaleID(), $"City Services Rebalance {Mod.modAsset.version}" },
            { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

            { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Options" },
            { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Actions" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Logging)), "Detailed logging" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.Logging)), "Outputs more diagnostic information to the log file." },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseLocalConfig)), "Use local configuration" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseLocalConfig)), "Local configuration will be used instead of the default one that is shipped with the mod." },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyConfiguration)), "Apply Configuration" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyConfiguration)), "This will apply a new configuration from Confix.xml file." },
            { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ApplyConfiguration)), "This will apply a new configuration. Please confirm." },
        };
    }

    public void Unload()
    {
    }
}
