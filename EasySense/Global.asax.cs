using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EasySense
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // System.Data.Entity.Database.SetInitializer<Models.EasySenseContext>(null);

            Startup.Config = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/config.json"));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["sid"] = Helpers.String.RandomString(64);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }
    }
}
