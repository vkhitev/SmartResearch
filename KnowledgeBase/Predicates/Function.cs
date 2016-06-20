using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeBase.Predicates
{
	/// <summary>
	/// Функция-предикат
	/// </summary>
    public class Function : Arg
    {
		/// <summary>
		/// Список аргументов функции
		/// </summary>
        private Arg[] _args;
        public Arg[] Args
        {
            get { return _args; }
        }

		/// <summary>
		/// Построить функцию по имени и аргументам
		/// </summary>
		/// <param name="name">Имя функции</param>
		/// <param name="args">Аргументы функции</param>
        public Function(string name, Arg[] args)
			: base(name, ArgType.Function)
        {
            _args = args.Clone() as Arg[];
        }

		/// <summary>
		/// Построить функцию по имени и списку строковых
		/// представлений аргументов
		/// </summary>
		/// <param name="name">Имя функции</param>
		/// <param name="args">Аргументы функции</param>
		public Function(string name, List<string> args)
			: base(name, ArgType.Function)
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