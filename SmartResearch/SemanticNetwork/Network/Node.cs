using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticNetwork.Network
{
    public enum NodeType { Object, Instance }; ///< Класс або об'єкт(екземпляр) класу
    public class Node
    {
        public string Name { get; set; }
        public List<Edge> InEdges { get; set; } ///< ребра що входять у вершину
        public List<Edge> OutEdges { get; set; } ///< ребра що виходять у вершину
        public NodeType DataType
        {
            get
            {
                foreach (Edge edge in InEdges)
                {
                    if (edge.DataType == EdgeType.Is)
                        return NodeType.Object;
                }
                foreach (Edge edge in OutEdges)
                {
                    if (edge.DataType != EdgeType.Is)
                        return NodeType.Object;
                }
                return NodeType.Instance;
            }
        }
        public int EdgeCount
        {
            get { return InEdges.Count + OutEdges.Count; }
        }

        public Node(string name)
        {
            Name = name;
            InEdges = new List<Edge>();
            OutEdges = new List<Edge>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
