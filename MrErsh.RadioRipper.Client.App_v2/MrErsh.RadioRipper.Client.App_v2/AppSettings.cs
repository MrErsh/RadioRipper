using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace MrErsh.RadioRipper.Client
{
    public class AppSettings : IAppSettings
    {
        #region Properties

        public string Host
        {
            get => Get<string>();
            set => Set(value);
        }

        public string ActualApiHost => Host ?? Shared.SharedConstants.DEFAULT_HOST;

        #endregion

        #region Private methods

        private static T Get<T>([CallerMemberName] string prop = default)
        {
            try
            {
                return (T)ApplicationData.Current.LocalSettings.Values[prop];
            }
            catch
            {
                return default;
            }
        }

        private static void Set(object value, [CallerMemberName, NotNull] string prop = default)
            => ApplicationData.Current.LocalSettings.Values[prop] = value;

        #endregion
    }
}
