using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TrueWays.Core.Common.Dapper;
using TrueWays.Core.Utilities;

namespace TrueWays.Core.Repository
{
    public class RepositoryBase<T> where T : class
    {
        private readonly string _tableName;
        private readonly string _readConnectionString;
        private readonly string _writeConnectionString;


        protected RepositoryBase(string tableName, string configurationKey = "true_ways")
        {
            _tableName = tableName;
            _readConnectionString = ConfigHelper.GetConnectionString(configurationKey + "_read");
            _writeConnectionString = ConfigHelper.GetConnectionString(configurationKey + "_write");
        }

        /// <summary>
        /// 读数据库连接
        /// </summary>
        protected virtual MySqlConnection GetReadConnection => new MySqlConnection(_readConnectionString);

        /// <summary>
        /// 写数据库连接
        /// </summary>
        protected virtual MySqlConnection GetWriteConnection => new MySqlConnection(_writeConnectionString);

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual T Get(object condition)
        {
            using (var connection = GetReadConnection)
            {
                return connection.QueryList<T>(condition, _tableName).FirstOrDefault();
            }
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetList(object condition)
        {
            using (var connection = GetReadConnection)
            {
                return connection.QueryList<T>(condition, _tableName);
            }
        }

        /// <summary>
        /// 查询分页列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItem"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(object condition, string orderBy, int page, int pageSize, out int totalItem)
        {
            using (var connection = GetReadConnection)
            {
                return connection.QueryPaged<T>(condition, _tableName, orderBy, page, pageSize,
                    out totalItem);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isReturnIncrementId"></param>
        /// <returns></returns>
        public virtual int Insert(object model, bool isReturnIncrementId = false)
        {
            using (var connection = GetWriteConnection)
            {
                return connection.Insert(model, _tableName, isReturnIncrementId: isReturnIncrementId);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isReturnIncrementId"></param>
        /// <returns></returns>
        public virtual int ReplaceInsert(object model, bool isReturnIncrementId = false)
        {
            using (var connection = GetWriteConnection)
            {
                return connection.ReplaceInsert(model, _tableName, isReturnIncrementId: isReturnIncrementId);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual bool Update(object model, object condition)
        {
            using (var connection = GetWriteConnection)
            {
                return connection.Update(model, condition, _tableName) > 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual bool Delete(object condition)
        {
            using (var connection = GetWriteConnection)
            {
                return connection.Delete(condition, _tableName) > 0;
            }
        }

        public int Count(object condition)
        {
            using (var connection = GetReadConnection)
            {
                return connection.GetCount(condition, _tableName);
            }
        }
    }
}
