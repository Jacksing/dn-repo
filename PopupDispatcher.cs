using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rismo.Utils
{
    /// <summary>
    ///PopupDispatcher 的摘要说明
    /// </summary>
    public class PopupDispatcher
    {
        private static Dictionary<string, bool> _status = new Dictionary<string, bool>();

        public static string StatusComplete = "complete";
        public static string StatusProcessing = "processing";

        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static readonly int Base = Alphabet.Length;

        public static int Current = 0;

        public static string Encode(int i)
        {
            if (i == 0) return Alphabet[0].ToString();

            var s = string.Empty;

            while (i > 0)
            {
                s += Alphabet[i % Base];
                i = i / Base;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static int Decode(string s)
        {
            var i = 0;

            foreach (var c in s)
            {
                i = (i * Base) + Alphabet.IndexOf(c);
            }

            return i;
        }

        public static string Register()
        {
            //Guid guid = Guid.NewGuid();
            //_status.Add(guid.ToString(), false);
            //return guid.ToString();
            var guid = Encode(Current++);
            _status.Add(guid, false);
            return guid;
        }

        public static string Query(string guid)
        {
            if (!_status.ContainsKey(guid) || _status[guid])
            {
                return StatusComplete;
            }
            else
            {
                return StatusProcessing;
            }
        }

        public static bool Complete(string guid)
        {
            if (_status.ContainsKey(guid) && !_status[guid])
            {
                _status[guid] = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    } 
}