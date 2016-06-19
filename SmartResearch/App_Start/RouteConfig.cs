using System;
using System.Web.Routing;

//namespace SmartResearch.App_Start
namespace SmartResearch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute(null, "", "~/Pages/Default.aspx");
            routes.MapPageRoute(null, "application", "~/Pages/App.aspx");
            routes.MapPageRoute(null, "about", "~/Pages/About.aspx");
            routes.MapPageRoute(null, "contacts", "~/Pages/Contacts.aspx");
            routes.MapPageRoute(null, "documentation", "~/Pages/Documentation.aspx");
			routes.MapPageRoute(null, "analysis_results", "~/Pages/AnalysisResults.aspx");
		}
    }
}