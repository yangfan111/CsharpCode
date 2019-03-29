using Core.Compensation;
using Core.ObjectPool;

namespace Core.BulletSimulation
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

        public static DefaultBulletSegment Allocate(int serverTime, RaySegment raySegment, IBulletEntity bulletEntity)
        {
            var ret= ObjectAllocatorHolder<DefaultBulletSegment>.Allocate();
            ret.ServerTime = serverTime;
            ret.RaySegment = raySegment;
            ret.BulletEntity = bulletEntity;
            return ret;
        }

        private DefaultBulletSegment()
        {
            
        }

        public int ServerTime;
        public RaySegment RaySegment;
        public IBulletEntity BulletEntity;

        public bool IsValid
        {
            get { return BulletEntity.IsValid; }
        }

        protected override void OnCleanUp()
        {
            ServerTime = 0;
            BulletEntity = null;
            ObjectAllocatorHolder<DefaultBulletSegment>.Free(this);
        }
    }
}