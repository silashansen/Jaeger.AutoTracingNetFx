using System.Web;
using System.Web.Mvc;
using WebApplication1.Attributes;

namespace WebApplication1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
        }
    }
}
