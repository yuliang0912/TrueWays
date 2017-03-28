using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Common.Extensions;
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

        public int Create(CustomerInfo model)
        {
            var maxShopNo = Instance.GetMaxShopNo();
            model.ShopNo = maxShopNo + 1;
            model.ShopName = model.ShopName ?? string.Empty;
            model.Abbreviation = model.Abbreviation ?? string.Empty;
            model.ContactName = model.ContactName ?? string.Empty;
            model.Phone = model.Phone ?? string.Empty;
            model.Mobile = model.Mobile ?? string.Empty;
            model.Address = model.Address ?? string.Empty;
            model.Salesman = model.Salesman ?? string.Empty;
            model.Remark = (model.Remark ?? string.Empty).CutString(500);
            model.CreateDate = DateTime.Now;
            model.Status = 0;

            return _customerInfoRepository.Insert(model);
        }

        public bool Update(object model, object condition)
        {
            return _customerInfoRepository.Update(model, condition);
        }


        public List<CustomerInfo> GetPageList(object conditon, int page, int pageSize, out int totalItem)
        {
            return _customerInfoRepository.GetPageList(conditon, "", page, pageSize, out totalItem).ToList();
        }

        public List<CustomerInfo> GetList(object conditon)
        {
            return _customerInfoRepository.GetList(conditon).ToList();
        }


        public IEnumerable<CustomerInfo> SearchCustomers(string keyWords, string phone, int status, int page,
            int pageSize,
            out int totalItem)
        {
            return _customerInfoRepository.SearchCustomers(keyWords, phone, status, page, pageSize, out totalItem);
        }

        public int GetMaxShopNo()
        {
            return _customerInfoRepository.Max();
        }
    }
}
