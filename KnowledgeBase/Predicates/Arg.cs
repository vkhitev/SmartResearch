using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeBase.Predicates
{
	/// <summary>
	/// Тип аргумента предиката
	/// </summary>
    public enum ArgType
	{
		Variable,
		Constatnt,
		Function
	}

	/// <summary>
	/// Аргумент предиката
	/// </summary>
    public class Arg
    {
		/// <summary>
		/// Строковое представление аргумента
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Тип аргумента
		/// </summary>
        protected ArgType _type;
        public ArgType Type
        { get { return _type; } }

		/// <summary>
		/// Символьное представление предиката
		/// </summary>
        public char TypeSymbol
        {
            get
            {
                if (_type == ArgType.Variable)
                    return '\u2C6F';
                else return '\u018E';
            }
        }

		/// <summary>
		/// Построить аргумент по имени и типу
		/// </summary>
		/// <param name="name">Имя аргумента</param>
		/// <param name="type">Тип аргумента</param>
        public Arg(string name, ArgType type = ArgType.Variable)
        {
            Name = name;
            _type = type;
        }

		/// <summary>
		/// Построить пустой аргумент типа "переменная"
		/// </summary>
        public Arg()
        {
            _type = ArgType.Variable;
            Name = "arg";
        }

        public override string ToString()
        {
            return Name;
        }

		/// <summary>
		/// Скопировать аргумент из другого
		/// </summary>
		/// <param name="a">Источник, из которого происходит копирование</param>
        public void Set(Arg a)
        {
            Name = a.Name;
            _type = a.Type;
        }

		/// <summary>
		/// Вернуть копию аргумента
		/// </summary>
		/// <returns>Копия аргумента</returns>
        public Arg Copy()
        {
            return this.MemberwiseClone() as Arg;
        }
    }
}
