using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Common.Dapper;
using TrueWays.Core.Common.Extensions;
using TrueWays.Core.Models;

namespace TrueWays.Core.Repository
{
    public class OrderInfoRepository : RepositoryBase<OrderInfo>
    {
        const string TableName = "orderInfo";

        public OrderInfoRepository() : base(TableName)
        {
        }


        public IEnumerable<OrderInfo> SearchOrders(string orderNo, string phone, string shopName, int orderStatus,
            int page,
            int pageSize,
            out int totalItem)
        {
            var additional = string.Empty;

            if (!string.IsNullOrWhiteSpace(shopName))
            {
                additional += "AND shopName LIKE @shopName ";
            }
            if (!string.IsNullOrEmpty(phone))
            {
                additional += "AND (phone = @phone OR mobile = @phone) ";
            }
            if (orderStatus > 0)
            {
                additional += "AND orderStatus = @orderStatus ";
            }
            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                additional += "AND orderNo LIKE @orderNo ";
            }

            Func<object, string> buildWhereSql =
                (cond) =>
                    SqlMapperExtensions.BuildWhereSql(cond, false, additional, "shopName", "orderStatus", "phone",
                        "orderNo");

            using (var connection = GetReadConnection)
            {
                return
                    connection.QueryPaged<OrderInfo>(
                        new
                        {
                            orderNo = orderNo.FormatSqlLikeString(),
                            shopName = shopName.FormatSqlLikeString(),
                            phone,
                            orderStatus
                        }, TableName,
                        "CreateDate DESC",
                        page, pageSize,
                        out totalItem, buildWhereSql);
            }
        }

        public OrderInfo GetLast(object condition)
        {
            using (var connection = GetReadConnection)
            {
                return
                    connection.QueryList<OrderInfo>(condition, TableName, orderBy: "CreateDate DESC").FirstOrDefault();
            }
        }
    }
}
