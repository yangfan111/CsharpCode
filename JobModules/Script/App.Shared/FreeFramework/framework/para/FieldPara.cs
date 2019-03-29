using System;
using System.Reflection;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.exception;

namespace com.wd.free.para
{
    [System.Serializable]
    public class FieldPara : AbstractPara
    {
        private const long serialVersionUID = 7322706102008731347L;

        [System.NonSerialized]
        private object obj;

        [System.NonSerialized]
        private FieldInfo field;

        [System.NonSerialized]
        private IPara para;

        public FieldPara()
        {
        }

        public FieldPara(object obj, string fieldName, FieldInfo field)
            : base()
        {
            this.obj = obj;
            this.field = field;
            this.name = fieldName;
            this.para = ReflectionCache.GetPara(obj, fieldName);
            if (para == null)
            {
                throw new GameConfigExpception(fieldName + " can not be set/get at " + obj.GetType().FullName);
            }
        }

        public override string[] GetConditions()
        {
            return para.GetConditions();
        }

        public override bool Meet(string con, IPara v)
        {
            return para.Meet(con, v);
        }

        public override object GetValue()
        {
            try
            {
                object v = field.GetValue(obj);
                return v;
            }
            catch (ArgumentException e)
            {
                Sharpen.Runtime.PrintStackTrace(e);
            }
            catch (MemberAccessException e)
            {
                Sharpen.Runtime.PrintStackTrace(e);
            }
            return null;
        }

        public override string[] GetOps()
        {
            return para.GetOps();
        }

        public override void SetValue(string op, IPara v)
        {
            try
            {
                para.SetValue(op, v);
                if (op.Equals(OP_ASSIGN))
                {
                    field.SetValue(obj, para.GetValue());
                }
            }
            catch (Exception e)
            {
                throw new GameConfigExpception("set value failed.\n" + ExceptionUtil.GetExceptionContent(e));
            }
        }

        public override IPara Initial(string con, string v)
        {
            return para.Initial(con, v);
        }

        public override IPoolable Copy()
        {
            return para.Copy();
        }

        private static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.FieldPara());

        protected internal override ParaPool<IPara> GetPool()
        {
            return pool;
        }
    }
}
