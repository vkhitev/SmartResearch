using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartResearch.KnowledgeBase.Predicates
{
    public class Function: Arg
    {
        private Arg[] _args;
        public Arg[] Args
        {
            get { return _args; }
        }

        public Function(string name, Arg[] args): base(name, ArgType.Function)
        {
            _args = args.Clone() as Arg[];
        }

        public Function(string name, List<string> args) : base(name, ArgType.Function)
        {
            List<Arg> temp_args = new List<Arg>();
            foreach (string arg_name in args)
                temp_args.Add(new Arg(arg_name));
            _args = temp_args.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);
            sb.Append("(");
            for (int i = 0; i < Args.Length; i++)
            {
                sb.Append(Args[i].Name);
                if (i < Args.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
