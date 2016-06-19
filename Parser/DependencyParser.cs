using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
	public class DependencyParser
	{
		public enum Parsing { Nsubj, Compound, Nothing, Waiting };

		public static List<SemanticNetwork.Process> ParseDependencies(string text)
		{
			List<SemanticNetwork.Process> processes = new List<SemanticNetwork.Process>();
			text = text.Trim(new char[] { '\n' });
			text = text.Replace("\r", "");

			string[] rows = text.Split('\n');
			string dep, members;
			Parsing status = Parsing.Nothing;
			string subj1 = "", subj2 = "";
			int var_subj_index = 0;
			foreach (string row in rows)
			{
				if (row == "")
					continue;
				string[] temp = row.Split('(', ')');
				dep = temp[0];
				members = temp[1];
				#region Huge Switch Block
				switch (status)
				{
					case Parsing.Nothing:
						{
							if (dep == "nsubj" || dep == "compound")
							{
								subj1 = members.Split('-')[0];
								string temp_subj = members.Split(' ')[1].Split('-')[0];
								if (subj1 == temp_subj)
									break;
								if (dep == "nsubj")
									status = Parsing.Nsubj;
								else status = Parsing.Compound;
								if (subj2 == "" || (temp_subj != "he" && temp_subj != "she" && temp_subj != "it"))
									subj2 = temp_subj;
							}
							break;
						}
					case Parsing.Waiting:
						{
							switch (dep)
							{
								case "nsubj":
								case "compound":
									{
										subj1 = members.Split('-')[0];
										string temp_subj = members.Split(' ')[1].Split('-')[0];
										if (subj1 == temp_subj)
											break;
										if (dep == "nsubj")
											status = Parsing.Nsubj;
										else status = Parsing.Compound;
										if (subj2 == "" || (temp_subj != "he" && temp_subj != "she" && temp_subj != "it"))
											subj2 = temp_subj;
										break;
									}
								case "conj":
									{
										SemanticNetwork.Process temp_process = processes.Last();
										switch (var_subj_index)
										{
											case 0: processes.Add(new SemanticNetwork.Process(members.Split(' ')[1].Split('-')[0], temp_process.Action, temp_process.Target)); break;
											case 1: processes.Add(new SemanticNetwork.Process(temp_process.Object, members.Split(' ')[1].Split('-')[0], temp_process.Target)); break;
											case 2: processes.Add(new SemanticNetwork.Process(temp_process.Object, temp_process.Action, members.Split(' ')[1].Split('-')[0])); break;
										}
										status = Parsing.Nothing;
										break;
									}
								case "amod":
									{
										processes.Add(new SemanticNetwork.Process(subj2, members.Split(' ')[1].Split('-')[0], members.Split('-')[0]));
										status = Parsing.Nothing;
										break;
									}
							}
							break;
						}
					case Parsing.Nsubj:
						{
							switch (dep)
							{
								case "aux":
									{
										processes.Add(new SemanticNetwork.Process(subj2, members.Split(' ')[1].Split('-')[0], members.Split('-')[0]));
										status = Parsing.Waiting;
										var_subj_index = 1;
										break;
									}
								case "dobj":
									{
										processes.Add(new SemanticNetwork.Process(subj2, members.Split('-')[0], members.Split(' ')[1].Split('-')[0]));
										status = Parsing.Waiting;
										var_subj_index = 2;
										break;
									}
								case "cop":
									{
										processes.Add(new SemanticNetwork.Process(subj2, "is a", members.Split('-')[0]));
										status = Parsing.Waiting;
										var_subj_index = 2;
										break;
									}
								case "amod":
									{
										processes.Add(new SemanticNetwork.Process(subj2, subj1 + members.Split(',')[1].Split('-')[0], members.Split('-')[0]));
										status = Parsing.Waiting;
										var_subj_index = 2;
										break;
									}
									//case "nsubj":
									//    {
									//        processes.Add(new SemanticNetwork.Process(subj2, members.Split(',')[1].Split('-')[0], members.Split('-')[0]));
									//        status = Parsing.Nothing;
									//        break;
									//    }
									//case "compound":
									//    {
									//        processes.Add(new SemanticNetwork.Process(subj2, subj1, members.Split(' ')[1].Split('-')[0]));
									//        status = Parsing.Nothing;
									//        break;
									//    }
							}
							break;
						}
					case Parsing.Compound:
						{
							switch (dep)
							{
								case "dep":
									{
										processes.Add(new SemanticNetwork.Process(members.Split('-')[0], subj2, members.Split(' ')[1].Split('-')[0]));
										status = Parsing.Nothing;
										var_subj_index = 0;
										break;
									}
								case "nsubj":
									{
										processes.Add(new SemanticNetwork.Process(subj2, members.Split('-')[0], members.Split(' ')[1].Split('-')[0]));
										status = Parsing.Nothing;
										var_subj_index = 2;
										break;
									}
								case "compound":
									{
										processes.Add(new SemanticNetwork.Process(subj2, members.Split(' ')[1].Split('-')[0], members.Split('-')[0]));
										status = Parsing.Nothing;
										var_subj_index = 1;
										break;
									}
							}
							break;
						}
				}
				#endregion
			}
			return processes;
		}
	}
}
