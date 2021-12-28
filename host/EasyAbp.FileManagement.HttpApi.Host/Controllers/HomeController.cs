using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.FileManagement.Controllers
{
    public class HomeController : AbpControllerBase
    {
        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
