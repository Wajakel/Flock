using System;
using System.Configuration;
using System.Diagnostics;

namespace FlockProcess
{
    /// <summary>
    /// Class to handle app.config values
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The base URL for all the API calls
        /// </summary>
        public static string BaseURL
        {
            get
            {
                var baseURL = ConfigurationManager.AppSettings["baseURL"];
                if (baseURL == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return baseURL;
            }
        }

        /// <summary>
        /// The URL parameter for getting random photos
        /// </summary>
        public static string RandomPhotosURL
        {
            get
            {
                var randomURL = ConfigurationManager.AppSettings["randomPhotosURL"];
                if (randomURL == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return randomURL;
            }
        }

        /// <summary>
        /// The application auth key for the API
        /// </summary>
        public static string AuthKey
        {
            get
            {
                var secretKey = ConfigurationManager.AppSettings["secretKey"];
                if (secretKey == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return secretKey;
            }
        }

        /// <summary>
        /// The local cache location (where wallpapers are downloaded)
        /// </summary>
        public static string LocalCacheLocation
        {
            get
            {
                var cacheLocation = ConfigurationManager.AppSettings["localCacheLocation"];
                if (cacheLocation == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return cacheLocation;
            }
        }

        /// <summary>
        /// The number of items to store in the cache
        /// </summary>
        public static int LocalCacheSize
        {
            get
            {
                var cacheSize = ConfigurationManager.AppSettings["localCacheSize"];
                if (cacheSize == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return int.Parse(cacheSize);
            }
        }

        /// <summary>
        /// Represents whether or not the application automatically starts up when Windows does
        /// </summary>
        public static bool AutomaticStartup
        {
            get
            {
                var openOnStartup = ConfigurationManager.AppSettings["openOnStartup"];
                if (openOnStartup == null)
                {
                    throw new SettingsPropertyNotFoundException();
                }

                return bool.Parse(openOnStartup);
            }

            set
            {
                ConfigurationManager.AppSettings["openOnStartup"] = (!AutomaticStartup).ToString();
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).Save();
            }
        }

        /// <summary>
        /// The name of the current serialised wallpaper
        /// </summary>
        public static string AuthorTextFile
        {
            get
            {
                return string.Format("{0}{1}", Environment.CurrentDirectory, "author.json");
            }
        }

        public static string UnsplashHomeURL
        {
            get
            {
                return "https://unsplash.com/";
            }
        }

        public static string UnsplashReferral
        {
            get
            {
                return "?utm_source=flock&utm_medium=referral&utm_campaign=api-credit";
            }
        }

        /// <summary>
        /// The application version
        /// </summary>
        public static string ApplicationVersion
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            }
        }
    }
}
