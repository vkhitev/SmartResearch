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
            //ButtonFileContinue.Style.Add("display", "none");
        }

        protected void ButtonNextStep_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Length == 0)
            {
                Label l = new Label();
                l.Text = "Заполните поле ввода!";
                l.ForeColor = System.Drawing.Color.Red;
                l.Style.Add("display", "block");
                //EmptyLabel.Controls.Add(new LiteralControl("<br />"));
                EmptyLabel.Controls.Add(l);
                ButtonNextStep.Style.Add("margin-bottom", "0");
                return;
            }

            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("query", TextBox1.Text);
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

            //OutputFromStanford.Text = trimmedText;

            Parse(trimmedText);

            string text = TextBox1.Text;
            Session.Add("msg", text);
            //Response.Redirect("/ShowImagePage.aspx");
            Response.Redirect("/AnalysisResults.aspx");
        }

        protected void ButtonParseText_Click(object sender, EventArgs e)
        {
            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add("query", TextBox1.Text);
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

            //OutputFromStanford.Text = trimmedText;

            Parse(trimmedText);
        }

        public enum Parsing { Nsubj, Compound, Nothing, Waiting };

        private void Parse(string text)
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
            AnalysisResults.processes = processes;
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName);
                if (fileExtension.ToLower() != ".doc" &&
                    fileExtension.ToLower() != ".docx" &&
                    fileExtension.ToLower() != ".txt")
                {
                    LabelUpload.Text = "Поддерживаемые расширения: .doc, .docx, .txt";
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
                        FileUpload.SaveAs(Server.MapPath(@"~/Uploads/" + FileUpload.FileName));
                        _uploadedFilePath = Server.MapPath(@"~/Uploads/" + FileUpload.FileName);
                        LabelUpload.Text = "Файл загружен";
                        LabelUpload.ForeColor = System.Drawing.Color.Green;
                        string text;
                        using (var streamReader = new StreamReader(_uploadedFilePath, Encoding.UTF8))
                        {
                            text = streamReader.ReadToEnd();
                            TextBox1.Text = text;
                            //Session.Add("msg", text);
                            //Response.Redirect("/AnalysisResults.aspx");
                        }
                        //ButtonFileContinue.Style.Add("display", "block");
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
            if (!System.IO.File.Exists(_uploadedFilePath))
            {
                LabelUpload.Text = "Файл не загружен";
                return;
            }

            string text;
            using (var streamReader = new StreamReader(_uploadedFilePath, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                TextBox1.Text = text;
                //Session.Add("msg", text);
                //Response.Redirect("/AnalysisResults.aspx");
            }
        }
    }
}