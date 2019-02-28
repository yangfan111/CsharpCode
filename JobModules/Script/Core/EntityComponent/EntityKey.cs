using App.Shared;
using App.Shared.Components;
using System;
using System.Collections.Generic;

namespace Core.EntityComponent
{
    [Serializable]
    public class EntityKeyComparer : IEqualityComparer<EntityKey>, IComparer<EntityKey>
    {
        private static EntityKeyComparer _instance = new EntityKeyComparer();
        public static EntityKeyComparer Instance { get { return _instance; } }
        public bool Equals(EntityKey x, EntityKey y)
        {
            return x == y;
        }

        public int GetHashCode(EntityKey obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(EntityKey x, EntityKey y)
        {
            if (x.EntityType < y.EntityType)
                return -1;
            if (x.EntityType > y.EntityType)
                return 1;
            if (x.EntityId < y.EntityId)
                return -1;
            if (x.EntityId > y.EntityId)
                return 1;
            return 0;
        }
    }
    [Serializable]
    public struct EntityKey
    {
        public static readonly EntityKey Default =new EntityKey(0,-1);

        public EntityKey(int entityId, short entityType)
        {
            
            EntityId = entityId;
            EntityType = entityType;
        }

        public readonly int EntityId;
        public readonly short EntityType;
        public override bool Equals(object obj)
        {
            // ReSharper disable once PossibleNullReferenceException
            EntityKey other = (EntityKey)obj;
            return ((other.EntityId == EntityId) && (other.EntityType == EntityType));
        }

        public override int GetHashCode()
        {
            int hashCode = 1392425097;
            hashCode = hashCode * -1521134295 + EntityId;
            hashCode = hashCode * -1521134295 + EntityType;
            return hashCode;
        }
		
        
		public override string ToString()
		{
		    string type = "" + EntityType;
		    switch (EntityType)
		    {
		        case 0:
		            type = "Player";
		            break;
		        case 1:
		            type = "Bullet";
		            break;
		        case 2:
		            type = "ClientEffect";
		            break;
		        case 3:
		            type = "Vehicle";
		            break;
		        case 4:
		            type = "Equipment";
		            break;                 
		    }
		    return  string.Format("T{1}_{0}{2}", type, EntityType, EntityId);
		}

        public static bool operator ==(EntityKey x, EntityKey y)
        {
            return x.EntityId == y.EntityId && x.EntityType == y.EntityType;
        }
        public static bool operator !=(EntityKey x, EntityKey y)
        {
            return x.EntityId != y.EntityId || x.EntityType != y.EntityType;
        }

        public static bool operator <(EntityKey x, EntityKey y)
        {
            return x.EntityType < y.EntityType || (x.EntityType == y.EntityType && x.EntityId < y.EntityId);
        }

        public static bool operator >(EntityKey x, EntityKey y)
        {
            return x.EntityType > y.EntityType || (x.EntityType == y.EntityType && x.EntityId > y.EntityId);
        }
    }
}