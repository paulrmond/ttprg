using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using BCS.Models;

namespace BCS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer<BCS_Context>(null);
            //Database.SetInitializer<BCS_Context>(new DropCreateDatabaseAlways<BCS_Context>());

            //DropCreateDatabaseAlways - use only if has model changes.
            Database.SetInitializer<BCS_Context>(new BCSDBInitializer());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
