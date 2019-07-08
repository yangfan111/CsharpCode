using Core.Compensation;
using Core.ObjectPool;

namespace Core.Attack
{
   
    
    public class DefaultBulletSegment : BaseRefCounter
    {
         public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(DefaultBulletSegment)){}
            public override object MakeObject()
            {
                return new DefaultBulletSegment();
            }

        }

         public override string ToString()
         {
             return string.Format("ServerTime:{0},BulletEntityAgent:{1},RaySegment:{2}", ServerTime, BulletEntityAgent,
                 RaySegment);
         }

         public static DefaultBulletSegment Allocate(int serverTime, RaySegment raySegment, IBulletEntityAgent bulletEntityAgent)
        {
            var ret= ObjectAllocatorHolder<DefaultBulletSegment>.Allocate();
            ret.ServerTime = serverTime;
            ret.RaySegment = raySegment;
            ret.BulletEntityAgent = bulletEntityAgent;
            return ret;
        }

        private DefaultBulletSegment()
        {
            
        }

        public int ServerTime;
        public RaySegment RaySegment;
        public IBulletEntityAgent BulletEntityAgent;

        public bool IsValid
        {
            get { return BulletEntityAgent.IsValid; }
        }

        protected override void OnCleanUp()
        {
            ServerTime = 0;
            BulletEntityAgent = null;
            ObjectAllocatorHolder<DefaultBulletSegment>.Free(this);
        }
    }
}