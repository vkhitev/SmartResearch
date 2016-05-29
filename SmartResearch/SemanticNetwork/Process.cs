using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticNetwork
{
    public class Process
    {
        private string _object; ///< об'єкт, що виконує опрецію
        public string @Object
        {
            get { return _object; }
        }

        private string _action; ///< подія (операція)
        public string @Action
        {
            get { return _action; }
        }

        private string _target; ///< об'єкт на якому зосереджена подія (операція)
        public string Target
        {
            get { return _target; }
        }

        private Process _condition_process; ///< процесс, який повинен відбутись до початку виконання поточного
        public Process Condition
        {
            get { return _condition_process; }
        }

        public Process(string obj, string act, string target, Process condition = null)
        {
            _object = obj;
            _action = act;
            _target = target;
            _condition_process = condition;
        }
    }
}
