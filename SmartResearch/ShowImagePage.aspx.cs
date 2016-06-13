using SmartResearch.SemanticNetwork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartResearch
{
    public partial class ShowImagePage : System.Web.UI.Page
    {
        private List<Process> ParseToProcesses(string text)
        {
            List<Process> list = new List<Process>();

            text = text.Trim(new char[] { '\n' });
            text = text.Replace("\r", "");

            string[] rows = text.Split('\n');

            foreach (string row in rows)
            {
                string[] msg = row.Split(' ');

                if (msg.Length == 4)
                {
                    msg[1] = msg[1] + " " + msg[2];
                    msg[2] = msg[3];
                }

                list.Add(new Process(msg[0], msg[1], msg[2]));
            }

            return list;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string message = Application["msg"] as string;
            //string message = Request.QueryString["msg"];
            if (message == null)
            {
                Response.Redirect("/App.aspx");
            }

            List<Process> pr1 = new List<Process> { new Process("Animal","has","Brain"),
                                                    new Process("Animal","live","Life"),
                                                    new Process("Cat","has","Claws"),
                                                    new Process("Cat","has","Fur"),
                                                    new Process("Cat","has","Size"),
                                                    new Process("Fur", "has", "Color"),
                                                    new Process("Cat","has","Tail"),
                                                    new Process("Cat","is a","Animal"),
                                                    new Process("Tiger","is a","Cat"),
                                                    new Process("Mouse","is a","Animal"),
                                                    new Process("Mouse","has","Tail"),
                                                    new Process("Mouse","has small","Size"),
                                                    new Process("Cat","hunt","Mouse", new Process("Cat","starving", null)) };

            pr1 = ParseToProcesses(message);

            SemanticNetwork.Network.SNetwork snet = new SemanticNetwork.Network.SNetwork(pr1);
            snet.SaveToPng(Server.MapPath("image2.png"));

            System.Diagnostics.Debug.WriteLine(snet.GetDefinition("Cat"));
        }
    }
}