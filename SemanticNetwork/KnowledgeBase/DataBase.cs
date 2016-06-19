using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using SemanticNetwork.KnowledgeBase.Predicates;
using SemanticNetwork.Network;
using SemanticNetwork.KnowledgeBase.Expressions;

namespace SemanticNetwork.KnowledgeBase
{
	/// <summary>
	/// Класс для управления базой данных, содержащей базу знаний
	/// </summary>
    public static class DataBase
    {
		/// <summary>
		/// Список предикатов
		/// </summary>
        private static List<Predicate> _predicates = new List<Predicate>();
        public static List<Predicate> Predicates { get { return _predicates; } }

		/// <summary>
		/// Список аксиом
		/// </summary>
        private static List<Axiom> _axioms = new List<Axiom>();
        public static List<Axiom> Axioms { get { return _axioms; } }

		/// <summary>
		/// Список сущностей
		/// </summary>
        private static List<string> _instanses = new List<string>();
        public static List<string> Instanses { get { return _instanses; } }

		/// <summary>
		/// Заполнить объект данными из семантической сети
		/// </summary>
		/// <param name="net">Семантическая сеть</param>
        public static void DataFromSemanticNetwork(SNetwork net)
        {
            _predicates.Clear();
            _instanses.Clear();
            _axioms.Clear();

            _predicates.Add(new Predicate("x has value y", "HasVal", new Arg("x"), new Arg("y")));
            foreach (var n in net.Objects)
            {
                if (n.DataType == NodeType.Instance)
                    _instanses.Add(n.Name);
                else
                    _predicates.Add(new Predicate(String.Format("x is {0}", n.Name), n.Name, new Arg("x")));
            }
            foreach (var action in net.Actions)
            {
                if (GetPredicate(action.Name) == null)
                {
                    if (action.Target != null)
                        _predicates.Add(new Predicate(String.Format("x {0} y",action.Name),action.Name,new Arg("x"),new Arg("y")));
                    else _predicates.Add(new Predicate(String.Format("x {0}", action.Name), action.Name, new Arg("x")));
                }
                if (action.Value != "" && !_instanses.Contains(action.Value))
                    _instanses.Add(action.Value);
                
            }
            foreach (var action in net.Actions)
            {
                if (!action.IsDepended)
                {
                    Dictionary<string, string> args = new Dictionary<string, string>();
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb_args = new StringBuilder();
                    int arg_num = 0;
                    if (action.Name == "is a")
                    {
                        if (action.Object.DataType == NodeType.Object)
                        {
                            _axioms.Add(new Axiom(String.Format("Ɐx: {0}(x)→{1}(x)", action.Object.Name, action.Target.Name)));
                        }
                        else _axioms.Add(new Axiom(String.Format("{1}({0})", action.Object.Name, action.Target.Name)));

                    }
                    else
                    {
                        Edge curr = action;
                        do
                        {
                            if (!args.ContainsKey(curr.Object.Name))
                            {
                                if (curr.Object.DataType == NodeType.Object)
                                {
                                    args[curr.Object.Name] = String.Format("x{0}", arg_num);
                                    sb_args.Append(String.Format("Ɐx{0}", arg_num++));
                                    if (curr.IsDepended)
                                        sb.Insert(0,String.Format("{0}({1})˄", curr.Object.Name, args[curr.Object.Name]));
                                    else sb.Insert(0, String.Format("{0}({1})→(", curr.Object.Name, args[curr.Object.Name]));
                                }
                                else args[curr.Object.Name] = curr.Object.Name;
                            }
                            
                            if (curr.Target != null)
                            {
                                if (!args.ContainsKey(curr.Target.Name))
                                {
                                    if (curr.Target.DataType == NodeType.Object)
                                    {
                                        args[curr.Target.Name] = String.Format("x{0}", arg_num);
                                        sb_args.Append(String.Format("Ɐx{0}", arg_num++));
                                        sb.Insert(0,String.Format("{0}({1})˄", curr.Target.Name, args[curr.Target.Name]));
                                    }
                                    else args[curr.Target.Name] = curr.Target.Name;
                                }
                                sb.Append(String.Format("{0}({1},{2})", curr.Name, args[curr.Object.Name], args[curr.Target.Name]));
                                if (curr.Value != "")
                                    sb.AppendFormat("˄HasVal({0},{1})", curr.Target, curr.Value);
                            }
                            else sb.Append(String.Format("{0}({1})", curr.Name, args[curr.Object.Name]));

                            if (curr.ChainEdge != null)
                                sb.Append("→");
                            curr = curr.ChainEdge;
                        } while (curr != null);
                        sb.Insert(0, sb_args.ToString() + ":");
                        sb.Append(")");
                        _axioms.Add(new Axiom(sb.ToString()));
                    }
                }
            }
        }

