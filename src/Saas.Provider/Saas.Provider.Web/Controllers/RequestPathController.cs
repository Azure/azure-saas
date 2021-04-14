using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Controllers
{
    [Route("/{tenant}")]
    public class RequestPathController : Controller
    {
        [Route("/{tenant}")]
        public IActionResult Index(string tenant)
        {
            ViewBag.Tenant = tenant;

            return View();
        }
    }
}
