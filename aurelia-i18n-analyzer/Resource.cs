using System.Collections.Generic;

namespace aureliai18nanalyzer
{
    public class Resource
    {
        public string Path
        {
            get;
            set;
        }

        public List<Locale> Locales
        {
            get;
            set;
        }

        public Resource()
        {
        }
    }
}
