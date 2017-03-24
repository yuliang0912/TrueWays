using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Repository;

namespace TrueWays.Core.Service
{
    public class OrderService : Singleton<OrderService>
    {
        private readonly OrderInfoRepository _orderInfoRepository = new OrderInfoRepository();

        private OrderService()
        {
        }
    }
}
