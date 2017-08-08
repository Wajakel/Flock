namespace FlockObjects
{
    public class Wallpaper
    {
        /// <summary>
        /// The height of the wallpaper
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The width of the wallpaper
        /// </summary>
        public int Width { get; set;  }

        /// <summary>
        /// The remote wallpaper location
        /// </summary>
        public string RemoteLocation { get; set;  }


        /// <summary>
        /// The local wallpaper location (download)
        /// </summary>
        public string LocalLocation { get; set; }

        /// <summary>
        /// The photo's author
        /// </summary>
        public string Author { get; set; }

        public string AuthorURL { get; set; }
    }
}
