using Core.Utils;
using System.Collections.Generic;
using System;
using Sharpen;

namespace com.cpkf.yyjd.tools.util.math
{
    public static class ParseUtility
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ParseUtility));
        private static ParseException parseException = new ParseException();
        #region parse value
        /// <summary>
        /// 当前测试环境下 20000次double.parse gc : 0.9m time : 240ms   20000次QuickDoubleParse gc : 0 time : 2ms
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double QuickDoubleParse(string input)
        {
            double result = 0;
            var pos = 0;
            var len = input.Length;
            if (len == 0) throw parseException;
            char c = input[0];
            double sign = 1;
            if (c == '-')
            {
                sign = -1;
                ++pos;
                if (pos >= len) throw parseException;
            }

            while (true) // breaks inside on pos >= len or non-digit character
            {
                if (pos >= len) return sign * result;
                c = input[pos++];
                if (c < '0' || c > '9') break;
                result = (result * 10.0) + (c - '0');
            }

            if (c != '.' && c != ',') throw parseException;
            double exp = 0.1;
            while (pos < len)
            {
                c = input[pos++];
                if (c < '0' || c > '9') throw parseException;
                result += (c - '0') * exp;
                exp *= 0.1;
            }
            return sign * result;
        }

        public static float QuickFloatParse(string input)
        {
            float result = 0;
            var pos = 0;
            var len = input.Length;
            if (len == 0) throw parseException;
            char c = input[0];
            float sign = 1;
            if (c == '-')
            {
                sign = -1;
                ++pos;
                if (pos >= len) throw parseException;
            }

            while (true) // breaks inside on pos >= len or non-digit character
            {
                if (pos >= len) return sign * result;
                c = input[pos++];
                if (c < '0' || c > '9') break;
                result = (result * 10.0f) + (c - '0');
            }

            if (c != '.' && c != ',') throw parseException;
            float exp = 0.1f;
            while (pos < len)
            {
                c = input[pos++];
                if (c < '0' || c > '9') throw parseException;
                result += (c - '0') * exp;
                exp *= 0.1f;
            }
            return sign * result;
        }

        public static int QuickIntParse(string input)
        {
            int result = 0;
            var pos = 0;
            var len = input.Length;
            if (len == 0) throw parseException;
            char c = input[0];
            int sign = 1;
            if (c == '-')
            {
                sign = -1;
                ++pos;
                if (pos >= len) throw parseException;
            }

            while (true) // breaks inside on pos >= len or non-digit character
            {
                if (pos >= len) return sign * result;
                c = input[pos++];
                if (c < '0' || c > '9') throw parseException;
                result = (result * 10) + (c - '0');
            }
        }

        public static bool QuickBoolParse(string input)
        {
            var pos = 0;
            var len = input.Length;
            if (len == 0) return false;
            char c = input[0];

            while (c == ' ') // breaks inside on pos >= len or non-digit character
            {
                if (pos >= len) return false;
                c = input[pos++];
            }

            if ((pos + 3) >= len) return false;
            return (
                (input[pos] == 'T' || input[pos] == 't') &&
                (input[pos++] == 'R' || input[pos] == 'r') &&
                (input[pos++] == 'U' || input[pos] == 'u') &&
                (input[pos++] == 'E' || input[pos] == 'e'));
        }
        #endregion

        #region parse type
        public static bool IsInt64(string type)
        {
            return (
                (type[0] == 'I' || type[0] == 'i') &&
                (type[1] == 'N' || type[1] == 'n') &&
                (type[2] == 'T' || type[2] == 't') &&
                type[3] == '6' &&
                type[4] == '4'
                );
        }

        public static bool IsInt32(string type)
        {
            return (
                (type[0] == 'I' || type[0] == 'i') &&
                (type[1] == 'N' || type[1] == 'n') &&
                (type[2] == 'T' || type[2] == 't') &&
                type[3] == '3' &&
                type[4] == '2'
                );
        }

        public static bool IsSingle(string type)
        {
            return (
                (type[0] == 'S' || type[0] == 's') &&
                (type[1] == 'I' || type[1] == 'i') &&
                (type[2] == 'N' || type[2] == 'n') &&
                (type[3] == 'G' || type[3] == 'g') &&
                (type[4] == 'L' || type[4] == 'l') &&
                (type[5] == 'E' || type[5] == 'e')
                );
        }

        public static bool IsDouble(string type)
        {
            return (
                (type[0] == 'D' || type[0] == 'd') &&
                (type[1] == 'O' || type[1] == 'o') &&
                (type[2] == 'U' || type[2] == 'u') &&
                (type[3] == 'B' || type[3] == 'b') &&
                (type[4] == 'L' || type[4] == 'l') &&
                (type[5] == 'E' || type[5] == 'e')
                );
        }

        public static bool IsString(string type)
        {
            return (
                (type[0] == 'S' || type[0] == 's') &&
                (type[1] == 'T' || type[1] == 't') &&
                (type[2] == 'R' || type[2] == 'r') &&
                (type[3] == 'I' || type[3] == 'i') &&
                (type[4] == 'N' || type[4] == 'n') &&
                (type[5] == 'G' || type[5] == 'g')
                );
        }

        public static bool IsBoolean(string type)
        {
            return (
                (type[0] == 'B' || type[0] == 'b') &&
                (type[1] == 'O' || type[1] == 'o') &&
                (type[2] == 'O' || type[2] == 'o') &&
                (type[3] == 'L' || type[3] == 'l') &&
                (type[4] == 'E' || type[4] == 'e') &&
                (type[5] == 'A' || type[5] == 'a') &&
                (type[6] == 'N' || type[6] == 'n')
                );
        }
        #endregion
    }
}
