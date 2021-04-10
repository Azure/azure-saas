using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Controllers
{
    public class RequestPathController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
