using SemanticNetwork;
using SemanticNetwork.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartResearch
{
    public partial class AnalysisResults : System.Web.UI.Page
    {
        public static List<Process> processes = new List<Process>();
        protected void Page_Load(object sender, EventArgs e)
        {
            SNetwork net = new SNetwork(processes);

			string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KnowledgeBaseConnectionString"].ConnectionString;
			SemanticNetwork.KnowledgeBase.DataBase.CreateConnection(connectionString);
			SemanticNetwork.KnowledgeBase.DataBase.DataFromSemanticNetwork(net);
			SemanticNetwork.KnowledgeBase.DataBase.WriteToDataBase();
		}

        protected void ButtonShowGraph_Click(object sender, EventArgs e)
        {
			SNetwork snet = new SNetwork(processes);
            snet.SaveToPng(Server.MapPath("graph.png"));

            HyperLink link = new HyperLink();
            link.NavigateUrl = "graph.png";

            Image imgControl = new Image();
            imgControl.ImageUrl = "graph.png";
            imgControl.BorderStyle = BorderStyle.Groove;
            imgControl.ID = "Graph";
            imgControl.Visible = true;
            link.Controls.Add(imgControl);
            ResultsPlaceholder.Controls.Add(link);

            System.Diagnostics.Debug.WriteLine(snet.GetDefinition("Cat"));
        }

        protected void ButtonFindDefinition_Click(object sender, EventArgs e)
        {

        }

        protected void ButtonChangeDependencies_Click(object sender, EventArgs e)
        {

        }
    }
}