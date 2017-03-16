using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Web.App_Data;
using Core.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Controllers
{
    public class UserController : Controller
    {
        private DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userList = _context.Users.ToList();
            return View(userList);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(User registeruser)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(registeruser);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(registeruser);
        }
    }
}