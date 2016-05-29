using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using KnowledgeBase.Predicates;

namespace KnowledgeBase.Expressions
{
    public class Axiom : Term
    {
        public string Description;

        public string CurrentExpression
        {
            get
            {
                return Expresion.Split(':').Last();
            }
        }

        public Axiom(string expresion, string description = null) : base(expresion)
        {
            Description = description;
        }

        public override string ToString()
        {
            return Expresion;
        }
    }
}
