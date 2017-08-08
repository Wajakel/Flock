using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using FlockObjects;
using Newtonsoft.Json;

namespace FlockProcess
{
    /// <summary>
    /// The main wallpaper retriever process
    /// </summary>
    public static class WallpaperProcess
    {
        /// <summary>
        /// Run the wallpaper retriever process
        /// </summary>
        public static void Run()
        {
            var newWallpaper = WallpaperRetriever.GetFromRemote();
            if (newWallpaper == null) throw new Exception("Failed to connect to the wallpaper service. Please check your internet connectivity or try again later.");
            WallpaperSetter.Set(newWallpaper.LocalLocation, WallpaperStyle.Fill);
            WallpaperSetter.UpdateAuthor(newWallpaper);
            CacheHousekeeper.Run();
        }

        /// <summary>
        /// Checks to see if the current desktop image was set by Flock
        /// </summary>
        /// <returns>True if it was set by Flock, false otherwise</returns>
        public static bool IsDesktopSetByFlock()
        {
            try
            {
                var currentDesktopPath = WallpaperRetriever.GetCurrentDesktop();
                Wallpaper currentSavedWallpaper = JsonConvert.DeserializeObject<Wallpaper>(File.ReadAllText(Config.AuthorTextFile));

                var currentDesktopHash = GetHash(new Bitmap(@currentDesktopPath));
                var currentSavedHash = GetHash(new Bitmap(@currentSavedWallpaper.LocalLocation));

                int equalElements = currentDesktopHash.Zip(currentSavedHash, (i, j) => i == j).Count(eq => eq);

                return (equalElements > 250);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the author of the current desktop wallpaper
        /// </summary>
        /// <returns>The author name</returns>
        public static string GetAuthorFromSavedFile()
        {
            Wallpaper currentSavedWallpaper = JsonConvert.DeserializeObject<Wallpaper>(File.ReadAllText(Config.AuthorTextFile));
            return currentSavedWallpaper.Author;
        }

        /// <summary>
        /// Gets the author URL of the current desktop wallpaper
        /// </summary>
        /// <returns>The author name</returns>
        public static string GetAuthorURLFromSavedFile()
        {
            Wallpaper currentSavedWallpaper = JsonConvert.DeserializeObject<Wallpaper>(File.ReadAllText(Config.AuthorTextFile));
            return currentSavedWallpaper.AuthorURL;
        }

        /// <summary>
        /// Creates a hash of an image for comparison.
        /// From: http://stackoverflow.com/a/35153895
        /// </summary>
        /// <param name="bmpSource">The bitmap source</param>
        /// <returns>A list of bits</returns>
        private static List<bool> GetHash(Bitmap bmpSource)
        {
            var lResult = new List<bool>();
            //create new image with 16x16 pixel
            var bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }
    }
}