		/// <summary>
		/// Создать таблицы базы данных и записать в них информацию
		/// </summary>
        public static void WriteToDataBase()
        {
            ClearTable("Axioms");
            ClearTable("Instanses");
            ClearTable("Predicates");

            for (int i = 0; i < Instanses.Count; i++)
                InsertInstanse(i, Instanses[i]);
            for (int i = 0; i < Predicates.Count; i++)
                InsertPredicate(i, Predicates[i]);
            for (int i = 0; i < Axioms.Count; i++)
                InsertAxiom(i, Axioms[i]);
        }

		/// <summary>
		/// Очистить таблицу базы данных
		/// </summary>
		/// <param name="table_name">Имя базы данных</param>
        public static void ClearTable(string table_name)
        {
            if (Connect())
            {
                using (SqlCommand command = new SqlCommand(String.Format("Delete {0}", table_name), data_base))
                    command.ExecuteNonQuery();
                data_base.Close();
            }
		}

		/// <summary>
		/// Строка подключения базы данных
		/// </summary>
        private static string connection_string;

		/// <summary>
		/// Собственно база данных
		/// </summary>
        private static SqlConnection data_base = new SqlConnection();

		/// <summary>
		/// Создать подключение по 
		/// </summary>
		/// <param name="core_name"></param>
        public static void CreateConnection(string core_name)
        {
            //SqlConnectionStringBuilder conect_build = new SqlConnectionStringBuilder();
            //conect_build.InitialCatalog = core_name;
            //conect_build.DataSource = Environment.UserDomainName;
            //conect_build.IntegratedSecurity = true;
            //connection_string = conect_build.ConnectionString;
            connection_string = core_name; // new
        }

		/// <summary>
		/// Подключиться к базе данных
		/// </summary>
		/// <returns></returns>
        private static bool Connect()
        {
            if (connection_string != null)
            {
                if (data_base.State == System.Data.ConnectionState.Closed)
                {
                    data_base.ConnectionString = connection_string;
                    data_base.Open();
                }
                return true;
            }
            else return false;
        }

		/// <summary>
		/// Считать все предикаты из базы данных
		/// </summary>
		/// <returns>Список предикатов</returns>
        public static List<Predicate> ReadAllPredicates()
        {
            if (Connect())
            {
                _predicates.Clear();
                SqlCommand command = new SqlCommand("Select * From Predicates;", data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        _predicates.Add(new Predicate(data["Name"].ToString(), data["Arguments"].ToString().Split(',', ';', ' '), data["Meaning"].ToString()));
                    }
                }
                data_base.Close();
                return _predicates;
            }
            else return null;
        }

		/// <summary>
		/// Получить предикат по идентификационному номеру
		/// </summary>
		/// <param name="id">Номер предиката</param>
		/// <returns>Найденный предикат</returns>
        public static Predicate GetPredicate(int id)
        {
            if (Connect())
            {
                Predicate predicate;
                SqlCommand command = new SqlCommand(String.Format("Select * From Predicates Where ID={0};", id), data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    data.Read();
                    predicate = new Predicate(data["Name"].ToString(), data["Arguments"].ToString().Split(',', ';', ' '), data["Meaning"].ToString());
                }
                data_base.Close();
                return predicate;
            }
            else return null;
        }

