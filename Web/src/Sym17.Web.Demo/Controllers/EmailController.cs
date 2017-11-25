using System.Web.Mvc;
using Sitecore.Analytics;

namespace Sym17.Web.Demo.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index(string xdbcontactid)
        {
            return View("~/Views/SYM/Email/Email.cshtml");
        }
    }
}