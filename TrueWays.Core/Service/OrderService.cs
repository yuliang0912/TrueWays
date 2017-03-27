using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Models;
using TrueWays.Core.Repository;

namespace TrueWays.Core.Service
{
    public class OrderService : Singleton<OrderService>
    {
        private readonly OrderInfoRepository _orderInfoRepository = new OrderInfoRepository();

        private OrderService()
        {
        }

        public IEnumerable<OrderInfo> SearchOrders(string orderNo, string phone, string shopName, int orderStatus,
            int page,
            int pageSize,
            out int totalItem)
        {
            return _orderInfoRepository.SearchOrders(orderNo, phone, shopName, orderStatus, page, pageSize,
                out totalItem);
        }

        public IEnumerable<OrderInfo> GetList(object condition)
        {
            return _orderInfoRepository.GetList(condition);
        }
    }
}
