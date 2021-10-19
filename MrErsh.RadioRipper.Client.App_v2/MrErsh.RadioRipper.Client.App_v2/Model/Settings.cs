using System;

namespace MrErsh.RadioRipper.Client.Model
{
    [Serializable]
    public sealed class Settings
    {
        public string ServerUrl { get; set; }
    }

    //public class SettingsService
    //{
    //    private static 

    //    public Settings Get()
    //    {
    //        ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
    //        ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values["RoamingFontInfo"];
    //        if (composite != null)
    //        {
    //            String fontName = composite["Font"] as string;
    //            int fontSize = (int)composite["FontSize"];
    //        }
    //    }

    //    public Sa
    //}
}
