/*
 * Translation module for i18n.
 * 
 * Create by jackrole 2016/04/21
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace Jackrole.Utils.Translation
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

                Func<string> getEnvironmentLanguage = () =>
                {
                    throw new NotImplementedException();
                };

                if (getEnvironmentLanguage() == "CN")
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
            catch
            {
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
        private string translatedString = null;
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

    public class TranslatedBlock
    {
        private string translatedBlock = null;
        public string OrignalBlock { get; private set; }

        public TranslatedBlock(string s)
        {
            this.OrignalBlock = s;
        }

        public static implicit operator string(TranslatedBlock ts)
        {
            if (ts.translatedBlock == null)
            {
                List<string> matchStrings = new List<string>();
                var translatedBlock = ts.OrignalBlock;

                foreach (Match match in Regex.Matches(translatedBlock, "\\(TS\\){{(.*?)}}"))
                {
                    if (!matchStrings.Contains(match.Value))
                    {
                        translatedBlock = translatedBlock.Replace(match.Groups[0].Value, match.Groups[1].Value.Translate());
                        matchStrings.Add(match.Value);
                    }
                }

                ts.translatedBlock = translatedBlock;
            }
            return ts.translatedBlock;
        }

        public static explicit operator TranslatedBlock(string s)
        {
            TranslatedBlock ts = new TranslatedBlock(s);
            return ts;
        }

        public override string ToString()
        {
            return (string)this;
        }
    }
}
