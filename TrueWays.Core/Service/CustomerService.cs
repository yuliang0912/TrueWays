using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Models;
using TrueWays.Core.Repository;

namespace TrueWays.Core.Service
{
    public class CustomerService : Singleton<CustomerService>
    {
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

        private CustomerService()
        {
        }

        public CustomerInfo Get(object condition)
        {
            return _customerInfoRepository.Get(condition);
        }


        public List<CustomerInfo> GetPageList(object conditon, int page, int pageSize, out int totalItem)
        {
            return _customerInfoRepository.GetPageList(conditon, "", page, pageSize, out totalItem).ToList();
        }


        public IEnumerable<CustomerInfo> SearchCustomers(string keyWords, string phone, int status, int page,
            int pageSize,
            out int totalItem)
        {
            return _customerInfoRepository.SearchCustomers(keyWords, phone, status, page, pageSize, out totalItem);
        }
    }
}
