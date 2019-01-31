using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Utils;
using UnityEngine;

namespace Core.Configuration.Utils
{
    static class StringUtil
    {
        static LoggerAdapter _logger = new LoggerAdapter(typeof(StringUtil));

        public static Dictionary<Type, Func<string, object>> Handler = new Dictionary<Type, Func<string, object>>
        {
            { typeof(string), ToString },
            { typeof(int), ToInt32 },
            { typeof(Vector3), ToVector3 }
        };

        public static object ToString(string value)
        {
            return value;
        }

        public static object ToInt32(string value)
        {
            int ret = 0;
            try
            {
                ret = int.Parse(value);
            }
            catch (Exception e)
            {
                _logger.DebugFormat("String To Int32 with exception: {0}, source: {1}", e.Message, value);
            }

            return ret;
        }

        public static object ToVector3(string value)
        {
            Vector3 ret = new Vector3();
            try
            {
                var numbers = value.Split(',');
                ret.Set(float.Parse(numbers[0]), float.Parse(numbers[1]), float.Parse(numbers[2]));
            }
            catch (Exception e)
            {
                _logger.DebugFormat("String To Vector3 with exception: {0}, source: {1}", e.Message, value);
            }

            return ret;
        }


    }

    

  
}
