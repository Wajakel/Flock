using System;
using System.Threading;
using FlockProcess;

namespace FlockDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var thread = new Thread(WallpaperProcess.Run);

            Console.WriteLine("Press enter to set new wallpaper");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                thread.Start();
            }
        }
    }
}
