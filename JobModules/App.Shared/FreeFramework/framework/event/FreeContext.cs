using App.Shared.FreeFramework.UnitTest;
using com.wd.free.action;
using com.wd.free.action.stage;
using com.wd.free.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.@event
{
    public class FreeContext
    {
        public bool AiSuccess;
        public bool DebugMode = false;

        public Contexts EntitasContexts;
        public FreeBufManager Bufs;
        public TimerTask TimerTask;
        public MultiFrameActions MultiFrame;
        public TestCase TestCase;

        public FreeContext(Contexts contexts)
        {
            this.EntitasContexts = contexts;
            Bufs = new FreeBufManager();
            TimerTask = new TimerTask();
            MultiFrame = new MultiFrameActions();
            TestCase = new TestCase();
        }
    }
}
