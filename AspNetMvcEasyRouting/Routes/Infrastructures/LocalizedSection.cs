using System.Globalization;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class LocalizedSection
    {
        public const string EN_NAME = "en-US";
        public const string FR_NAME = "fr-CA";
        public static CultureInfo EN = CultureInfo.GetCultureInfo(EN_NAME);
        public static CultureInfo FR = CultureInfo.GetCultureInfo(FR_NAME);
        public CultureInfo CultureInfo { get; set; }
        public string TranslatedValue { get; set; }

        public LocalizedSection(CultureInfo culture, string translatedValue)
        {
            this.CultureInfo = culture;
            this.TranslatedValue = translatedValue;
        }

        public static string ReplaceSection(string url, LocalizedSection areaTransaction, LocalizedSection controllerTransaction, LocalizedSection actionTransaction)
        {
            if (areaTransaction != null)
            {
                url = url.Replace("{" + Constants.AREA.ToLower() + "}", areaTransaction.TranslatedValue);
            }
            if (controllerTransaction != null)
            {
                url = url.Replace("{" + Constants.CONTROLLER.ToLower() + "}", controllerTransaction.TranslatedValue);
            }
            if (actionTransaction != null)
            {
                url = url.Replace("{" + Constants.ACTION.ToLower() + "}", actionTransaction.TranslatedValue);
            }
            return url;
        }
    }
}