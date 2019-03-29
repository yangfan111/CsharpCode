using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Core.Http
{
    public class HttpUtility
    {
        static char[] hexChars = "0123456789abcdefABCDEF".ToCharArray();

        private static readonly Dictionary<char, string> _urlECSEncodeTable = new Dictionary<char, string>
        {
            {' ', "%20"},
            {'!', "%21"},
            {'"', "%22"},
            {'#', "%23"},
            {'$', "%24"},
            {'%', "%25"},
            {'&', "%26"},
            {'\'', "%27"},
            {'(', "%28"},
            {')', "%29"},
            {'*', "%2A"},
            {'+', "%2B"},
            {',', "%2C"},
            {'-', "%2D"},
            {'.', "%2E"},
            {'/', "%2F"},
            {'0', "%30"},
            {'1', "%31"},
            {'2', "%32"},
            {'3', "%33"},
            {'4', "%34"},
            {'5', "%35"},
            {'6', "%36"},
            {'7', "%37"},
            {'8', "%38"},
            {'9', "%39"},
            {':', "%3A"},
            {';', "%3B"},
            {'<', "%3C"},
            {'=', "%3D"},
            {'>', "%3E"},
            {'?', "%3F"},
            {'@', "%40"},
            {'A', "%41"},
            {'B', "%42"},
            {'C', "%43"},
            {'D', "%44"},
            {'E', "%45"},
            {'F', "%46"},
            {'G', "%47"},
            {'H', "%48"},
            {'I', "%49"},
            {'J', "%4A"},
            {'K', "%4B"},
            {'L', "%4C"},
            {'M', "%4D"},
            {'N', "%4E"},
            {'O', "%4F"},
            {'P', "%50"},
            {'Q', "%51"},
            {'R', "%52"},
            {'S', "%53"},
            {'T', "%54"},
            {'U', "%55"},
            {'V', "%56"},
            {'W', "%57"},
            {'X', "%58"},
            {'Y', "%59"},
            {'Z', "%5A"},
            {'[', "%5B"},
            {'\\', "%5C"},
            {']', "%5D"},
            {'^', "%5E"},
            {'_', "%5F"},
            {'`', "%60"},
            {'a', "%61"},
            {'b', "%62"},
            {'c', "%63"},
            {'d', "%64"},
            {'e', "%65"},
            {'f', "%66"},
            {'g', "%67"},
            {'h', "%68"},
            {'i', "%69"},
            {'j', "%6A"},
            {'k', "%6B"},
            {'l', "%6C"},
            {'m', "%6D"},
            {'n', "%6E"},
            {'o', "%6F"},
            {'p', "%70"},
            {'q', "%71"},
            {'r', "%72"},
            {'s', "%73"},
            {'t', "%74"},
            {'u', "%75"},
            {'v', "%76"},
            {'w', "%77"},
            {'x', "%78"},
            {'y', "%79"},
            {'z', "%7A"},
            {'{', "%7B"},
            {'|', "%7C"},
            {'}', "%7D"},
            {'~', "%7E"}
        };

        private static readonly Dictionary<string, char> _urlECSDecodeTable = new Dictionary<string, char>();

        static HttpUtility()
        {

            foreach (var item in _urlECSEncodeTable)
            {
                _urlECSDecodeTable.Add(item.Value, item.Key);
            }
        }

        private static string UrlEncode(string context)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in context)
            {
                if (_urlECSEncodeTable.ContainsKey(c))
                    sb.Append(_urlECSEncodeTable[c]);
                else sb.Append(c);
            }
            return sb.ToString();
        }

        public static string TransNVCToString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            int num = 0;
            foreach (var key in nvc.AllKeys)
            {
                if (num != 0) sb.Append("&");
                sb.Append(UrlEncode(key));
                sb.Append("=");
                sb.Append(UrlEncode(nvc[key]));
                num++;
            }
            return sb.ToString();
        }

        internal static void QueryDecodeTo(string content, NameValueCollection target)
        {
            QueryDecodeTo(content, target, Encoding.Default);
        }

        internal static void QueryDecodeTo(string content, NameValueCollection target, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            if (target == null)
                throw new ArgumentNullException("target");
            
            string[] parts = content.Split('&');

            foreach (string part in parts)
            {
                string[] item = part.Split(new[] {'='}, 2);

                string key = UriDecode(item[0], encoding);
                string value = item.Length == 1 ? "" : UriDecode(item[1], encoding);

                target.Add(key, value);
            }
        }

        internal static string UriDecode(string value)
        {
            return UriDecode(value, Encoding.Default);
        }

        private static string UriDecode(string value, Encoding encoding)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            byte[] result = new byte[value.Length];
            int length = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i].Equals('%'))
                {
                    var a = 0;
                }

                if (value[i] == '%' &&
                    i < value.Length - 2 &&
                    IsHex(value[i + 1]) &&
                    IsHex(value[i + 2]) &&
                    _urlECSDecodeTable.ContainsKey(value.Substring(i, 3))
                )
                {
                    result[length++] = (byte) _urlECSDecodeTable[value.Substring(i, 3)];
                    i += 2;
                }
                else if (value[i] == '+')
                {
                    result[length++] = (byte) ' ';
                }
                else
                {
                    int c = value[i];

                    if (c > byte.MaxValue)
                        throw new InvalidOperationException("URI contained unexpected character");

                    result[length++] = (byte) c;
                }
            }

            return encoding.GetString(result, 0, length);
        }

        private static bool IsHex(char value)
        {
            return hexChars.Contains(value);
        }

        static int HexToInt(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;

            return -1;
        }

        public static void TrimAll(string[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
        }
    }
}