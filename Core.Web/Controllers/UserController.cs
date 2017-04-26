using System.Collections.Generic;
using System.Linq;
using Core.Web.App_Data;
using Core.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common;
using Repositories.Base;
using System.Data.SqlClient;

namespace Core.Web.Controllers
{
    public class UserController : Controller
    {
        private SqlServerContext _db = Repositories.RepositoryFctory.GetSqlServer.DbContext;

        //public UserController (DataContext context)
        //{
        //    _db = context;
        //}
        public IActionResult Index(int id)
        {
            var sql = Repositories.RepositoryFctory.GetSqlServer;
            var list = sql.FindList<Users>(PredicateBuilder.True<Users>());
            var list1 = sql.FindList<Users>(PredicateBuilder.True<Users>().And(ff => ff.Id == 1));
            var entity = sql.ExecuteSql<Users>("select * from users where id=@id", new SqlParameter("id", "2"));
            var entity1 = sql.ExecuteSql<Users>("select * from users ");

             
            //var entity3 = sql.ExecuteSqlCommand("update users set id=@id where id=@id", new SqlParameter("id", "2"));
            var entity4 = sql.ExecuteSqlCommand("select count(*) from users");
            var enti5 = _db.Set<Users>().FromSql("select * from users ").ToList().Count();

            int pageIndex = id == 0 ? 1 : id;
            int pageSize = 1;
            //var predicate = PredicateBuilder.True<Users>()
            //        .And(aa => aa.Id > 0);
            var userList = _db.Users.AsNoTracking()
                //.Where(predicate)
                .OrderBy(m => m.Id)
                .OrderByDescending(m => m.Age)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var count = _db.Users.AsNoTracking()
                  //.Where(predicate)
                  .Count();
            //Common.EnumResult dd = sql.Insert<Users>(null);
            ViewBag.ListCount = count;
            ViewBag.PageIndex = pageIndex;
            var s = EnumResult.Error.GetResultString("Ìí¼Ó");
            return View(userList);
        }

        public IActionResult IndexAjax()
        {
            return View();
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
                ViewBag.Result = 1;
            }
            return View(registeruser);
        }

        public IActionResult Delete(int id)
        {
            var model = _db.Users.Find(id);
            ViewBag.Result = -1;
            if (model != null)
            {
                _db.Users.Remove(model);
                _db.SaveChanges();
                ViewBag.Result = 1;
            }
            return RedirectToAction("Index");
        }

        public int DeleteNew(int id)
        {
            //var ss = from b in _db.Users where b.Id > 1 select b;
            var model = _db.Users.Find(id);
            _db.Users.Remove(model);
            _db.SaveChanges();
            return 1;
        }
    }

}