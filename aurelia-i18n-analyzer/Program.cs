using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using aureliai18nanalyzer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace aurelia_i18n_analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            string allFilesContent = "";

            List<string> resourcesExtensions = new List<string>();
            resourcesExtensions.Add("json");

            List<string> sourceExtensions = new List<string>();
            sourceExtensions.Add("ts");
            sourceExtensions.Add("html");
                
            List<string> resourceFiles = AuxMethods.ProcessPathFiles("/assets/locales", resourcesExtensions);
            List<string> sourceFiles = AuxMethods.ProcessPathFiles("/src", resourcesExtensions);

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
                        locales.AddRange(AuxMethods.asd(item.Value));
                    }
                }

                resources.Add(new Resource { Path = file, Locales = locales });
            }


            foreach (var resource in resources)
            {
                var total = 0;
                Console.WriteLine($"Processing {resource.Path}");

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
                        total++;
                        Console.WriteLine(item.LocalePath);
                    }
                }
                Console.WriteLine($"Total unused: {total}");
            }

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }
    }
}