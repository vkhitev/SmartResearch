using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartResearch.KnowledgeBase.Predicates
{
    public enum ArgType {Variable,Constatnt,Function }
    public class Arg
    {
        public string Name { get; set; }

        protected ArgType _type;
        public ArgType Type
        { get { return _type; } }

        public char TypeSymbol
        {
            get
            {
                if (_type == ArgType.Variable)
                    return '\u2C6F';
                else return '\u018E';
            }
        }

        public Arg(string name, ArgType type = ArgType.Variable)
        {
            Name = name;
            _type = type;
        }

        public Arg()
        {
            _type = ArgType.Variable;
            Name = "arg";
        }

        public override string ToString()
        {
            return Name;
        }

        public void Set(Arg a)
        {
            Name = a.Name;
            _type = a.Type;
        }

        public Arg Copy()
        {
            return this.MemberwiseClone() as Arg;
        }
    }
}
