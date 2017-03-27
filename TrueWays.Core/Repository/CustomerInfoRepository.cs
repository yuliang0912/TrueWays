using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TrueWays.Core.Common.Dapper;
using TrueWays.Core.Models;

namespace TrueWays.Core.Repository
{
    public class CustomerInfoRepository : RepositoryBase<CustomerInfo>
    {
        const string TableName = "customerInfo";

        public CustomerInfoRepository() : base(TableName)
        {
        }

        public IEnumerable<CustomerInfo> SearchCustomers(string keyWords, string phone, int status, int page,
            int pageSize,
            out int totalItem)
        {
            var additional = string.IsNullOrWhiteSpace(keyWords)
                ? string.Empty
                : "AND (name LIKE @keyWords OR contactName LIKE @keyWords) ";

            if (!string.IsNullOrEmpty(phone))
            {
                additional += $"AND (phone = '{phone}' OR mobile = '{phone}') ";
            }
            if (status > 0)
            {
                additional += $"AND status = {status} ";
            }

            Func<object, string> buildWhereSql =
                (cond) => SqlMapperExtensions.BuildWhereSql(cond, false, additional, "keyWords");

            using (var connection = GetReadConnection)
            {
                return connection.QueryPaged<CustomerInfo>(new {keyWords}, TableName, "CreateDate DESC", page, pageSize,
                    out totalItem, buildWhereSql);
            }
        }

        public int Max()
        {
            using (var connection = GetReadConnection)
            {
                return connection.ExecuteScalar<int>("SELECT MAX(shopNo) From customerInfo");
            }
        }
    }
}
