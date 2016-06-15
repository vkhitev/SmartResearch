using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartResearch.SemanticNetwork.Network
{
    public enum EdgeType { Is,Has,Other}
    public class Edge
    {
        public string Name { get; set; }
        //public List<string> Subjects { get; set; }
        public EdgeType DataType { get; private set; }
        public Node Target { get; set; }
        public Node Object { get; set; }
        public Edge ChainEdge { get; set; } ///< подія яка починається після даної
        public bool IsDepended { get; set; }

        public string Value { get; set; }

        public Edge(string name, Node from, Node to, Edge chain = null/*, params string[] subjects*/)
        {
            IsDepended = false;
            Name = name;
            Object = from;
            Object.OutEdges.Add(this);
            Target = to;
            if (Target != null)
                Target.InEdges.Add(this);
            //Subjects = subjects.ToList();
            ChainEdge = chain;
            if (ChainEdge != null)
                ChainEdge.IsDepended = true;
            switch (name)
            {
                case "is a": DataType = EdgeType.Is; break;
                case "has": DataType = EdgeType.Has; break;
                default:
                    DataType = EdgeType.Has; break;
                //default: {
                //        if (name.Substring(0, 4) == "has ")
                //            DataType = EdgeType.Has;
                //        else DataType = EdgeType.Other;
                //        break;
                //    }
            }
            Value = "";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0} {1}", Object.Name, Name));
            if (Target != null)
                sb.Append(" " + Target.Name);
            return sb.ToString();
        }
    }
}
