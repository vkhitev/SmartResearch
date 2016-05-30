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

namespace SmartResearch
{
    public partial class App : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox1.Text = File.ReadAllText(Server.MapPath("input.txt"));

            var httpRequest = (HttpWebRequest)WebRequest.Create("http://nlp.stanford.edu:8080/parser/");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            string postData = "My dog also likes eating sausage.";
            byte[] dataArray = Encoding.UTF8.GetBytes(postData.ToString());
            httpRequest.ContentLength = dataArray.Length;
            HttpWebResponse webResponse;
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                requestStream.Write(dataArray, 0, dataArray.Length);
                webResponse = (HttpWebResponse)httpRequest.GetResponse();
            }

            WebHeaderCollection header = webResponse.Headers;

            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(webResponse.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                TextBox1.Text = responseText;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(TextBox1.Text);

            var findclasses = doc.DocumentNode.Descendants("div").Where(d =>
                d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("parserOutput"));

            var nodes = findclasses.ToArray();
            TextBox1.Text = nodes[3].InnerText;

            //HtmlNodeCollection nodesMatchingXPath = doc.DocumentNode.SelectNodes("x/path/nodes");
            //HttpClient http = new HttpClient();
            //HttpClient http = new HttpClient();
            //var response = await http.GetByteArrayAsync(website);
            //String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            //source = WebUtility.HtmlDecode(source);
            //HtmlDocument resultat = new HtmlDocument();
            //resultat.LoadHtml(source);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string text = TextBox1.Text.ToString();
            Application.Add("msg", text);
            Response.Redirect("/ShowImagePage.aspx");
        }
    }
}