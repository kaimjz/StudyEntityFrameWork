using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult Error(string id)
        {
            ViewData["ErrorInfo"] = id;
            return View();
        }
    }
}