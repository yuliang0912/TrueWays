using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Models;

namespace TrueWays.Core.Repository
{
    public class CustomerInfoRepository : RepositoryBase<CustomerInfo>
    {
        const string TableName = "customerInfo";

        public CustomerInfoRepository() : base(TableName)
        {
        }
    }
}
