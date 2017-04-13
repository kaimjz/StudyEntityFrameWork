using System.Collections.Generic;
using System.Linq;
using Core.Web.App_Data;
using Core.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Web.Controllers
{
    public class UserController : Controller
    {
        private DataContext _db;

        public UserController(DataContext context)
        {
            _db = context;
        }

        public IActionResult Index(int id)
        {
            int pageIndex = id == 0 ? 1 : id;
            int pageSize = 1;
            //var predicate = PredicateBuilder.True<Users>()
            //        .And(aa => aa.Id > 0);
            var userList = _db.Users.AsNoTracking()
                //.Where(predicate)
                .OrderBy(m => m.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var count = _db.Users.AsNoTracking()
                  //.Where(predicate)
                  .Count();
            ViewBag.ListCount = count;
            ViewBag.PageIndex = pageIndex;
            return View(userList);
        }

        public IActionResult IndexAjax()
        {
            return View("IndexAjax");
        }

        [HttpPost]
        public JsonResult GetUserList()
        {
            int PageIndex = int.Parse(Request.Form["PageIndex"]);
            int PageSize = int.Parse(Request.Form["PageSize"]);
            int Pager = int.Parse(Request.Form["Pager"]);
            var userList = _db.Users.AsNoTracking()
                .OrderBy(k => k.Id)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            var count = 0;
            if (Pager == 0)
                count = _db.Users.AsNoTracking().Count();
            return new JsonResult(new { list = userList, count = count });
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Users registeruser)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(registeruser);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(registeruser);
        }
    }
}