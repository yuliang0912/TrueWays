using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Models;

namespace TrueWays.Core.Repository
{
    public class OrderInfoRepository : RepositoryBase<OrderInfo>
    {
        const string TableName = "orderInfo";

        public OrderInfoRepository() : base(TableName)
        {
        }
    }
}
