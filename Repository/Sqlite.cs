using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repositories.Base;

namespace Repositories
{
    public class Sqlite
    {

        #region 上下文

        private volatile static SqliteContext instance = null;

        public SqliteContext DbContext
        {
            get
            {
                var init = instance;
                if (init == null)
                {
                    init = instance = new SqliteContext();
                }
                return init;
            }
        }

        #endregion

        #region 公共

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加单条记录
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>枚举</returns>
        public EnumResult Insert<T>(T model) where T : class, new()
        {
            try
            {
                DbContext.Add<T>(model);
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.Fail;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }

        #endregion

        #region 删除

        /// <summary>
        /// 根据model主键删除单条数据
        /// </summary>
        /// <param name="model">包含主键的实体</param>
        /// <returns>枚举</returns>
        public EnumResult Delete<T>(T model) where T : class, new()
        {
            try
            {
                DbContext.Entry<T>(model).State = EntityState.Deleted;
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.NoData;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }

        /// <summary>
        /// 根据models主键删除多条数据
        /// </summary>
        /// <param name="models">包含主键的实体集合</param>
        /// <returns>枚举</returns>
        public EnumResult Delete<T>(List<T> models) where T : class, new()
        {
            try
            {
                foreach (var mo in models)
                {
                    DbContext.Entry<T>(mo).State = EntityState.Deleted;
                }
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.NoData;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }

        /// <summary>
        /// 根据where条件删除多条数据 先查再删,效率慢
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <returns>枚举</returns>
        public EnumResult Delete<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            try
            {
                List<T> listDels = DbContext.Set<T>().ToList();
                if (listDels != null && listDels.Count != 0)
                {
                    foreach (var del in listDels)
                    {
                        DbContext.Entry<T>(del).State = EntityState.Deleted;
                    }
                }
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.NoData;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }

        #endregion

        #region 修改

        /// <summary>
        /// 修改单条所有or部分属性
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="pNames">部分属性</param>
        /// <returns>枚举</returns>
        public EnumResult Modify<T>(T model, params string[] pNames) where T : class, new()
        {
            try
            {
                EntityEntry entry = DbContext.Entry<T>(model);
                if (pNames != null && pNames.Length > 0)
                {
                    entry.State = EntityState.Unchanged;
                    foreach (var pn in pNames)
                    {
                        entry.Property(pn).IsModified = true;
                    }
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.NoData;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }

        /// <summary>
        /// 修改多条所有or部分属性
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="whereLambda">where条件</param>
        /// <param name="pNames">部分属性</param>
        /// <returns>枚举</returns>
        public EnumResult Modify<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] pNames) where T : class, new()
        {
            try
            {
                var listModifies = DbContext.Set<T>().Where(whereLambda).ToList();
                if (listModifies == null || listModifies.Count == 0)
                {
                    return EnumResult.NoData;
                }
                Type t = typeof(T);
                var pros = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                var dicPros = new Dictionary<string, PropertyInfo>();
                foreach (var pn in pNames)
                {
                    var pro = pros.Find(p => p.Name.ToLower() == pn.ToLower());
                    if (pro != null)
                    {
                        dicPros.Add(pro.Name, pro);
                    }
                }
                if (dicPros.Count > 0)
                {
                    foreach (var kv in dicPros)
                    {
                        var newValue = kv.Value.GetValue(model, null);
                        foreach (var modify in listModifies)
                        {
                            kv.Value.SetValue(modify, newValue);
                        }
                    }
                }
                else
                {
                    foreach (var p in pros)
                    {
                        var newValue = p.GetValue(model, null);
                        foreach (var modify in listModifies)
                        {
                            p.SetValue(modify, newValue);
                        }
                    }
                }
                return DbContext.SaveChanges() > 0 ? EnumResult.Success : EnumResult.NoData;
            }
            catch (Exception ex)
            {
                return EnumResult.Error;
            }
        }


        #endregion

        #region 查询

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <returns>实体</returns>
        public T FindEntity<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            return DbContext.Set<T>().Where(whereLambda).FirstOrDefault();
        }

        /// <summary>
        /// 查询所有or几条数据
        /// </summary>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        public List<T> FindList<T>(int topNum = -1) where T : class, new()
        {
            if (topNum == -1)
            {
                return DbContext.Set<T>().ToList();
            }
            return DbContext.Set<T>().Take(topNum).ToList();
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        public List<T> FindList<T>(Expression<Func<T, bool>> whereLambda, int topNum = -1) where T : class, new()
        {
            if (topNum == -1)
            {
                return DbContext.Set<T>().Where(whereLambda).ToList();
            }
            return DbContext.Set<T>().Where(whereLambda).Take(topNum).ToList();
        }

        /// <summary>
        /// 查询多条数据并排序
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <param name="orderLambda">排序字段</param>
        /// <param name="isAsc">是否正排序,默认倒序</param>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        public List<T> FindList<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = false, int topNum = -1) where T : class, new()
        {
            if (isAsc)
            {
                if (topNum == -1)
                {
                    return DbContext.Set<T>().Where(whereLambda).OrderBy(orderLambda).ToList();
                }
                return DbContext.Set<T>().Where(whereLambda).OrderBy(orderLambda).Take(topNum).ToList();
            }
            else
            {
                if (topNum == -1)
                {
                    return DbContext.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).ToList();
                }
                return DbContext.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).Take(topNum).ToList();
            }
        }

        /// <summary>
        /// 查询多条数据分页并排序并返回总行数
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="whereLambda">where条件</param>
        /// <param name="orderLambda">排序</param>
        /// <param name="isAsc">是否正排序</param>
        /// <param name="totalCount">总行数</param>
        /// <returns>实体集合</returns>
        public List<T> FindList<T, TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc, out int totalCount) where T : class, new()
        {
            totalCount = DbContext.Set<T>().Where(whereLambda).Count();
            if (isAsc)
            {
                return DbContext.Set<T>().Where(whereLambda).OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return DbContext.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        #endregion

        #region SQL语句

        /// <summary>
        /// 执行sql语句or存储过程 并返回集合
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>集合</returns>
        public List<T> ExecuteSql<T>(string sql, params object[] parameters) where T : class, new()
        {
            try
            {
                return DbContext.Set<T>().FromSql(sql, parameters).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        /// 执行sql语句or存储过程 并返回集合+结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>集合</returns>
        public List<T> ExecuteSql<T>(string sql, ref int outResult, SqlParameter outParams, params object[] parameters) where T : class, new()
        {
            try
            {
                var ls = DbContext.Set<T>().FromSql(sql, outParams, parameters).ToList();
                outResult = (int)outParams.Value;
                return ls;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 执行DDL/DML命令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>结果</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            try
            {
                return DbContext.Database.ExecuteSqlCommand(sql, parameters);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion
    }
}
