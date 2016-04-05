using System.Web.Mvc;

namespace Nop.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            
 

            context.MapRoute(
              "MembershipCategory",
              "Admin/MembershipCategory",
              new { controller = "MembershipCategory", action = "List", area = "Admin", id = "" },
              new[] { "Nop.Admin.Controllers" }
          );

            context.MapRoute(
               "Plan",
               "Admin/Plan",
               new { controller = "Plan", action = "List", area = "Admin", id = "" },
               new[] { "Nop.Admin.Controllers" }
           );


            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Admin", id = "" },
                new[] { "Nop.Admin.Controllers" }
            );
        }
    }
}
