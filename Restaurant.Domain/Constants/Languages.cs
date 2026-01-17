using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Constants
{
    public static class Languages
    {
        public const string Arabic = "ar";
        public const string English = "en";

        public static readonly string[] SupportedLanguages = { English, Arabic };

        public static bool IsSupported(string language)
        {
            return SupportedLanguages.Contains(language);
        }
    }
}
