using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Common;
using Repositories.Base;

namespace Repositories
{
    public interface IRepository : IDisposable
    {
        SqlServerContext DbContext { get; }

        #region 添加

        /// <summary>
        /// 添加单条记录
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns>枚举</returns>
        EnumResult Insert<T>(T model) where T : class, new();

        #endregion

        #region 删除

        /// <summary>
        /// 根据model主键删除单条数据
        /// </summary>
        /// <param name="model">包含主键的实体</param>
        /// <returns>枚举</returns>
        EnumResult Delete<T>(T model) where T : class, new();

        /// <summary>
        /// 根据models主键删除多条数据
        /// </summary>
        /// <param name="models">包含主键的实体集合</param>
        /// <returns>枚举</returns>
        EnumResult Delete<T>(List<T> models) where T : class, new();

        /// <summary>
        /// 根据where条件删除多条数据 先查再删,效率慢
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <returns>枚举</returns>
        EnumResult Delete<T>(Expression<Func<T, bool>> whereLambda) where T : class, new();

        #endregion

        #region 修改

        /// <summary>
        /// 修改单条所有or部分属性
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="pNames">部分属性</param>
        /// <returns>枚举</returns>
        EnumResult Modify<T>(T model, params string[] pNames) where T : class, new();

        /// <summary>
        /// 修改多条所有or部分属性
        /// </summary>
        /// <param name="model">修改的实体</param>
        /// <param name="whereLambda">where条件</param>
        /// <param name="pNames">部分属性</param>
        /// <returns>枚举</returns>
        EnumResult Modify<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] pNames) where T : class, new();

        #endregion

        #region 查询

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <returns>实体</returns>
        T FindEntity<T>(Expression<Func<T, bool>> whereLambda) where T : class, new();

        /// <summary>
        /// 查询所有or几条数据
        /// </summary>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        List<T> FindList<T>(int topNum = -1) where T : class, new();

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        List<T> FindList<T>(Expression<Func<T, bool>> whereLambda, int topNum = -1) where T : class, new();

        /// <summary>
        /// 查询多条数据并排序
        /// </summary>
        /// <param name="whereLambda">where条件</param>
        /// <param name="orderLambda">排序字段</param>
        /// <param name="isAsc">是否正排序,默认倒序</param>
        /// <param name="topNum">取几条数据</param>
        /// <returns>实体集合</returns>
        List<T> FindList<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = false, int topNum = -1) where T : class, new();

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
        List<T> FindList<T, TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc, out int totalCount) where T : class, new();

        #endregion

        #region 执行sql语句

        /// <summary>
        /// 执行sql语句 并返回集合
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>集合</returns>
        List<T> ExecuteSql<T>(string sql, params object[] parameters) where T : class, new();

        /// <summary>
        /// 执行sql语句 并返回集合+结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>集合</returns>
        List<T> ExecuteSql<T>(string sql, ref int outResult, SqlParameter outParams, params object[] parameters) where T : class, new();

        /// <summary>
        /// 执行DDL/DML命令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">SqlParameter参数集合</param>
        /// <returns>结果</returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        #endregion
    }
}