using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;

namespace Repositories.Base
{
    public class SqlServerContext : DbContext
    {
        #region 方法1
        public SqlServerContext(DbContextOptions<SqlServerContext> options)
         : base(options)
        {
        }
        #endregion

        public DbSet<Users> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }

        #region 方法2
        public SqlServerContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer("server=.;database=Test;uid=sa;pwd=sa");
            }
            base.OnConfiguring(optionsBuilder);
        }
        #endregion
    }
}
