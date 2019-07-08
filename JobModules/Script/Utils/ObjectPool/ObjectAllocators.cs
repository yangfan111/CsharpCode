using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LiteNetLib;

namespace Core.ObjectPool
{
    public class ObjectAllocators
    {
        public static int DUMMY = 0;
        static Dictionary<Type, IObjectAllocator> _allocatorDic = new Dictionary<Type, IObjectAllocator>();

        private static List<CustomAbstractObjectFactory> FindAllGameComponentType()
        {
            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            List<CustomAbstractObjectFactory> resList = new List<CustomAbstractObjectFactory>();
            foreach (var assembly in assemblyList)
            {
                try
                {
                    List<Type> typeListInAssembly = DiscoverCustomAbstractObjectFactoryInAssembly(assembly);
                    foreach (var type in typeListInAssembly)
                    {
                        resList.Add((CustomAbstractObjectFactory) Activator.CreateInstance(type));
                    }
                }
                catch (Exception e)
                {
                }
            }

            return resList;
        }

        private static List<Type> DiscoverCustomAbstractObjectFactoryInAssembly(Assembly assembly)
        {
            try
            {
                Type[] typeList = assembly.GetTypes();
                List<Type> resList = new List<Type>();
                foreach (var type in typeList)
                {
                    if ((typeof(CustomAbstractObjectFactory).IsAssignableFrom(type)) &&
                        (!type.IsAbstract && !type.IsInterface))
                    {
                        resList.Add(type);
                    }
                }

                return resList;
            }
            catch (NotSupportedException)
            {
                //dononthing
                return new List<Type>();
            }
        }

        static ObjectAllocators()
        {
            GetAllocator(typeof(MemoryStream)).Factory = new MemoryStreamObjectFactory();
            foreach (var customAbstractObjectFactory in FindAllGameComponentType())
            {
                var type = customAbstractObjectFactory.Type;
                _allocatorDic[type] = new RingBufferObjectAllocator(customAbstractObjectFactory, 
                    customAbstractObjectFactory.InitPoolSize, customAbstractObjectFactory.AllocatorNumber);
            }
        }

        public static IObjectAllocator GetAllocator(Type type)
        {
            IObjectAllocator rc;
            if (!_allocatorDic.TryGetValue(type, out rc))
            {
                rc = new RingBufferObjectAllocator(new DefaultObjectFactory(type));
                _allocatorDic[type] = rc;
            }

            return rc;
        }

        public static void SetAllocator(Type type, IObjectAllocator allocator)
        {
            _allocatorDic[type] = allocator;
        }

        public static string PrintAllDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");

            sb.Append("<p>Object Pools</p>");
            sb.Append ("<table id=\"table_id\" class=\"display\" width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td>name</td>");
            sb.Append("<td>allocCount</td>");
            sb.Append("<td>freeCount</td>");
            sb.Append("<td>activeCount</td>");
            sb.Append("<td>newCount</td>");
            sb.Append("<td>poolSize</td>");
            sb.Append("<td>PoolCap</td>");
            sb.Append("</thead>");

            foreach (var allocator in _allocatorDic.Values)
            {
                allocator.PrintDebugInfo(sb);
               
            }
            NetPacketPoolsStatue.PrintDebugInfo(sb);
            sb.Append("</table>");
            return sb.ToString();
        }
    }
}