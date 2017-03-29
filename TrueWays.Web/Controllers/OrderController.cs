using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using TrueWays.Core;
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

            var list = OrderService.Instance.SearchOrders(orderNo, phone, shopName, orderStatus, page, pageSize,
                out totalItem);

            ViewBag.orderNo = orderNo;
            ViewBag.phone = phone;
            ViewBag.shopName = shopName;
            ViewBag.orderStatus = orderStatus;

            return View(new ApiPageList<OrderInfo>()
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalItem,
                PageList = list
            });
        }

        public ActionResult Edit(int orderId)
        {
            var orderInfo = OrderService.Instance.Get(new { orderId });
            if (orderInfo == null)
            {
                return Content("参数错误,未找到订单");
            }
            return View(orderInfo);
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
            workSheet.Cell(1, 4).Value = "联系人";
            workSheet.Cell(1, 5).Value = "固定电话";
            workSheet.Cell(1, 6).Value = "手机";
            workSheet.Cell(1, 7).Value = "地址";
            workSheet.Cell(1, 8).Value = "提交时间";
            workSheet.Cell(1, 9).Value = "状态";
            workSheet.Cell(1, 10).Value = "维修师傅";
            workSheet.Cell(1, 11).Value = "价格";
            workSheet.Cell(1, 12).Value = "操作员";
            workSheet.Cell(1, 13).Value = "备注";
            workSheet.Cell(1, 14).Value = "沟通记录";
            workSheet.Cell(1, 15).Value = "故障内容";

            int rows = 2, index = 1;
            foreach (var model in list)
            {
                workSheet.Cell(rows, 1).Value = index++;
                workSheet.Cell(rows, 2).Value = model.OrderNo;
                workSheet.Cell(rows, 3).Value = model.ShopName;
                workSheet.Cell(rows, 4).Value = model.ContactName;
                workSheet.Cell(rows, 5).Value = model.Phone;
                workSheet.Cell(rows, 6).Value = model.Mobile;
                workSheet.Cell(rows, 7).Value = model.Address;
                workSheet.Cell(rows, 8).Value = model.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                workSheet.Cell(rows, 9).Value = model.OrderStatus.ToString();
                workSheet.Cell(rows, 10).Value = model.Technician;
                workSheet.Cell(rows, 11).Value = model.Price;
                workSheet.Cell(rows, 12).Value = model.HandleName;
                workSheet.Cell(rows, 13).Value = model.Remark;
                workSheet.Cell(rows, 14).Value = model.CommunicationRecord;
                workSheet.Cell(rows, 15).Value = model.FaultContent;
                rows++;
            }

            workSheet.Rows(1, 1000).Height = 20;
            workSheet.Columns(1, 100).Width = 25;
            workSheet.Range("A1:M1").Style.Fill.BackgroundColor = XLColor.Green;
            workSheet.Range("A1:M1").Style.Font.SetFontColor(XLColor.Yellow);
            workSheet.Range("A1:M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            return new ExportExcelResult
            {
                WorkBook = workbook,
                FileName = "订单" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx"
            };
        }

        public JsonResult UpdateStatus(int orderId, int orderStatus)
        {
            var result = OrderService.Instance.Update(new { orderStatus }, new { orderId });

            return Json(result);
        }

        [HttpPost]
        public JsonResult EditOrder(UserInfo user, OrderInfo order, int orderStatus)
        {
            var orderInfo = OrderService.Instance.Get(order.OrderId);
            if (orderInfo == null)
            {
                return Json(new ApiResult<int>(2) { ErrorCode = 1, Message = "订单信息错误" });
            }
            if (orderInfo.Price < 0)
            {
                return Json(new ApiResult<int>(3) { ErrorCode = 1, Message = "价格不能小于0" });
            }
            if (orderStatus == -1)
            {
                order.OrderStatus = orderInfo.OrderStatus;
            }
            else
            {
                order.OrderStatus = (OrderStatus)orderStatus;
            }
            order.Remark = order.Remark ?? string.Empty;
            order.ShopName = order.ShopName ?? string.Empty;
            order.ContactName = order.ContactName ?? string.Empty;
            order.Phone = order.Phone ?? string.Empty;
            order.Mobile = order.Mobile ?? string.Empty;
            order.Address = order.Address ?? string.Empty;
            order.Technician = order.Technician ?? string.Empty;
            order.FaultContent = order.FaultContent ?? string.Empty;
            order.CommunicationRecord = order.CommunicationRecord ?? string.Empty;

            var result = OrderService.Instance.Update(new
            {
                order.ShopName,
                order.ContactName,
                order.Phone,
                order.Mobile,
                order.Address,
                order.Price,
                order.Technician,
                order.FaultContent,
                order.Remark,
                order.CommunicationRecord,
                handleDate = DateTime.Now,
                handleName = user.UserName,
                orderStatus = order.OrderStatus.GetHashCode()
            }, new { order.OrderId });
            return Json(result);
        }

        public JsonResult CloseOrder(UserInfo user, int orderId)
        {
            var order = OrderService.Instance.Get(new { orderId });
            if (order == null)
            {
                return Json(new ApiResult<int>(2) { ErrorCode = 1, Message = "未找到订单" });
            }
            if (order.OrderStatus == OrderStatus.交易关闭)
            {
                return Json(true);
            }

            var result = OrderService.Instance.Update(new
            {
                handleName = user.UserName,
                handleDate = DateTime.Now,
                orderStatus = OrderStatus.交易关闭.GetHashCode()
            }, new { orderId });
            return Json(result);
        }
    }
}