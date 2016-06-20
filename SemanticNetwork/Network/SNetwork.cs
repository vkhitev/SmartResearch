using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticNetwork.Network
{
	/// <summary>
	/// Семантическая сеть
	/// </summary>
    public class SNetwork
    {
		/// <summary>
		/// Действия (рёбра графа)
		/// </summary>
        public List<Edge> Actions { get; private set; }

		/// <summary>
		/// Объекты (вершины графа)
		/// </summary>
        public List<Node> Objects { get; private set; }

		/// <summary>
		/// Найти вершину по имени
		/// </summary>
		/// <param name="name">Имя вершины</param>
		/// <returns>Найденная вершина</returns>
        public Node FindNode(string name)
        {
            return Objects.Find(new Predicate<Node>(x => { return name == x.Name; }));
        }

		/// <summary>
		/// Найти ребро по имени
		/// </summary>
		/// <param name="name">Имя ребра</param>
		/// <returns>Найденное ребро</returns>
        public Edge FindEdge(string name)
        {
            return Actions.Find(new Predicate<Edge>(x => { return name == x.Name; }));
        }

		/// <summary>
		/// Построить семантическую сеть по списку процессов
		/// </summary>
		/// <param name="processes">Список процессов</param>
        public SNetwork(IEnumerable<Process> processes)
        {
            Actions = new List<Edge>();
            Objects = new List<Node>();

            foreach (Process process in processes)
            {
                Node node = FindNode(process.Object);
                if (node == null)
                {
                    node = new Node(process.Object);
                    Objects.Add(node);
                }

                Node target = FindNode(process.Target);
                if (target == null)
                {
                    target = new Node(process.Target);
                    Objects.Add(target);
                }

                Edge edge = new Edge(process.Action, node, target);
                if (process.Action.Length > 3 && process.Action.Substring(0, 3) == "has")
                {
                    edge.Name = "has";
                    edge.Value = process.Action.Substring(4);
                }
                Actions.Add(edge);


                #region Add Condition Process
                Process temp = process;
                while (temp.Condition != null)
                {
                    temp = temp.Condition;
                    node = FindNode(temp.Object);
                    if (node == null)
                    {
                        node = new Node(temp.Object);
                        Objects.Add(node);
                    }
                    if (temp.Target != null)
                    {
                        target = FindNode(temp.Target);
                        if (target == null)
                        {
                            target = new Node(temp.Target);
                            Objects.Add(target);
                        }
                        edge = new Edge(temp.Action, node, target, edge);
                        Actions.Add(edge);
                    }
                    else
                    {
                        edge = new Edge(temp.Action, node, null, edge);
                        Actions.Add(edge);
                    }
                }
                #endregion
            }
        }

		/// <summary>
		/// Сохранить изображение графа семантической сети
		/// </summary>
		/// <param name="path">Путь к файлу</param>
        public void SaveToPng(string path)
        {
            Dictionary<Node, PointF> nodes = new Dictionary<Node, PointF>(); // Координаты вершин
            Dictionary<Edge, PointF> edges = new Dictionary<Edge, PointF>(); // Координаты середин рёбер
            using (Bitmap b = new Bitmap(1920, 1080))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    ///Розміщення вершин по колу
                    PointF center = new Point(960, 540);
                    float radius = 400;
                    float d_angle = (float)(2 * Math.PI / Objects.Count); ///< кут між вершинами
                    float angle = 0;
                    g.Clear(Color.White);
                    foreach (Node n in Objects)
                    {
                        PointF p = new PointF(radius * (float)Math.Cos(angle) + center.X, radius * (float)Math.Sin(angle) + center.Y);
                        nodes[n] = p;
                        angle += d_angle;
                    }

                    //Малюваня ребер
                    foreach (Edge edge in Actions)
                    {
                        PointF m = new PointF(); // середина ребра
                        PointF l = new PointF(); // кінець ребра
                        AdjustableArrowCap bigArrow = new AdjustableArrowCap(8, 8);

                        PointF target_point; // точка яку треба з'єднати
                        Color color = Color.LightGray;
                        if (edge.Target == null)
                        {
                            target_point = edges[edge.ChainEdge]; // ребро (подія-умова)
                            color = Color.Gold;
                        }
                        else target_point = nodes[edge.Target]; // вершина
                        if (edge.Value != "") // константа
                            color = Color.Green;
                        m.X = (nodes[edge.Object].X + target_point.X) / 2;
                        m.Y = (nodes[edge.Object].Y + target_point.Y) / 2;

                        #region Вирахування траєкторії з врахуванням можливості збігу траєкторії декількох ребер
                        float k = -(nodes[edge.Object].X - target_point.X) / (nodes[edge.Object].Y - target_point.Y);
                        float c = -k * m.X + m.Y;
                        int i = 1;
                        while (edges.ContainsValue(m))
                        {
                            float x, y;
                            if (Math.Abs(k) < 1)
                            {
                                y = m.Y - i * Math.Abs(k) * 25;
                                x = (y - c) / k;
                            }
                            else
                            {
                                x = m.X + i * (25 / Math.Abs(k));
                                y = x * k + c;
                            }
                            m = new PointF(x, y);
                            if (i < 0)
                                i += i;
                            i *= -1;
                        }
                        edges[edge] = m;
                        #endregion

                        #region відкидання 1/15 відрізку від кінця
                        l.X = (nodes[edge.Object].X + 15 * target_point.X) / 16;
                        l.Y = (nodes[edge.Object].Y + 15 * target_point.Y) / 16;
                        g.DrawLines(new Pen(color, 1) { CustomEndCap = bigArrow }, new PointF[3] { nodes[edge.Object], m, l });
                        g.DrawString(String.Format("{0} {1}", edge.Name, edge.Value), new Font("TimesNewRoman", 12), Brushes.Red, m, stringFormat);

                        if (edge.Target != null && edge.ChainEdge != null) // (у випадку події-умови з ціллю)
                        {
                            target_point = edges[edge.ChainEdge];
                            l.X = (m.X + 18 * target_point.X) / 19;
                            l.Y = (m.Y + 18 * target_point.Y) / 19;
                            g.DrawLine(new Pen(Color.Gold, 1) { CustomEndCap = bigArrow }, m, l);
                        }
                        #endregion
                    }

                    // малювання вершин
                    foreach (Node obj in Objects)
                    {
                        g.DrawString(obj.Name, new Font("TimesNewRoman", 12), Brushes.Blue, nodes[obj], stringFormat);
                        //g.DrawRectangle(Pens.Gold, nodes[obj].X - 50, nodes[obj].Y - 10, 100, 20);
                    }
                }
                b.Save(path, ImageFormat.Png);
            }
        }

        public string GetDefinition(string name)
        {
            Node n = FindNode(name);
            try
            {
                if (n.OutEdges.Count == 0)
                    return name + " already has definition";
                else
                {
                    StringBuilder def = new StringBuilder(name);
                    StringBuilder is_str = new StringBuilder();
                    StringBuilder has_str = new StringBuilder();
                    StringBuilder act_str = new StringBuilder();
                    bool has_is = false, has_has = false, has_act = false;
                    foreach (Edge edge in n.OutEdges)
                    {
                        switch (edge.DataType)
                        {
                            case EdgeType.Is:
                                {
                                    is_str.AppendFormat(" is {0}", edge.Target.Name);
                                    has_is = true;
                                    break;
                                }
                            case EdgeType.Has:
                                {
                                    if (has_has)
                                        has_str.AppendFormat(", {0} {1}", edge.Value, edge.Target.Name);
                                    else has_str.AppendFormat(" has {0} {1}", edge.Value, edge.Target.Name);
                                    has_has = true;
                                    break;
                                }
                            default:
                                {
                                    if (edge.Target == null)
                                        break;
                                    if (has_act)
                                        act_str.Append(", ");
                                    act_str.AppendFormat(" {0} {1}", edge.Name, edge.Target.Name);
                                    has_act = true;
                                    break;
                                }
                        }
                    }
                    if (has_is && (has_has || has_act))
                        def.Append(is_str.ToString() + " which");
                    else def.Append(is_str.ToString());
                    if (has_has)
                        def.Append(has_str.ToString());
                    if (has_has && has_act)
                        def.Append(" and");
                    if (has_act)
                        def.Append(act_str.ToString());
                    return def.ToString().Replace("  ", " ");
                }
            }
            catch (NullReferenceException)
            {
                return "Object Not Found";
            }
        }

    }
}
