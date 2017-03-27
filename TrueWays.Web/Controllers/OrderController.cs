using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using TrueWays.Core.ActionResultExtensions;
using TrueWays.Core.Models;
using TrueWays.Core.Models.Result;
using TrueWays.Core.Service;
using TrueWays.Web.Fillter;

namespace TrueWays.Web.Controllers
{
    [AdminAuthorize]
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index(string orderNo = "", string phone = "", string shopName = "", int orderStatus = -1,
            int page = 1, int pageSize = 20)
        {

            int totalItem;

            var list = OrderService.Instance.SearchOrders(orderNo, phone, shopName, orderStatus, page, pageSize, out totalItem);

            ViewBag.orderNo = orderNo;
            ViewBag.phone = phone;
            ViewBag.shopName = shopName;

            return View(new ApiPageList<OrderInfo>()
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalItem,
                PageList = list
            });
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult Export()
        {
            var list = OrderService.Instance.GetList(null).OrderByDescending(t => t.CreateDate).ToList();

            var workbook = new XLWorkbook();
            workbook.Worksheets.Add("Sheet1");
            var workSheet = workbook.Worksheet(1);
            workSheet.Cell(1, 1).Value = "序号";
            workSheet.Cell(1, 2).Value = "订单编号";
            workSheet.Cell(1, 3).Value = "商家名称";
            workSheet.Cell(1, 5).Value = "联系人";
            workSheet.Cell(1, 6).Value = "固定电话";
            workSheet.Cell(1, 7).Value = "手机";
            workSheet.Cell(1, 8).Value = "地址";
            workSheet.Cell(1, 9).Value = "提交时间";
            workSheet.Cell(1, 10).Value = "状态";
            workSheet.Cell(1, 11).Value = "维修师傅";
            workSheet.Cell(1, 12).Value = "价格";
            workSheet.Cell(1, 13).Value = "操作员";

            int rows = 2, index = 1;
            foreach (var model in list)
            {
                workSheet.Cell(rows, 1).Value = index++;
                workSheet.Cell(rows, 2).Value = model.OrderNo;
                workSheet.Cell(rows, 3).Value = model.ShopName;
                workSheet.Cell(rows, 4).Value = model.ContactName;
                workSheet.Cell(rows, 6).Value = model.Phone;
                workSheet.Cell(rows, 7).Value = model.Mobile;
                workSheet.Cell(rows, 8).Value = model.Address;
                workSheet.Cell(rows, 9).Value = model.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                workSheet.Cell(rows, 10).Value = model.OrderStatus == 0 ? "有效" : "无效";
                workSheet.Cell(rows, 11).Value = model.Technician;
                workSheet.Cell(rows, 12).Value = model.Price;
                workSheet.Cell(rows, 13).Value = model.HandleDate;
                rows++;
            }

            workSheet.Rows(1, 1000).Height = 20;
            workSheet.Columns(1, 100).Width = 25;
            workSheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.Green;
            workSheet.Range("A1:K1").Style.Font.SetFontColor(XLColor.Yellow);
            workSheet.Range("A1:K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            return new ExportExcelResult
            {
                WorkBook = workbook,
                FileName = "客户" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx"
            };
        }
    }
}