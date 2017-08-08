using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using FlockObjects;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace FlockProcess
{
    internal static class WallpaperRetriever
    {
        /// <summary>
        /// Calls the API and retrieves a photo
        /// </summary>
        /// <returns>The wallpaper object</returns>
        public static Wallpaper GetFromRemote()
        {
            var randomWallpaperURL = string.Format("{0}{1}", Config.BaseURL, Config.RandomPhotosURL);
            Wallpaper newWallpaper = null;
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            try
            {
                // We want to keep retrieving photos until we get one that's big enough for our screen size
                // TODO: Refactor this to make one API call
                do
                {
                    Console.WriteLine("WR: START Wallpaper API call");
                    // Call to the API and grab a new photo
                    var responseObject = FetchPhotos(randomWallpaperURL);
                    Console.WriteLine("WR: END Wallpaper API call");

                    Console.WriteLine("WR: START JSON parse");
                    var parsedObject = JObject.Parse(responseObject);
                    Console.WriteLine("WR: END JSON parse");

                    newWallpaper = ConvertJsonToWallpaper(parsedObject);
                } while (newWallpaper.Height < screenHeight || newWallpaper.Width < screenWidth);

                Console.WriteLine("WR: START Wallpaper download");
                // Download the photo
                DownloadPhoto(newWallpaper);
                Console.WriteLine("WR: END Wallpaper download");

            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }

            return newWallpaper;
        }

        public static string GetCurrentDesktop()
        {
            var wpReg = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            var wallpaperPath = wpReg.GetValue("WallPaper").ToString();
            wpReg.Close();
            return wallpaperPath;
        }

        /// <summary>
        /// Make the web request to the API 
        /// </summary>
        /// <returns></returns>
        private static string FetchPhotos(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Accept-Version", "v1");
            request.Headers.Add("Authorization", string.Format("Client-ID {0}", Config.AuthKey));
            request.Headers.Add("orientation", "landscape");

            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                var errorResponse = e.Response;
                throw;
            }
        }

        /// <summary>
        /// Converts the API json to a wallpaper object
        /// </summary>
        /// <param name="parsed">The parsed JSON</param>
        /// <returns>The wallpaper object</returns>
        private static Wallpaper ConvertJsonToWallpaper(JObject parsed)
        {
            var downloadLocation = string.Format("{0}{1}.jpg", Config.LocalCacheLocation, Guid.NewGuid());

            return new Wallpaper()
            {
                Width = int.Parse(parsed["width"].ToString()),
                Height = int.Parse(parsed["height"].ToString()),
                RemoteLocation = parsed["urls"]["full"].ToString(),
                LocalLocation = downloadLocation,
                Author = parsed["user"]["name"].ToString(),
                AuthorURL = parsed["user"]["links"]["html"].ToString()
            };
        }

        /// <summary>
        /// Download the actual wallpaper image file
        /// </summary>
        /// <param name="wallpaper">The wallpaper object</param>
        private static void DownloadPhoto(Wallpaper wallpaper)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(wallpaper.RemoteLocation, wallpaper.LocalLocation);
            }
        }
    }
}
