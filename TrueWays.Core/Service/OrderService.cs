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

        public bool Update(object model, object condition)
        {
            return _orderInfoRepository.Update(model, condition);
        }

        public OrderInfo GetLast(object condition)
        {
            return _orderInfoRepository.Get(condition);
        }


        public OrderInfo Get(object condition)
        {
            return _orderInfoRepository.Get(condition);
        }

        public bool CreateOrder(OrderInfo order)
        {
            order.CreateDate = DateTime.Now;
            order.HandleDate = new DateTime(2000, 1, 1);
            order.EndDate = new DateTime(2000, 1, 1);
            order.CommunicationRecord = string.Empty;
            order.FaultContent = string.Empty;
            order.Remark = string.Empty;
            order.HandleName = string.Empty;
            order.Technician = string.Empty;
            return _orderInfoRepository.Insert(order) > 0;
        }
    }
}
