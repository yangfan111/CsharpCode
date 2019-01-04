using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using UnityEngine.Profiling;

namespace App.Client.Scripts
{
    public static class ClientDebugScript
    {
        public static bool Enabled = true;
        
        private static void MarkProfilerFrameTag()
        {
#if PROFILER_FRAME_TAG
            Profiler.SetFrameTag(ProfilerFrameDataTag.Stuck);
#endif
        }

        public static void DoUpdate(ClientGameController controller)
        {

#if ENABLE_PROFILER
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                MarkProfilerFrameTag();
                controller.SendDebugScriptInfo("MarkFrameTag");
            }
#endif
        }
    }
}
