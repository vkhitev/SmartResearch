using SmartResearch.KnowledgeBase.Predicates;
using System.Collections.Generic;
using System.Text;
namespace SmartResearch.KnowledgeBase.Expressions
{
    public enum Operation { Conjunction = 1, Disjunction, Implication, Equivalent, NotAnOperation }

    public class Node
    {
        private Operation _op;
        public Operation @Operation
        {
            get { return _op; }
            set {
                empty = false;
                Predicate = null;
                if (Right == null)
                    Right = new Node(this);
                if (Left == null)
                    Left = new Node(this);
                _op = value;
            }
        }
        private Predicate _predicate;
        public Predicate @Predicate
        {
            get { return _predicate; }
            set
            {
                empty = false;
                _op = Operation.NotAnOperation;
                Right = null;
                Left = null;
                _predicate = value;
            }
        }

        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }

        private bool empty;
        public bool IsEmpty {
            get { return empty; }
        }

        public Node(Operation op, Node parent = null)
        {
            Parent = parent;
            _predicate = null;
            _op = op;
            empty = false;
            Right = new Node(this);
            Left = new Node(this);
        }

        public Node(Predicate pr, Node parent = null)
        {
            Parent = parent;
            _predicate = pr;
            _op = Operation.NotAnOperation;
            empty = false;
        }

        public Node(Node parent = null)
        {
            Parent = parent;
            _predicate = null;
            _op = Operation.NotAnOperation;
            empty = true;
        }

        public void SetArgs(Node n)
        {
            if (Predicate != null)
            {
                Arg[] temp = new Arg[n.Predicate.ArgNum];
                for (int i = 0; i < Predicate.ArgNum; i++)
                {
                    temp[i] = n.Predicate.Args[i].Copy();
                    if (Predicate.Args[i].Name.Length < 10 || Predicate.Args[i].Name.Substring(0, 10) != "unique_arg")
                    {
                        Arg a = temp[i].Copy();
                        temp[i].Set(Predicate.Args[i]);
                        for (int j = 0; j < i; j++)
                            if (temp[j] == a)
                                temp[j].Set(Predicate.Args[i]);
                    }
                }
                Predicate.Args = temp;
            }
            else
            {
                Left.SetArgs(n.Left);
                Right.SetArgs(n.Right);
            }
        }

        public static bool NodeEqual(Node n1, Node n2)
        {
            if (n1 == null)
            {
                if (n2 == null)
                    return true;
                else return false;
            }
            if (!Predicate.Equal(n1.Predicate, n2.Predicate))
                return false;
            if (n1.Operation != n2.Operation)
                return false;
            return true;
        }

        private static bool Equal(Node n1, Node n2)
        {
            bool result = false;
            if (Node.NodeEqual(n1, n2))
            {
                result = true;
                if (n1.Left != null)
                    result = Equal(n1.Left, n2.Left);
                if (n1.Right != null)
                    result = Equal(n1.Right, n2.Right);
            }
            return result;
        }

        public Node Copy(Node parent)
        {
            if (Operation == Operation.NotAnOperation)
                return new Node(Predicate.Copy(), parent);
            else
            {
                Node temp = new Node(_op, parent);
                temp.Left = this.Left.Copy(temp);
                temp.Right = this.Right.Copy(temp);
                return temp;
            }
        }

        public override string ToString()
        {
            if (Operation == Operation.NotAnOperation)
                return Predicate.StrRepresentation;
            else switch (Operation)
                {
                    case Operation.Implication: { return '\u2192'.ToString(); }
                    case Operation.Conjunction: { return '\u02C4'.ToString(); }
                    case Operation.Disjunction: { return '\u02C5'.ToString(); }
                    case Operation.Equivalent: { return '\u2194'.ToString(); }
                    default: return "";
                };
        }

        public static explicit operator ExpressionTree(Node n)
        {
            Node curr = n;
            while (curr.Parent != null)
                curr = curr.Parent;
            return new ExpressionTree(curr);
        }
    } 

    public class ExpressionTree
    {
        private Node _root = new Node();
        public Node Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public ExpressionTree CurrentPosition;

        public ExpressionTree(string expression, List<Predicate> Predicates)
        {
            #region To Polski record
            Stack<int> operations = new Stack<int>();
            List<int> pol_record = new List<int>();
            int number = -1;
            foreach (var v in expression)
            {
                if (!char.IsDigit(v) && number != -1)
                {
                    pol_record.Add(number);
                    number = -1;
                }
                switch (v)
                {
                    case (char)40: {operations.Push(10); break; }
                    case (char)41:
                        {
                            while (operations.Peek() != 10)
                            { pol_record.Add(-1*operations.Pop());}
                            operations.Pop();
                            break;
                        }
                    case (char)708: { operations.Push(1); break; }
                    case (char)709:
                        {
                            while (operations.Count > 0 && operations.Peek() == 1)
                                pol_record.Add(-1*operations.Pop());
                            operations.Push(2);
                            break;
                        }
                    case (char)8594:
                        {
                            while (operations.Count > 0 && operations.Peek() < 3)
                                pol_record.Add(-1 * operations.Pop());
                            operations.Push(3);
                            break;
                        }
                    case (char)8596:
                        {
                            while (operations.Count > 0 && operations.Peek() < 4)
                                pol_record.Add(-1 * operations.Pop());
                            operations.Push(4);
                            break;
                        }
                    default: {
                            if (number == -1)
                                number = int.Parse(v.ToString());
                            else number = number * 10 + int.Parse(v.ToString());
                            break;
                        }
                }
            }
            if (number != -1)
                pol_record.Add(number);
            while (operations.Count > 0)
                pol_record.Add(-1 * operations.Pop());
            pol_record.Reverse();
            #endregion

            #region To Tree
            Node previous = null;
            Node current = _root;
            foreach (int val in pol_record)
            {
                if (val < 0)
                {
                    if (!current.IsEmpty)
                    {
                        if (current.Right.IsEmpty)
                            current = current.Right;
                        else current = current.Left;
                    }
                    current.Operation = (Operation)(-1 * val);
                }
                else
                {
                    if (current.IsEmpty)
                        current.Predicate = Predicates[val];
                    else if (current.Right.IsEmpty)
                        current.Right.Predicate = Predicates[val];
                    else if (current.Left.IsEmpty)
                        current.Left.Predicate = Predicates[val];
                    else
                    {
                        current = current.Parent;
                        while (!current.Left.IsEmpty)
                            current = current.Parent;
                        current.Left.Predicate = Predicates[val];
                        previous = current.Parent;
                    }
                }
            }
            #endregion
        }

        public ExpressionTree(Node root)
        {
            _root = root.Copy(null);
        }

        private void GoThrough(Node curr, ref StringBuilder sb)
        {
            if (curr == null)
                return;
            GoThrough(curr.Left, ref sb);
            sb.Append(curr.ToString());
            GoThrough(curr.Right, ref sb);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            GoThrough(Root, ref sb);
            return sb.ToString();
        }

    }
}
