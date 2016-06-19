using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticNetwork
{
	/// <summary>
	/// Процесс, состоящий из объекта, действия
	/// и цели, на которую направленно действия
	/// </summary>
    public class Process
    {
		/// <summary>
		/// Объект, исполняющий действие
		/// </summary>
        private string _object;
        public string @Object
        {
            get { return _object; }
        }

		/// <summary>
		/// Действие (операция)
		/// </summary>
		private string _action;
        public string @Action
        {
            get { return _action; }
        }

		/// <summary>
		/// Цель действия (операции)
		/// </summary>
        private string _target;
        public string Target
        {
            get { return _target; }
        }

		/// <summary>
		/// Процесс, который должен произойти до начала исполнения текущего процесса
		/// </summary>
        private Process _condition_process;
        public Process Condition
        {
            get { return _condition_process; }
        }

		/// <summary>
		/// Создаёт процесс
		/// </summary>
		/// <param name="obj">Объект</param>
		/// <param name="act">Действие</param>
		/// <param name="target">Цель действия</param>
		/// <param name="condition">Условный процесс</param>
        public Process(string obj, string act, string target, Process condition = null)
        {
            _object = obj;
            _action = act;
            _target = target;
            _condition_process = condition;
        }
    }
}
