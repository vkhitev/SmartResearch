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
        private static string _uploadedFilePath;

        protected void Page_Load(object sender, EventArgs e)
        {
			Session.Clear();

			string db = Request.QueryString["db"];
			if (db != null && db == "failed")
			{
				Label l = new Label();
				l.Text = "Извините, мы не можем создать базу знаний по введённому тексту.";
				l.ForeColor = System.Drawing.Color.Red;
				l.Style.Add("display", "block");
				EmptyLabel.Controls.Add(l);
				ButtonNextStep.Style.Add("margin-bottom", "0");
			}
		}

        protected void ButtonNextStep_Click(object sender, EventArgs e)
        {
			if (TextBox.Text.Length == 0)
			{
				Label l = new Label();
				l.Text = "Заполните поле ввода!";
				l.ForeColor = System.Drawing.Color.Red;
				l.Style.Add("display", "block");
				EmptyLabel.Controls.Add(l);
				ButtonNextStep.Style.Add("margin-bottom", "0");
				return;
			}

			NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("query", TextBox.Text);
            string postData = outgoingQueryString.ToString().ToLower();

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
            string responseText = "";
            using (var reader = new StreamReader(webResponse.GetResponseStream(), ASCIIEncoding.ASCII))
            {
                responseText = reader.ReadToEnd();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseText);

            var foundClasses = doc.DocumentNode.Descendants("div").Where(d =>
                d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("parserOutput"));

            var nodes = foundClasses.ToArray();

            var result = nodes[3].InnerText.Trim();

			AnalysisResults.processes = Parser.DependencyParser.ParseDependencies(result);

			Response.Redirect("analysis_results");
		} 

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName);
                if (fileExtension.ToLower() != ".txt")
                {
                    LabelUpload.Text = "Поддерживаемое расширение: .txt";
                    LabelUpload.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    int fileSize = FileUpload.PostedFile.ContentLength;
                    if (fileSize > 2097152)
                    {
                        LabelUpload.Text = "Превышен максимальный размер файла (2мб)";
                        LabelUpload.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        FileUpload.SaveAs(Server.MapPath(@"~/" + FileUpload.FileName));
                        _uploadedFilePath = Server.MapPath(@"~/" + FileUpload.FileName);
						//FileUpload.SaveAs(Server.MapPath(@"~/Uploads/" + FileUpload.FileName));
						//_uploadedFilePath = Server.MapPath(@"~/Uploads/" + FileUpload.FileName);
						LabelUpload.Text = "Файл загружен";
                        LabelUpload.ForeColor = System.Drawing.Color.Green;
                        string text;
                        using (var streamReader = new StreamReader(_uploadedFilePath, Encoding.UTF8))
                        {
                            text = streamReader.ReadToEnd();
                            TextBox.Text = text;
                        }
                    }
                }
            }
            else
            {
                LabelUpload.Text = "Пожалуйста, выберите файл для загрузки";
                LabelUpload.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void ButtonFileContinue_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_uploadedFilePath))
            {
                LabelUpload.Text = "Файл не загружен";
                return;
            }

            string text;
            using (var streamReader = new StreamReader(_uploadedFilePath, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                TextBox.Text = text;
            }
        }

		protected void Example1_Click(object sender, EventArgs e)
		{
			TextBox.Text = "Every animal has brain and live life. " +
						   "Cat is an animal, it has tail and claws. " +
						   "Also cat has color fur. " +
						   "Cat hunts mouse. " +
						   "Mouse is an animal too. " +
						   "It has small tail. " +
						   "Mouse eat wheat and cheese.";
		}

		protected void Example2_Click(object sender, EventArgs e)
		{
			TextBox.Text = "\"Domino\" is the best pizzeria with delicious pizza. " +
						   "Carbonara is a pizza. " +
						   "It has delicious taste. " +
						   "It has bacon, ham, onion, mozzarella and mushrooms.";
		}

		protected void Example3_Click(object sender, EventArgs e)
		{
			TextBox.Text = "Complex_number is a set of all numbers. " +
						   "It has Rational_numbers, Irrational_numbers, Integers and natural_numbers.";
		}
	}
}