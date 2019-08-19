using App.Shared.FreeFramework.UnitTest;
using com.wd.free.action;
using com.wd.free.action.stage;
using com.wd.free.@event;
using com.wd.free.map;

namespace App.Shared.FreeFramework.framework.@event
{
    public class FreeContext
    {
        public bool AiSuccess;
        public bool OrderComplete;
        public bool DebugMode = false;

        public Contexts EntitasContexts;
        public FreeBufManager Bufs;
        public PosManager Poss;
        public TimerTask TimerTask;
        public MultiFrameActions MultiFrame;
        public TestCase TestCase;

        public FreeContext(Contexts contexts, IEventArgs args)
        {
            this.EntitasContexts = contexts;
            Bufs = new FreeBufManager();
            Poss = new PosManager(args);
            TimerTask = new TimerTask();
            MultiFrame = new MultiFrameActions();
            TestCase = new TestCase();
        }
    }
}
