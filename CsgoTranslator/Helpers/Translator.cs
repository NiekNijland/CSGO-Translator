using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using CsgoTranslator.EventArgs;
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
                MainWindow.Succeeded(null, new TranslatorExceptionEventArgs { Exception = new GoogleTranslateTimeoutException() });
                MainWindow.Succeeded(null, new TranslatorExceptionEventArgs { Exception = new NoInternetException() });
            }
            catch (TranslatorException e)
            {
                translation = new Translation(lang);    
                MainWindow.ErrorEncountered(null, new TranslatorExceptionEventArgs { Exception = e });
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
            catch(WebException e)
            {
                if(e.Message.StartsWith("The remote name could not be resolved: "))
                    throw new NoInternetException();
                
                if(e.Message == "The remote server returned an error: (429) Too Many Requests.")
                    throw new GoogleTranslateTimeoutException();

                throw;
            }
        }
    }
}
