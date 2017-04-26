using System;
using System.Collections.Generic;
using System.Text;
using Repositories.Base;


namespace Repositories
{
    public static class RepositoryFctory//<T> where T : class, new()
    {
        //private DataContext DbContext { get; set; }
        //public RepositoryFctory(DataContext dbContext)
        //{
        //    DbContext = dbContext;
        //}
        ///// <summary>
        ///// 获取DBContext
        ///// </summary>
        ///// <returns></returns>
        //public IRepository GetSqlServer
        //{
        //    get
        //    {
        //        return new SqlServer(DbContext);
        //    }
        //}


        //public RepositoryFctory()
        //{

        //}
        public static IRepository GetSqlServer
        {
            get
            {
                return new SqlServer();
            }

        }

        public static IRepository GetSqlite
        {
            get
            {
                return new SqlServer();
            }

        }
    }
}
