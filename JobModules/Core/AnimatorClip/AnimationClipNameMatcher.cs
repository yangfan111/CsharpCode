using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Animation
{
    class AnimationClipNameMatcher
    {
        // 
        private readonly Regex animPattern;

        public AnimationClipNameMatcher()
        {
            animPattern = new Regex(@"[\w]+?_([a-zA-Z0-9]+)(?:_([FBLR]{1,2}|Idle))?$");
        }
        public readonly Dictionary<string,string> cache = new Dictionary<string, string>();

        public string Match(string name)
        {
            if (cache.ContainsKey(name)) return cache[name];
            var match = animPattern.Match(name);
            var ret = string.Empty;
            if (match.Success)
            {
                ret= match.Groups[1].Value;
            }

            cache[name] = ret;
            return ret;
        }
    }
}
