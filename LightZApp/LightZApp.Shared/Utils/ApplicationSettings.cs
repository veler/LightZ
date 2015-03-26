namespace LightZApp.Utils
{
    using Newtonsoft.Json;

    internal static class ApplicationSettings
    {
        /// <summary>
        /// Define a roaming setting
        /// </summary>
        /// <typeparam name="T">the type of the data</typeparam>
        /// <param name="key">the name of the setting</param>
        /// <param name="value">the data</param>
        internal static void SetSetting<T>(string key, T value)
        {
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values[key] = JsonConvert.SerializeObject(value, Formatting.None);
        }

        /// <summary>
        /// Retrieve a roaming setting
        /// </summary>
        /// <typeparam name="T">the type of the data</typeparam>
        /// <param name="key">the name of the setting</param>
        /// <returns>the data</returns>
        internal static T GetSetting<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(Windows.Storage.ApplicationData.Current.RoamingSettings.Values[key].ToString());
        }
    }
}
