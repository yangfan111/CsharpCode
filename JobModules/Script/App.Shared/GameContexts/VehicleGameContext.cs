using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.GameModule.Common;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace App.Shared.GameContexts
{
    public class VehicleGameContext : EntitasGameContext<VehicleEntity>
    {
        private VehicleContext _context;
        private IGroup<VehicleEntity> _destroyEntities;

        public VehicleGameContext(VehicleContext context, Bin2D<IGameEntity> bin) : base(context, VehicleComponentsLookup.componentTypes, bin)
        {
            _context = context;
        }

        protected override VehicleEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((VehicleContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Vehicle; }
        }


        public int DestroyComponentId
        {
            get { return VehicleComponentsLookup.FlagDestroy; }
        }

        public Entity[] DestoryEntities
        {
            get
            {
                if (_destroyEntities == null)
                {
                    _destroyEntities = _context.GetGroup(VehicleMatcher.AnyOf(
                        VehicleMatcher.FlagDestroy));

                }
                return _destroyEntities.GetEntities();
            }
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<VehicleEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<VehicleEntity>();
            }

            public override int InitPoolSize
            {
                get { return 128; }
            }
        }
    }
}
