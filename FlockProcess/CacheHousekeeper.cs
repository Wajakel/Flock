using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FlockProcess
{
    /// <summary>
    /// The local cache housekeeping service. Deletes old files and duplicates to keep it from getting too large
    /// </summary>
    internal static class CacheHousekeeper
    {
        /// <summary>
        /// Run the housekeeping service
        /// </summary>
        public static void Run()
        {
            try
            {
            int deletedFiles = 0;
            Console.WriteLine("HK: Starting housekeeping");
            var directory = new DirectoryInfo(Config.LocalCacheLocation);

            var duplicates = directory.GetFiles().GroupBy(f => CalculateHash(f)).Where(g => g.Count() > 1);
            var duplicatesCount = duplicates.Count();

            Console.WriteLine("HK: START Deleting {0} duplicates", duplicatesCount);

            foreach (var dupe in duplicates)
            {
                Console.WriteLine("Deleting duplicate with md5: {0} ({1})", dupe.Key, dupe.OrderByDescending(f => f.CreationTimeUtc).First().FullName);
                dupe.OrderByDescending(x => x.CreationTimeUtc).First().Delete();
                deletedFiles ++;
            }

            Console.WriteLine("HK: END Deleting {0} duplicates", duplicatesCount);

            Console.WriteLine("HK: START Deleting old files");

            while (directory.GetFiles().Count() > Config.LocalCacheSize)
            { 
                var oldest = directory.GetFiles().OrderBy(f => f.CreationTimeUtc).First();
                Console.WriteLine("Deleting {0}", oldest.FullName);
                oldest.Delete();
                deletedFiles++;
            };

            Console.WriteLine("HK: END Deleting old files");

            Console.WriteLine("HK: Finished housekeeping. Deleted {0} files", deletedFiles);
            }
            catch (Exception e)
            {
                throw new FileLoadException();
            }
        }

        /// <summary>
        /// Calculate the MD5 hash of a file contents to verify duplicates
        /// </summary>
        /// <param name="file">The file to be checked</param>
        /// <returns>The MD5 as a string</returns>
        private static string CalculateHash(FileInfo file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }
    }
}
