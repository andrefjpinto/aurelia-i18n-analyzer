using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace aureliai18nanalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string allFilesContent = "";
            string directoryToBeSearched = args[0];
            string translationFilesPath = args[1];

            Console.WriteLine("Directory to be searched (absolute path): "+ directoryToBeSearched);
            if (AuxMethods.DirectoryPathValidator(directoryToBeSearched))
            {   
                Console.WriteLine("Translation files location (absolute path): " + translationFilesPath);

                if (!AuxMethods.DirectoryPathValidator(translationFilesPath))
                {
                    Environment.Exit(267); //ERROR_DIRECTORY The directory name is invalid.
                }
            }
            else
            {
                Environment.Exit(267); //ERROR_DIRECTORY The directory name is invalid.
            }

            Console.WriteLine("File types to be searched: .ts .html");
            Console.WriteLine("Language files types: .json");

            Console.WriteLine();
            Console.WriteLine();

            List <string> resourcesExtensions = new List<string>();
            resourcesExtensions.Add("json");

            List<string> sourceExtensions = new List<string>();
            sourceExtensions.Add("ts");
            sourceExtensions.Add("html");

            List<string> resourceFiles = AuxMethods.ProcessPathFiles(translationFilesPath, resourcesExtensions);
            List<string> sourceFiles = AuxMethods.ProcessPathFiles(directoryToBeSearched, resourcesExtensions);

            foreach (var item in sourceFiles)
            {
                try {
                    using (StreamReader sr = new StreamReader(@item))
                    {
                        String line = sr.ReadToEnd();
                        allFilesContent += line.Replace(" ", "");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            List<Resource> resources = new List<Resource>();

            foreach (var file in resourceFiles)
            {
                List<Locale> locales = new List<Locale>();

                using (StreamReader asd = File.OpenText(@file.ToString()))
                using (JsonTextReader reader = new JsonTextReader(asd))
                {
                    JObject locale = (JObject)JToken.ReadFrom(reader);

                    foreach (var item in locale)
                    {
                        locales.AddRange(AuxMethods.GetJsonPaths(item.Value));
                    }
                }

                resources.Add(new Resource { Path = file, Locales = locales });
            }

            var total = 0;
            foreach (var resource in resources)
            {
                var totalPerLangFile = 0;
                ConsoleExtension.WriteSuccessLine($"Processing {resource.Path}");

                foreach (var item in resource.Locales)
                {
                    if (allFilesContent
                        .Contains("\"" + item.LocalePath + "\"") || 
                        allFilesContent.Contains("\"" + item.LocalePath + "|") || 
                        allFilesContent.Contains("'" + item.LocalePath + "'") || 
                        allFilesContent.Contains("'" + item.LocalePath + "|") ||
                        allFilesContent.Contains("]" + item.LocalePath + "|") ||
                        allFilesContent.Contains("]" + item.LocalePath + "\"") ||
                        allFilesContent.Contains("]" + item.LocalePath + "'")
                       )
                    {
                        item.Active = true;
                    }
                    else
                    {
                        totalPerLangFile++;
                        Console.WriteLine(item.LocalePath);
                    }
                }
                ConsoleExtension.WriteSuccessLine($"Finished. Total unused on this language file: {totalPerLangFile}");
                total += totalPerLangFile;
                Console.WriteLine();
            }

            sw.Stop();
            Console.WriteLine();
            ConsoleExtension.WriteSuccessLine($"Total unused: {total}");
            Console.WriteLine("Elapsed = {0}", sw.Elapsed);
            Console.ReadLine();
        }
    }
}