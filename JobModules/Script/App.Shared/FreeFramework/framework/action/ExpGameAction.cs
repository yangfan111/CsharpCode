using System;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;

namespace com.wd.free.action
{
    [System.Serializable]
    public class ExpGameAction : AbstractGameAction
    {
        private const long serialVersionUID = 1131444043139644761L;

        private string exp;

        [System.NonSerialized]
        private ExpParaOp op;

        public ExpGameAction()
            : base()
        {
        }

        public virtual string GetExp()
        {
            return exp;
        }

        public virtual ExpParaOp GetOp(IEventArgs args)
        {
            if (op == null)
            {
                op = ExpSetParser.Parse(exp, args);
            }
            return op;
        }

        public ExpGameAction(string exp)
            : base()
        {
            this.exp = exp;
        }

        public override void DoAction(IEventArgs args)
        {
            // 如果有变量每次重新生成
            if (op == null || (exp.Contains(FreeUtil.VAR_START) && exp.Contains(FreeUtil.VAR_END)))
            {
                op = ExpSetParser.Parse(FreeUtil.ReplaceVar(exp, args), args);
            }
            try
            {
                op.Op(args);
            }
            catch (Exception e)
            {
                Sharpen.Runtime.PrintStackTrace(e);
            }
        }

        public string Exp
        {
            get { return exp; }
            set { exp = value; }
        }

        public override string ToString()
        {
            return exp;
        }
    }
}
