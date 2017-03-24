using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TrueWays.Core.Models.Result
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiPageList<T>
    {
        public ApiPageList()
        {
            PageList = Enumerable.Empty<T>();
        }

        /// <summary>
        /// 页码(从1开始)
        /// </summary>
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 总数据量
        /// </summary>
        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonProperty(PropertyName = "pageCount")]
        public int PageCount
        {
            get
            {
                if (TotalCount < 1 || PageSize < 1)
                {
                    return 0;
                }
                return (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            }
        }

        [JsonProperty(PropertyName = "pageList")]
        public IEnumerable<T> PageList { get; set; }


        public virtual string GetPagerHtml(string urlFormat)
        {
            List<int> alPages = CalculateBeginAndEnd(TotalCount, PageSize, Page, 5);

            if (1 >= alPages.Count) return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append("<ul class=\"pagination\">");

            if (Page > 1)
            {
                sb.AppendFormat("<li><a href=\"{0}\">上一页</a></li>", string.Format(urlFormat, Page - 1));
            }

            foreach (int i in alPages)
            {
                if (i == Page)
                {
                    sb.AppendFormat("<li class=\"active\"><a href=\"{0}\">{1}</a></li>", string.Format(urlFormat, i), i);
                }
                else
                {
                    sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", string.Format(urlFormat, i), i);
                }
            }

            if (Page < PageCount)
            {
                sb.AppendFormat("<li><a href=\"{0}\">下一页</a></li>", string.Format(urlFormat, Page + 1), Page + 1);
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取页面中需要显示的页码
        /// </summary>
        /// <param name="totalRecords">记录数</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="stepNum">当前页左右要显示页码数</param>
        /// <returns>需要显示的页码</returns>
        private List<int> CalculateBeginAndEnd(int totalRecords, int pageSize, int pageIndex, int stepNum)
        {
            List<int> list = new List<int>();
            int intBegin = 0;
            int intEnd = 0;

            if (PageCount == 0 || pageIndex < 1 || PageCount < pageIndex)
                return list;

            stepNum = stepNum < 1 ? 1 : stepNum;

            intBegin = pageIndex - stepNum;
            intEnd = pageIndex + stepNum;

            if (intBegin < 1)
            {
                intEnd -= intBegin;

                intBegin = 1;
            }

            if (intEnd > PageCount)
            {
                intBegin -= intEnd - PageCount - 1;

                intEnd = PageCount;
            }

            if (intBegin < 1)
            {
                intBegin = 1;
            }

            for (int i = intBegin; i <= intEnd; i++)
            {
                list.Add(i);
            }

            return list;
        }
    }
}