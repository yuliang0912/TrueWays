using System;

namespace TrueWays.Core.Models
{
    public class OrderInfo
    {
        public int OrderId { get; set; }

        public string OrderNo { get; set; }

        public string ShopName { get; set; }

        public string ContactName { get; set; }

        /// <summary>
        /// 固话
        /// </summary>
        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// 技师
        /// </summary>
        public string Technician { get; set; }

        /// <summary>
        /// 故障内容
        /// </summary>
        public string FaultContent { get; set; }

        /// <summary>
        /// 沟通记录
        /// </summary>
        public string CommunicationRecord { get; set; }


        public string Remark { get; set; }


        public DateTime CreateDate { get; set; }

        public string HandleName { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime HandleDate { get; set; }

        public DateTime EndDate { get; set; }

        public int OrderStatus { get; set; }
    }
}
