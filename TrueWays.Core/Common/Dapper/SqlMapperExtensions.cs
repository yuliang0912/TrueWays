using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dapper;

namespace TrueWays.Core.Common.Dapper
{
    public static class SqlMapperExtensions
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> ParamCache =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();

        /// <summary>Insert data into table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Insert(this IDbConnection connection, dynamic data, string table,
            IDbTransaction transaction = null, int? commandTimeout = null, bool isReturnIncrementId = false)
        {
            var obj = data as object;
            var properties = GetProperties(obj);
            var columns = string.Join(",", properties);
            var values = string.Join(",", properties.Select(p => "@" + p));
            var sql = $"insert into {table} ({columns}) values ({values});";

            if (isReturnIncrementId)
            {
                sql += "SELECT LAST_INSERT_ID()";
                return connection.ExecuteScalar<int>(sql, obj, transaction, commandTimeout);
            }

            return connection.Execute(sql, obj, transaction, commandTimeout);
        }


        /// <summary>Insert data into table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int ReplaceInsert(this IDbConnection connection, dynamic data, string table,
            IDbTransaction transaction = null, int? commandTimeout = null, bool isReturnIncrementId = false)
        {
            var obj = data as object;
            var properties = GetProperties(obj);
            var columns = string.Join(",", properties);
            var values = string.Join(",", properties.Select(p => "@" + p));
            var sql = $"replace into {table} ({columns}) values ({values});";

            if (isReturnIncrementId)
            {
                sql += "SELECT LAST_INSERT_ID()";
                return connection.ExecuteScalar<int>(sql, obj, transaction, commandTimeout);
            }

            return connection.Execute(sql, obj, transaction, commandTimeout);
        }


        /// <summary>Updata data for table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="buildCondition"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Update(this IDbConnection connection, dynamic data, dynamic condition, string table,
            Func<object, string> buildCondition = null,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj);
            var updateFields = string.Join(",", GetProperties(obj).Select(p => p + " = @" + p));

            var whereFields = buildCondition == null ? BuildWhereSql(conditionObj) : buildCondition(conditionObj);

            var sql = $"update {table} set {updateFields}{whereFields}";

            var parameters = new DynamicParameters(data);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add(p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);
            
            return connection.Execute(sql, parameters, transaction, commandTimeout);
        }
        

        /// <summary>Delete data from table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="buildCondition"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Delete(this IDbConnection connection, dynamic condition, string table,
            Func<object, string> buildCondition = null,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;

            var whereFields = buildCondition == null ? BuildWhereSql(conditionObj) : buildCondition(conditionObj);

            var sql = $"delete from {table}{whereFields}";

            return connection.Execute(sql, conditionObj, transaction, commandTimeout);
        }

        /// <summary>Get data count from table with a specified condition.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="buildCondition"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int GetCount(this IDbConnection connection, object condition, string table,
            Func<object, string> buildCondition = null, bool isOr = false,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return
                QueryList<int>(connection, condition, table, buildCondition, null, "count(*)", isOr, transaction,
                    commandTimeout).Single();
        }

        /// <summary>Query a list of data from table with specified condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="table"></param>
        /// <param name="buildCondition"></param>
        /// <param name="orderBy"></param>
        /// <param name="columns"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object condition, string table,
            Func<object, string> buildCondition = null, string orderBy = null,
            string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            var conditionObj = condition as object;

            var whereFields = buildCondition == null ? BuildWhereSql(conditionObj) : buildCondition(conditionObj);

            var order = string.IsNullOrEmpty(orderBy) ? string.Empty : " ORDER BY " + orderBy;

            var sql = $"SELECT {columns} FROM {table} {whereFields} {order}";

            return connection.Query<T>(sql, condition, transaction, true,
                commandTimeout);
        }

        /// <summary>
        /// 获取分页,并且返回总行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="condition"></param>
        /// <param name="buildCondition"></param>
        /// <param name="table"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItem"></param>
        /// <param name="columns"></param>
        /// <param name="isOr"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryPaged<T>(this IDbConnection connection, dynamic condition, string table,
            string orderBy, int pageIndex, int pageSize, out int totalItem, Func<object, string> buildCondition = null,
            string columns = "*", bool isOr = false,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;

            var whereFields = buildCondition == null ? BuildWhereSql(conditionObj) : buildCondition(conditionObj);

            var totalSql = $"SELECT COUNT(*) FROM {table}{whereFields}";

            totalItem = connection.ExecuteScalar<int>(totalSql, conditionObj, transaction, commandTimeout);

            if (totalItem <= (pageIndex - 1)*pageSize)
            {
                return Enumerable.Empty<T>();
            }

            var sql = string.IsNullOrWhiteSpace(orderBy)
                ? $"SELECT {columns} FROM {table}{whereFields} LIMIT {(pageIndex - 1)*pageSize},{pageSize}"
                : $"SELECT {columns} FROM {table}{whereFields} ORDER BY {orderBy} LIMIT {(pageIndex - 1)*pageSize},{pageSize}";

            return connection.Query<T>(sql, conditionObj, transaction, true, commandTimeout);
        }

        private static List<string> GetProperties(object obj)
        {
            if (obj == null)
            {
                return new List<string>();
            }
            if (obj is DynamicParameters)
            {
                return ((DynamicParameters) obj).ParameterNames.ToList();
            }
            if (obj is Dictionary<string, object>)
            {
                return ((Dictionary<string, object>) obj).Select(t => t.Key).ToList();
            }
            return GetPropertyInfos(obj).Select(x => x.Name).ToList();
        }

        private static List<PropertyInfo> GetPropertyInfos(object obj)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }

            List<PropertyInfo> properties;
            if (ParamCache.TryGetValue(obj.GetType(), out properties))
            {
                return properties.ToList();
            }
            properties =
                obj.GetType()
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    .Where(t =>  !t.GetCustomAttributes(true).Any(x => x is IgnoreFieldAttribute))
                    .ToList();
            
            ParamCache[obj.GetType()] = properties;
            return properties;
        }


        /// <summary>
        /// 拓展构建查询条件的方法
        /// </summary>
        /// <param name="condition">条件对象</param>
        /// <param name="isOr">是否OR关联条件</param>
        /// <param name="additional">追加查询</param>
        /// <param name="ignoreColumn">忽略条件中的列</param>
        /// <returns></returns>
        public static string BuildWhereSql(object condition, bool isOr = false, string additional = "",
            params string[] ignoreColumn)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var properties = GetProperties(conditionObj);
            var whereColumn = properties.Where(t => !ignoreColumn.Contains(t)).ToList();

            if (whereColumn.Count == 0 && string.IsNullOrWhiteSpace(additional))
            {
                return whereFields;
            }

            whereFields = " WHERE ";

            if (whereColumn.Count > 0)
            {
                var separator = isOr ? " OR " : " AND ";
                whereFields += string.Join(separator, whereColumn.Select(p => p + " = @" + p));
            }
            else if (!string.IsNullOrWhiteSpace(additional))
            {
                whereFields = whereFields + " 1=1 ";
            }

            if (!string.IsNullOrWhiteSpace(additional))
            {
                whereFields = whereFields + " " + additional;
            }

            return whereFields;
        }
    }
}
