using System;
using System.Collections.Generic;
using System.Text;

namespace Core.EntityComponent
{
	public class EntityMapComparaeStatisticTotal
	{
		public int TotalEntityCount;
		public int TotalComponentCount;
		public int LastEntityCount;
		public int LastComponentCount;
		public DateTime LastCreateTime;
		public int LastInterval;
		public int TotalInterval;
	}

	public struct EntityMapComparaeStatisticOne
    {
        public int EntityCount;
        public int ComponentCount;
        public DateTime CreateTime;
	    public int Interval;
	}

    public class EntityMapComparator
    {
        
        private static Dictionary<string , EntityMapComparaeStatisticTotal> _statistics = new Dictionary<string, EntityMapComparaeStatisticTotal>();

        public static string PrintDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>EntityMapCompare</p>");
            sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td>id</td>");
            sb.Append("<td>total entity</td>");
            sb.Append("<td>total component</td>");
	        sb.Append("<td>total interval(ms)</td>");
	        sb.Append("<td>last entity</td>");
	        sb.Append("<td>last component</td>");
	        sb.Append("<td>last interval(ms)</td>");
			sb.Append("<td>age(ms)</td>");
            sb.Append("</thead>");

            foreach (var counter in _statistics)
            {
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append(counter.Key);
                sb.Append("</td>");

				sb.Append("<td>");
                sb.Append(counter.Value.TotalEntityCount);
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append(counter.Value.TotalComponentCount);
                sb.Append("</td>");
	            sb.Append("<td>");
	            sb.Append((counter.Value.TotalInterval));
	            sb.Append("</td>");

	            sb.Append("<td>");
	            sb.Append(counter.Value.LastEntityCount);
	            sb.Append("</td>");
	            sb.Append("<td>");
	            sb.Append(counter.Value.LastComponentCount);
	            sb.Append("</td>");
	            sb.Append("<td>");
	            sb.Append((counter.Value.LastInterval));
	            sb.Append("</td>");

				sb.Append("<td>");
                sb.Append((DateTime.Now - counter.Value.LastCreateTime).TotalMilliseconds);
                sb.Append("</td>");

            }
            sb.Append("</table>");
            return sb.ToString();
        }



	    public static void Diff(
		    EntityMap left,
		    EntityMap right,
		    IEntityMapCompareHandler handler, 
		    string staticsticId = "",
		    IGameEntityComparator gameEntityComparator = null, bool skipMissHandle=false)
	    {
		    if (gameEntityComparator == null)
			    gameEntityComparator = new GameEntityComparator(handler);
		    
		    EntityMapComparaeStatisticOne oneStat = UnsortedEntityMapComparator.Diff(left, right, handler, gameEntityComparator, skipMissHandle);
		    RecordStatistics(staticsticId, oneStat);
	    }
	    
	   


	    private static void RecordStatistics(string staticsticId, EntityMapComparaeStatisticOne oneStat)
	    {
		    EntityMapComparaeStatisticTotal totalStat;
		    if (!_statistics.TryGetValue(staticsticId, out totalStat))
		    {
			    totalStat = new EntityMapComparaeStatisticTotal();
			    _statistics[staticsticId] = totalStat;
		    }

		    totalStat.TotalComponentCount += oneStat.ComponentCount;
		    totalStat.TotalEntityCount += oneStat.EntityCount;
		    totalStat.LastComponentCount = oneStat.ComponentCount;
		    totalStat.LastEntityCount = oneStat.EntityCount;
		    totalStat.LastCreateTime = oneStat.CreateTime;
		    totalStat.LastInterval = oneStat.Interval;
		    totalStat.TotalInterval = oneStat.Interval;
	    }
    }


    public class UnsortedEntityMapComparator
    {
        public static EntityMapComparaeStatisticOne Diff(EntityMap left, EntityMap right, IEntityMapCompareHandler handler,IGameEntityComparator gameEntityComparator,bool skipMissHandle=false )
        {
            int entityCount = 0;
            int componentCount = 0;
			DateTime startTime = DateTime.Now;
           
            foreach (var rightEntry in right)
            {
                entityCount++;
                if (handler.IsBreak())
                {
                    break;
                }
                var entityKey = rightEntry.Key;
                var rightEntity = rightEntry.Value;
                IGameEntity leftEntity;
                left.TryGetValue(entityKey, out leftEntity);

                if (leftEntity == null)
                {
	                if(!skipMissHandle)
						handler.OnLeftEntityMissing(rightEntity);
                    continue;
                }

                componentCount += gameEntityComparator.Diff(leftEntity, rightEntity,skipMissHandle);
                

            }

            if (!handler.IsBreak() && !skipMissHandle)
            {
                foreach (var leftEntry in left)
                {
                    
                    var entityKey = leftEntry.Key;
                    var leftEntity = leftEntry.Value;

                    IGameEntity rightEntity;
                    if (!right.TryGetValue(entityKey, out rightEntity))
                    {
	                    entityCount++;
						handler.OnRightEntityMissing(leftEntity);
                    }
                }
            }

            return new EntityMapComparaeStatisticOne
            {
                EntityCount = entityCount,
                ComponentCount = componentCount,
                CreateTime = DateTime.Now,
				Interval = (int)(startTime- startTime).TotalMilliseconds
            };
        }
    }
}