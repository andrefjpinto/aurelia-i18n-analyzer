using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace aureliai18nanalyzer
{
    public static class AuxMethods
    {
        public static List<string> ProcessPathFiles(string path, List<string> extensions)
        {
            List<string> paths = new List<string>();

            if (File.Exists(path))
            {
                if (extensions.Contains(Path.GetExtension(path)))
                {
                    paths.Add(path);
                }
            }
            else if (Directory.Exists(path))
            {
                paths.AddRange(ProcessDirectory(path));
            }

            return paths;
        }

        private static List<string> ProcessDirectory(string targetDirectory)
        {
            List<string> paths = new List<string>();

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                paths.Add(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                paths.AddRange(ProcessDirectory(subdirectory));

            return paths;
        }

        public static List<Locale> GetJsonPaths(JToken subLocale) {
            List<Locale> resources = new List<Locale>();

            if (subLocale.HasValues == false)
            {
                resources.Add(new Locale { LocalePath = subLocale.Path } );
                return resources;
            }
            else {
                foreach (var itemdf in subLocale)
                {
                    var result = GetJsonPaths(itemdf);
                    resources.AddRange(result);
                }

                return resources;
            }
        }

        public static bool DirectoryPathValidator(string path)
        {
            if (!Directory.Exists(path))
            {
                ConsoleExtension.WriteErrorLine("The path specified either doesn't exist or isn't valid. Please confirm that the path is an absolute path to a directory.");
                return false;
            }

            if (!Path.IsPathRooted(path))
            {
                ConsoleExtension.WriteErrorLine("The path specified isn't an absolute path. Please confirm that the path is an absolute path to a directory.");
                return false;
            }

            return true;
        }
    }
}
