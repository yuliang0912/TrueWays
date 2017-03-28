using System;

namespace TrueWays.Core.Models
{
    public class CustomerInfo
    {
        public int CustomerId { get; set; }

        /// <summary>
        /// 店铺编码
        /// </summary>
        public int ShopNo { get; set; }


        public string ShopName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string Abbreviation { get; set; }

        public string ContactName { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string Salesman { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public string Remark { get; set; }

        public string Logo { get; set; }
    }
}
