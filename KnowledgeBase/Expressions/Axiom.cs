using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace KnowledgeBase.Expressions
{
	/// <summary>
	/// Аксиома, содержащая выражение в строковом виде и его описание.
	/// </summary>
    public class Axiom : Term
    {
		/// <summary>
		/// Описание аксиомы.
		/// </summary>
        public string Description;

		/// <summary>
		/// Получить текущее выражение.
		/// </summary>
        public string CurrentExpression
        {
            get
            {
                return Expresion.Split(':').Last();
            }
        }

		/// <summary>
		/// Создаёт аксиому по заданному выражению и возможно его описанию.
		/// </summary>
		/// <param name="expresion">Выражение</param>
		/// <param name="description">Описание выражения</param>
        public Axiom(string expresion, string description = null)
			: base(expresion)
        {
            Description = description;
        }

        public override string ToString()
        {
            return Expresion;
        }
    }
}
