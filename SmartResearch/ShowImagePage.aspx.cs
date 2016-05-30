using SmartResearch.SemanticNetwork;
using System;
using System.Collections.Generic;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //Label1.Text = Application["msg"] as string;

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

            SmartResearch.SemanticNetwork.Network.SNetwork snet = new SemanticNetwork.Network.SNetwork(pr1);
            var graph = snet.SaveToPng("qwe");
            //image.Save(Server.MapPath("image.png"), System.Drawing.Imaging.ImageFormat.Png);

            //image.Save(@"C:\Users\admin\Documents\Projects\SR\SmartResearch\SmartResearch\image.png", System.Drawing.Imaging.ImageFormat.Png);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.google.com/images/weather/mostly_sunny.gif");
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //System.Drawing.Image img = System.Drawing.Image.FromStream(response.GetResponseStream());
            //img.Save(Server.MapPath("~/image.png"), System.Drawing.Imaging.ImageFormat.Png);
            EncoderParameters eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)16);
            ImageCodecInfo ici = GetEncoderInfo("image/png");
            //graph.Save(Server.MapPath("~/image1.png"), ici, eps);
            //Response.Write(Server.MapPath("~/image.png"));
        }

        ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}