using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utils.Singleton;

namespace Utils.AssetManager
{
    public class LoadRequestProfile
    {
        public readonly AssetInfo Info;
        public int RequestTimes;
        public int InstantiateTimes;
        public int PooledTimes;
        public int RecycleTimes;
        public int DestroyTimes;
        public long TotalInstantiateTime;
        public long TotalHandlerTime;
        public long TotalRecycleTime;
        private Stopwatch _stopwatch = new Stopwatch();
        public LoadRequestProfile(AssetInfo info)
        {
            Info = info;
        }

        public static String HtmlTableHead;

        static LoadRequestProfile()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>Duration Info</p>");
            sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td>AssetInfo</td>");
            sb.Append("<td>RequestTimes</td>");
            sb.Append("<td>InstantiateTimes</td>");
            sb.Append("<td>PooledTimes</td>");
            sb.Append("<td>RecycleTimes</td>");
            sb.Append("<td>DestroyTimes</td>");
            sb.Append("<td>TotalInstantiateTime</td>");
            sb.Append("<td>TotalHandlerTime</td>");
            sb.Append("<td>TotalRecycleTime</td>");
            sb.Append("</thead>");
            HtmlTableHead = sb.ToString();
        }

        public void StartWatch()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public long StopWatch()
        {
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }

        public string GetHtmlRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>").Append(Info.BundleName).Append(".").Append(Info.AssetName).Append("</td>");
            sb.Append("<td>").Append(RequestTimes).Append("</td>");
            sb.Append("<td>").Append(InstantiateTimes).Append("</td>");
            sb.Append("<td>").Append(PooledTimes).Append("</td>");
            sb.Append("<td>").Append(RecycleTimes).Append("</td>");
            sb.Append("<td>").Append(DestroyTimes).Append("</td>");
            sb.Append("<td>").Append(TotalInstantiateTime / 10000f).Append("</td>");
            sb.Append("<td>").Append(TotalHandlerTime / 10000f).Append("</td>");
            sb.Append("<td>").Append(TotalRecycleTime / 10000f).Append("</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }
    }

    public class LoadRequestProfileHelp : Singleton<LoadRequestProfileHelp>
    {
        private Dictionary<AssetInfo, LoadRequestProfile> _dictionary =
            new Dictionary<AssetInfo, LoadRequestProfile>(AssetInfo.AssetInfoComparer.Instance);

        public LoadRequestProfile GetProfile(AssetInfo info)
        {
            if (!_dictionary.ContainsKey(info))
            {
                LoadRequestProfile profile = new LoadRequestProfile(info);
                _dictionary[info] = profile;
                return profile;
            }

            return _dictionary[info];
        }

        public String GetHtml(bool filter = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LoadRequestProfile.HtmlTableHead);
            foreach (var loadRequestProfile in _dictionary.Values)
            {
                if (!filter || loadRequestProfile.RequestTimes > 1)
                    sb.Append(loadRequestProfile.GetHtmlRow());
            }
            return sb.ToString();
        }
    }



}
