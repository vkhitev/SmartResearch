﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using KnowledgeBase.Predicates;

namespace KnowledgeBase.Expressions
{
    public class Term
    {
        public static Regex args_reg_exp = new Regex(@"[ⱯƎ][^ⱯƎ:]+");
        public static Regex predicates_args_reg_exp = new Regex(@"(?<=\()[^˄˅¬→↔\(\)]+(?=\))");
        public static Regex def_predicates_name_reg_exp = new Regex(@"[^:\(˄˅¬→↔\s]+(?=\()");
        public static Regex predicates_name_reg_exp = new Regex(@"[^:\(˄˅→↔\s]+(?=\()");

        protected string _expresion;
        public string Expresion
        {
            get
            {
                StringBuilder output = new StringBuilder(_expresion);
                for (int i = 0; i < Args.Count; i++)
                {
                    output = output.Replace(String.Format("unique_arg{0}", i), Args[i].Name);
                }
                return output.ToString();
            }
        }

        public List<Arg> Args = new List<Arg>();

        protected List<Predicate> _predicates = new List<Predicate>();
        public List<Predicate> Predicates { get { return _predicates; } }

        private ExpressionTree _logic_tree;
        public ExpressionTree LogicTree { get { return _logic_tree; } }

        public Predicate GetPredicate(string name)
        {
            foreach (Predicate pr in Predicates)
            {
                if (pr.Name == name)
                    return pr;
            }
            return null;
        }

        public string ReplacePredicatesWithIndexes(string expresion)
        {
            string exp = expresion.Split(':').Last();
            for (int i = 0; i < Predicates.Count; i++)
            {
                exp = exp.Replace(Predicates[i].ToString(), i.ToString());
            }
            return exp.Replace(" ", "");
        }

        public Arg GetArg(string name)
        {
            foreach (var arg in Args)
            {
                if (arg.Name == name)
                    return arg;
            }
            foreach (string inst in DataBase.Instanses)
                if (name == inst)
                {
                    Arg arg = new Arg(name, ArgType.Constatnt);
                    Args.Add(arg);
                    return arg;
                }
            throw new FormatException();
        }

        private void Parse(string str)
        {
            try
            {
                #region Args Parse
                List<string> prev_vars = new List<string>();
                foreach (Match m in args_reg_exp.Matches(str))
                {
                    string temp = m.ToString();
                    ArgType type;
                    if (temp[0] == '\u2C6F')
                        type = ArgType.Variable;
                    else if (prev_vars.Count == 0)
                        type = ArgType.Constatnt;
                    else type = ArgType.Function;
                    temp = temp.Substring(1).Replace(" ", "");
                    string[] args_names = temp.Split(',');
                    foreach (string arg_name in args_names)
                    {
                        if (type == ArgType.Function)
                        {
                            Function func = new Function(arg_name, prev_vars);
                            Args.Add(func);
                        }
                        else {
                            Arg arg = new Arg(arg_name, type);
                            Args.Add(arg);
                            if (type == ArgType.Variable)
                                prev_vars.Add(arg_name);
                        }
                    }
                }
                #endregion

                #region Predicate Parse
                MatchCollection pr_names = predicates_name_reg_exp.Matches(str);
                MatchCollection pr_args = predicates_args_reg_exp.Matches(str);
                bool not = false;
                for (int i = 0; i < pr_names.Count; i++)
                {
                    string name = pr_names[i].ToString();
                    if (name[0] == '¬')
                    {
                        not = true;
                        name = name.Substring(1);
                    }
                    else not = false;
                    Predicate pr = DataBase.GetPredicate(name).Copy();
                    string[] args = pr_args[i].ToString().Replace(" ", "").Split(',');
                    for (int j = 0; j < args.Length; j++)
                    {
                        pr.Args[j] = GetArg(args[j]);
                    }
                    if (not)
                        pr.IsNot = true;
                    _predicates.Add(pr);
                }
                #endregion
            }
            catch (Exception)
            {
                throw new FormatException("Failed To Parse:\n" + str);
            }
        }

        /// <summary>
        /// Замінює аргументи на шаблонні
        /// </summary>
        /// <param name="str">вираз</param>
        /// <returns>шаблонний вираз</returns>
        private string GetArgShablon(string str)
        {
            string pattern;
            string expr = str;
            for (int i = 0; i < Args.Count; i++)
            {
                if (!DataBase.Instanses.Contains(Args[i].Name))
                {
                    pattern = String.Format(@"(?<=[ⱯƎ\W]){0}(?=[ⱯƎ\W])", Args[i].Name);
                    expr = Regex.Replace(expr, pattern, String.Format("unique_arg{0}", i));
                }
            }
            return expr;
        }

        public Term(string expresion)
        {
            Parse(expresion);
            _expresion = GetArgShablon(expresion);
            _logic_tree = new ExpressionTree(ReplacePredicatesWithIndexes(expresion), Predicates);

        }

        public Term GetUnformShablon()
        {
            return new Term(this._expresion);
        }

        public override string ToString()
        {
            return Expresion;
        }
    }
}
