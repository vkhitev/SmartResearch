using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace SmartResearch
{
    public partial class App : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Application.Clear();
        }

        protected void ButtonNextStep_Click(object sender, EventArgs e)
        {
            string text = TextBox1.Text;
            Application.Add("msg", text);
            //Response.Redirect("/ShowImagePage.aspx");
            Response.Redirect("/AnalysisResults.aspx");
        }

        protected void ButtonParseText_Click(object sender, EventArgs e)
        {
            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("query", TextBox1.Text);
            string postData = outgoingQueryString.ToString();

            var httpRequest = (HttpWebRequest)WebRequest.Create("http://nlp.stanford.edu:8080/parser/index.jsp");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] dataArray = Encoding.UTF8.GetBytes(postData);
            httpRequest.ContentLength = dataArray.Length;
            HttpWebResponse webResponse;
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                requestStream.Write(dataArray, 0, dataArray.Length);
                webResponse = (HttpWebResponse)httpRequest.GetResponse();
            }

            WebHeaderCollection header = webResponse.Headers;

            var encoding = ASCIIEncoding.ASCII;
            string responseText = "";
            using (var reader = new System.IO.StreamReader(webResponse.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseText);

            var findclasses = doc.DocumentNode.Descendants("div").Where(d =>
                d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("parserOutput"));

            var nodes = findclasses.ToArray();

            var untrimmedText = nodes[3].InnerText;
            var trimmedText = untrimmedText.Trim();

            OutputFromStanford.Text = trimmedText;

            Parse(trimmedText);
        }

        private void Parse(string text)
        {
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

                //list.Add(new Process(msg[0], msg[1], msg[2]));
            }
        }
    }
}