		/// <summary>
		/// Получить предикат по его имени
		/// </summary>
		/// <param name="name">Имя предиката</param>
		/// <returns>Предикат по его имени</returns>
        public static Predicate GetPredicate(string name)
        {
            foreach (Predicate pr in Predicates)
            {
                if (pr.Name == name)
                    return pr;
            }
            return null;
        }

		/// <summary>
		/// Добавить предикат в базу данных
		/// </summary>
		/// <param name="id">Номер предиката</param>
		/// <param name="predicate">Предикат</param>
        public static void InsertPredicate(int id, Predicate predicate)
        {
            if (Connect())
            {
                //_predicates[id] = predicate;
                StringBuilder sb = new StringBuilder("Insert Into Predicates(ID,Name,Arguments,Meaning) Values(");
                sb.Append(String.Format("'{0}',",id));
                sb.Append(String.Format("'{0}','", predicate.Name));
                for (int i = 0; i < predicate.ArgNum; i++)
                {
                    if (i < predicate.ArgNum - 1)
                        sb.Append(predicate.Args[i].Name + ", ");
                    else sb.Append(predicate.Args[i].Name+"',");
                }
                sb.Append(String.Format("'{0}')", predicate.StrRepresentation));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Обновить предикат
		/// </summary>
		/// <param name="id">Номер предиката</param>
		/// <param name="predicate">Новый предикат</param>
        public static void UpdatePredicate(int id, Predicate predicate)
        {
            if (Connect())
            {
                _predicates.Add(predicate);
                StringBuilder sb = new StringBuilder("Update Predicates Set ");
                sb.Append(String.Format("Name='{0}', ", predicate.Name));
                sb.Append("Arguments='");
                for (int i = 0; i < predicate.ArgNum; i++)
                {
                    if (i < predicate.ArgNum - 1)
                        sb.Append(predicate.Args[i].Name + ", ");
                    else sb.Append(predicate.Args[i].Name + "', ");
                }
                sb.Append(String.Format("Meaning='{0}' ", predicate.StrRepresentation));
                sb.Append(String.Format("Where ID={0};", id));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Удалить предикат по его номеру
		/// </summary>
		/// <param name="id">Номер предиката</param>
        public static void DeletePredicate(int id)
        {
            if (Connect())
            {
                Predicates.RemoveAt(id);
                using (SqlCommand command = new SqlCommand("DeletePredicate", data_base))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter()
                    {
                        ParameterName = "@index",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = id,
                        Direction = System.Data.ParameterDirection.Input
                    };
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    data_base.Close();
                }
            }
        }

		/// <summary>
		/// Считать все аксиомы из базы данных
		/// </summary>
		/// <returns>Список аксиом</returns>
        public static List<Axiom> ReadAllAxioms()
        {
            if (Connect())
            {
                _axioms.Clear();
                SqlCommand command = new SqlCommand("Select * From Axioms;", data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        _axioms.Add(new Axiom(data["Expresion"].ToString(),data["Description"].ToString()));
                    }
                }
                data_base.Close();
                return _axioms;
            }
            else return null;
        }

		/// <summary>
		/// Получить аксиому по номеру
		/// </summary>
		/// <param name="id">Номер аксиомы</param>
		/// <returns>Найденная аксиома</returns>
        public static Axiom GetAxiom(int id)
        {
            if (Connect())
            {
                Axiom ax;
                SqlCommand command = new SqlCommand(String.Format("Select * From Axioms Where ID={0};", id), data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    data.Read();
                    ax = new Axiom(data["Expresion"].ToString(), data["Description"].ToString());
                }
                data_base.Close();
                return ax;
            }
            else return null;
        }

		/// <summary>
		/// Вставить аксиому в базу данных
		/// </summary>
		/// <param name="id"></param>
		/// <param name="ax"></param>
        public static void InsertAxiom(int id, Axiom ax)
        {
            if (Connect())
            {
                StringBuilder sb = new StringBuilder("Insert Into Axioms(ID,Expresion,Description) Values(");
                sb.Append(String.Format("{0},", id));
                sb.Append(String.Format("N'{0}',", ax.Expresion));
                sb.Append(String.Format("'{0}')", ax.Description));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Обновить аксиому
		/// </summary>
		/// <param name="id">Номер аксиомы</param>
		/// <param name="ax">Новая аксиома</param>
        public static void UpdateAxiom(int id, Axiom ax)
        {
            if (Connect())
            {
                StringBuilder sb = new StringBuilder("Update Axioms Set ");
                sb.Append(String.Format("Expresion=N'{0}', ", ax.Expresion));
                sb.Append(String.Format("Description='{0}' ", ax.Description));
                sb.Append(String.Format("Where ID={0};", id));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Удалить аксиому
		/// </summary>
		/// <param name="id">Номер аксиомы</param>
        public static void DeleteAxiom(int id)
        {
            if (Connect())
            {
                using (SqlCommand command = new SqlCommand("DeleteAxiom", data_base))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter()
                    {
                        ParameterName = "@index",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = id,
                        Direction = System.Data.ParameterDirection.Input
                    };
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    data_base.Close();
                }
            }
        }

		/// <summary>
		/// Считать все сущности
		/// </summary>
		/// <returns>Список сущностей</returns>
        public static List<Axiom> ReadAllInstanses()
        {
            if (Connect())
            {
                _axioms.Clear();
                SqlCommand command = new SqlCommand("Select * From Instanses;", data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        _instanses.Add(data["Name"] as string);
                    }
                }
                data_base.Close();
                return _axioms;
            }
            else return null;
        }

		/// <summary>
		/// Получить сущность
		/// </summary>
		/// <param name="id">Номер сущности</param>
		/// <returns></returns>
        public static string GetInstanse(int id)
        {
            if (Connect())
            {
                string inst;
                SqlCommand command = new SqlCommand(String.Format("Select * From Instanses Where ID={0};", id), data_base);
                using (SqlDataReader data = command.ExecuteReader())
                {
                    data.Read();
                    inst = data["Name"] as string;
                }
                data_base.Close();
                return inst;
            }
            else return null;
        }

		/// <summary>
		/// Добавить сущность
		/// </summary>
		/// <param name="id">Номер сущности</param>
		/// <param name="inst">Сущность</param>
        public static void InsertInstanse(int id, string inst)
        {
            if (Connect())
            {
                StringBuilder sb = new StringBuilder("Insert Into Instanses(ID,Name) Values(");
                sb.Append(String.Format("{0},", id));
                sb.Append(String.Format("'{0}')", inst));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Обновить сущность
		/// </summary>
		/// <param name="id">Номер сущности</param>
		/// <param name="inst">Сущность</param>
        public static void UpdateInstanse(int id, string inst)
        {
            if (Connect())
            {
                StringBuilder sb = new StringBuilder("Update Instanses Set ");
                sb.Append(String.Format("Name='{0}' ", inst));
                sb.Append(String.Format("Where ID={0};", id));
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), data_base))
                    cmd.ExecuteNonQuery();
                data_base.Close();
            }
        }

		/// <summary>
		/// Удалить сущность
		/// </summary>
		/// <param name="id">Номер сущности</param>
        public static void DeleteInstanse(int id)
        {
            if (Connect())
            {
                using (SqlCommand command = new SqlCommand("DeleteInstanse", data_base))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter()
                    {
                        ParameterName = "@index",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Value = id,
                        Direction = System.Data.ParameterDirection.Input
                    };
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    data_base.Close();
                }
            }
        }

		/// <summary>
		/// Считать все объекты из базы данных
		/// </summary>
        public static void ReadAllCore()
        {
            ReadAllInstanses();
            ReadAllPredicates();
            ReadAllAxioms();
        }
    }
}
