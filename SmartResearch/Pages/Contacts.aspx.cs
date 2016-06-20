using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartResearch
{
    public partial class Contacts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SendMail_Click(object sender, EventArgs e)
        {
			string server = "smartresearch.azurewebsites.net";
			int port = 25;
			string to = "vkhitev@gmail.com";
			string from = txtFrom.Text;
			string subject = txtSubject.Text;
			string body = txtBody.Text;
			MailMessage message = new MailMessage(from, to, subject, body);
			SmtpClient client = new SmtpClient(server, port);
			// Credentials are necessary if the server requires the client 
			// to authenticate before it will send e-mail on the client's behalf.
			client.Credentials = CredentialCache.DefaultNetworkCredentials;

			try
			{
				client.Send(message);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in CreateTestMessage1(): {0}",
							ex.ToString());
			}
        }
    }
}