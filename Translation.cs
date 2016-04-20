using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using TaxlabSession;

namespace Rismo.Utils.Translation
{
    public static class Extensions
    {
        private static Dictionary<string, string> lcMessages;
        private static CompilationSection cs = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation");
        private static string _localePath = HttpContext.Current.Server.MapPath("../../../Locale/zh_CN/LC_MESSAGES.po");

        private static readonly object _lockObject = new object();

        public static Dictionary<string, string> GetLcMessages()
        {
            if (lcMessages == null)
            {
                using (var fs = new FileStream(_localePath, FileMode.OpenOrCreate))
                {
                    StreamReader sr = new StreamReader(fs);
                    string lcMessagesContent = sr.ReadToEnd();

                    if (lcMessagesContent == "")
                    {
                        lcMessages = new Dictionary<string, string>();
                    }
                    else
                    {
                        var splitedContent = Regex.Split(lcMessagesContent, string.Format("{0}{0}", Environment.NewLine));
                        splitedContent = (from sc in splitedContent where sc.Trim() != "" select sc).ToArray();

                        lcMessages = new Dictionary<string, string>();

                        var parttern = string.Format("msgid |{0}msgstr ", Environment.NewLine);

                        foreach (var item in splitedContent)
                        {
                            var msgPair = Regex.Split(item, parttern);
                            lcMessages.Add(msgPair[1], msgPair[2]);
                        }
                    }
                }
            }

            return lcMessages;
        }

        public static void SaveLcMessages()
        {
            lock (_lockObject)
            {
                using (var fs = new FileStream(_localePath, FileMode.Create))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in lcMessages)
                    {
                        if (sb.Length != 0)
                        {
                            sb.AppendFormat("{0}{0}", Environment.NewLine);
                        }
                        sb.AppendFormat("msgid {1}{0}msgstr {2}", Environment.NewLine, item.Key, item.Value);
                    }

                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    sw.Write(sb.ToString());
                    sw.Flush();
                }
            }
        }

        public static string Translate(this string s)
        {
            try
            {
                if (s == "") return "";

                if (!GetLcMessages().ContainsKey(s))
                {
                    GetLcMessages().Add(s, "");
                }

                if (TaxlabPageBase.GetUserSession().CurrentLanguage == Language.CHN)
                {
                    var translated = GetLcMessages()[s];
                    if (translated == "" && cs.Debug)
                    {
                        translated = string.Format("[CN]{0}", s);
                    }
                    return translated;
                }
                return s;
            }
            finally
            {
                if (cs.Debug)
                {
                    SaveLcMessages();
                }
            }
        }

        public static string _(this string s)
        {
            return s.Translate();
        }
    }

    public class TranslatedString
    {
        public string translatedString = null;
        public string OrignalString { get; private set; }

        public TranslatedString(string s)
        {
            this.OrignalString = s;
        }

        public static implicit operator string(TranslatedString ts)
        {
            if (ts.translatedString == null)
            {
                ts.translatedString = ts.OrignalString.Translate();
            }
            return ts.translatedString;
        }

        public static explicit operator TranslatedString(string s)
        {
            TranslatedString ts = new TranslatedString(s);
            return ts;
        }

        public override string ToString()
        {
            return (string)this;
        }
    }
}