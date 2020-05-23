using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.ObjectPool
{
    

    public class MemoryStreamObjectFactory : DefaultObjectFactory
    {
        public MemoryStreamObjectFactory() : base(typeof(MemoryStream))
        {
        }

        public override void ActivateObject(object arg0)
        {
            MemoryStream ms = (MemoryStream)(arg0);
            ms.Seek(0, SeekOrigin.Begin);
            ms.SetLength(0);
        }
    }

    public class DefaultObjectFactory : AbstractObjectFactory
    {
        private Type _type;
        private ObjectActivator _activator;
        public DefaultObjectFactory(Type type) : base(type)
        {
            _type = type;
            ConstructorInfo ctor = _type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .First(c => c.GetParameters().Length == 0);

            _activator = ObjectActivatorFactory.GetActivator(ctor);
        }


        public override object MakeObject()
        {
            return _activator(_type, true);
        }

        public override string ToString()
        {
            return string.Format("{0}", _type);
        }
    }
}