using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using FlockObjects;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace FlockProcess
{
    /// <summary>
    /// Sets a new wallpaper
    /// </summary>
    internal class WallpaperSetter
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        /// <summary>
        /// Runs the set method to put a new wallpaper on the desktop
        /// </summary>
        /// <param name="uri">The URI to the new wallpaper</param>
        /// <param name="style">The style of wallpaper to apply</param>
        public static void Set(string uri, WallpaperStyle style)
        {
            Console.WriteLine("WS: START Wallpaper set");

            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");

            using (var stream = new MemoryStream(File.ReadAllBytes(uri)))
            using (var image = Image.FromStream(stream, false, true))
            {
                image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }


            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == WallpaperStyle.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == WallpaperStyle.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == WallpaperStyle.Span)
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == WallpaperStyle.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == WallpaperStyle.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == WallpaperStyle.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

            Console.WriteLine("WS: END Wallpaper set");
        }

        /// <summary>
        /// Sets the author's name to the new text file
        /// </summary>
        /// <param name="authorName">The author's name</param>
        public static void UpdateAuthor(Wallpaper wallpaper)
        {
            var jsonObject = JsonConvert.SerializeObject(wallpaper);
            File.WriteAllText(Config.AuthorTextFile, jsonObject);
        }
    }
}
