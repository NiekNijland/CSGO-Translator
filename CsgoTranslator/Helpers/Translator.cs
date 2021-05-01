using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using CsgoTranslator.Exceptions;
using CsgoTranslator.Models;

namespace CsgoTranslator.Helpers
{
    /**
     * <summary>
     * Translates text using Google's online language tools.
     * </summary>
     */
    public static class Translator
    {
        
        private static readonly Dictionary<string, Translation> CachedTranslations = new Dictionary<string, Translation>();

        public static Translation GetTranslation(string sourceText, string lang = null)
        {
            lang ??= Properties.Settings.Default.Lang;

            if (CachedTranslations.ContainsKey(sourceText + lang))
                return CachedTranslations[sourceText + lang];

            Translation translation;
            try
            {
                translation = Translate(sourceText, lang);
            }
            catch (GoogleTranslateTimeoutException)
            {
                translation = new Translation(lang);    
                /* TODO: Fire some sort of event to MainWindow so we can display a proper error message */
            }

            CachedTranslations[sourceText + lang] = translation;
            
            return translation;
        }
        
        private static Translation Translate(string sourceText, string lang)
        {
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={lang}&dt=t&q={HttpUtility.UrlEncode(sourceText)}";
            var outputFile = Path.GetTempFileName();

            /* This method of getting translations is rate limited at 100 / hour, so big chance it will throw a 429 */
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    wc.DownloadFile(url, outputFile);
                }
                var message = File.ReadAllText(outputFile).Substring(4).Split('"')[0];
                return new Translation(lang, message);
            }
            catch(Exception)
            {
                /* TODO: add exception validation and maybe more catch hooks */
                throw new GoogleTranslateTimeoutException(); 
            }
        }
    }
}
