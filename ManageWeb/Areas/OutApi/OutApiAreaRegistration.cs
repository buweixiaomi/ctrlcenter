using System.Web.Mvc;

namespace ManageWeb.Areas.OutApi
{
    public class OutApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OutApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OutApi_default",
                "OutApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
