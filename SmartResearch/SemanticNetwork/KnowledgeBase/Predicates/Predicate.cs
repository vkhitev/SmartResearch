using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KnowledgeBase.Predicates
{
    public class Predicate
    {
        public bool IsNot { get; set; }

        private int _arg_num;
        public int ArgNum
        {
            get { return _arg_num; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private Arg[] _args;
        public Arg[] Args
        {
            get { return _args; }
            set
            {
                if (value.Length == _args.Length)
                    for (int i = 0; i < ArgNum; i++)
                        _args[i].Set(value[i]);
            }
        }

        public int UniqueArgNum
        { get { return UniqueArgs.Count; } }
        public List<Arg> UniqueArgs
        {
            get
            {
                List<Arg> u_args = new List<Arg>();
                foreach (Arg a in Args)
                    if (!u_args.Contains(a))
                        u_args.Add(a);
                return u_args;
            }
            set
            {
                List<Arg> u_args = UniqueArgs;
                for (int i = 0; i <Math.Min(u_args.Count,value.Count); i++)
                    u_args[i].Set(value[i]);
            }
        }

        private StringBuilder _str_representation;
        public string StrRepresentation
        {
            get
            {
                if (_str_representation == null)
                    return "Undefined";
                else
                {
                    StringBuilder output = new StringBuilder(_str_representation.ToString());
                    for (int i = 0; i < _arg_num; i++)
                    {
                        output = output.Replace(String.Format("unique_arg{0}", i), _args[i].Name);
                    }
                    return output.ToString();
                }
            }
        }

        public Predicate(string name, string[] args, string representation = null)
        {
            IsNot = false;
            args = (from arg in args where arg != "" select arg).ToArray();  //delete empty strings
            _name = name;
            _arg_num = args.Length;
            _args = new Arg[_arg_num];

            string pattern;
            string expr = representation;
            for (int i = 0; i < args.Length; i++)
            {
                _args[i] = new Arg(args[i], ArgType.Variable);
                pattern = String.Format(@"(?<=\W?){0}(?=\W?)", args[i]);
                expr = Regex.Replace(expr, pattern, String.Format("unique_arg{0}", i));
            }
            _str_representation = new StringBuilder(expr);
        }

        public Predicate(string representation, string name, params Arg[] args)
        {
            IsNot = false;
            _name = name;
            _arg_num = args.Length;
            _args = args.Clone() as Arg[];

            string pattern;
            string expr = representation;
            for (int i = 0; i < args.Length; i++)
            {
                pattern = String.Format(@"(?(^)|(?<=\W)){0}(?($)|(?=\W))", args[i]);
                expr = Regex.Replace(expr, pattern, String.Format("unique_arg{0}", i));
            }
            _str_representation = new StringBuilder(expr);
        }

        public Predicate(string name,params Arg[] args): this(null,name,args)
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsNot)
                sb.Append("¬");
            sb.Append(_name + "(");
            for (int i = 0; i < ArgNum; i++)
            {
                sb.Append(Args[i].Name);
                if (i < ArgNum - 1)
                    sb.Append(",");
            }
            sb.Append(")");
            return sb.ToString();
        }

        public Predicate Copy()
        {
            return new Predicate(StrRepresentation, Name, Args);
        }

        public static bool Equal(Predicate p1, Predicate p2)
        {
            if (p1 == null)
            {
                if (p2 == null)
                    return true;
                else return false;
            }
            if (p1.Name != p2.Name || p1.ArgNum != p2.ArgNum)
                return false;
            return true;
        }

    }
}
