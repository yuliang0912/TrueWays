using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using TrueWays.Core;
using TrueWays.Core.ActionResultExtensions;
using TrueWays.Core.Models;
using TrueWays.Core.Models.Result;
using TrueWays.Core.Service;
using TrueWays.Core.Utilities;
using TrueWays.Web.Fillter;

namespace TrueWays.Web.Controllers
{
    public class CustomerController : BaseController
    {
        [AdminAuthorize]
        public ActionResult Index(string keyWords = "", string phone = "", int status = -1, int page = 1,
            int pageSize = 20)
        {
            int totalItem;

            var list = CustomerService.Instance.SearchCustomers(keyWords, phone, status, page, pageSize, out totalItem);

            ViewBag.keyWords = keyWords;
            ViewBag.phone = phone;
            ViewBag.status = status;

            return View(new ApiPageList<CustomerInfo>()
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalItem,
                PageList = list
            });
        }

        [AdminAuthorize]
        public ActionResult Add()
        {
            return View();
        }

        [AdminAuthorize]
        public ActionResult Edit(int id)
        {
            var model = CustomerService.Instance.Get(new {customerId = id});
            if (model == null)
            {
                return Content("参数错误");
            }
            return View(model);
        }


        public ActionResult QrCode(int id)
        {
            var model = CustomerService.Instance.Get(new { customerId = id});
            if (model == null)
            {
                return Content("二维码错误,请联系客服");
            }
            var lastOrder = OrderService.Instance.GetLast(new {model.CustomerId, orderStatus = 0});
            ViewBag.showCancel = lastOrder != null;
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AddOrder()
        {
            var customerId = Convert.ToInt32(Request.Form["customerId"] ?? string.Empty);
            var model = CustomerService.Instance.Get(new {customerId});
            if (model == null)
            {
                return Json(new ApiResult<int>(2)
                {
                    ErrorCode = 1,
                    Message = "数据错误!"
                });
            }

            var order = new OrderInfo()
            {
                OrderNo = "M" + Core.Common.Extensions.StringExtensions.GetLongNo(),
                CustomerId = model.CustomerId,
                ContactName = model.ContactName,
                Phone = model.Phone,
                Mobile = model.Mobile,
                ShopName = model.ShopName,
                Address = model.Address,
                OrderStatus = OrderStatus.待受理
            };
            var result = OrderService.Instance.CreateOrder(order);
            return Json(result);
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult CreateQrCode(int customerId)
        {
            var model = CustomerService.Instance.Get(new {customerId});
            if (model == null)
            {
                return Content("参数错误");
            }

            using (var ms = new MemoryStream())
            {
                QrCodeHelper.GetQrCode("http://service.trueways.com/qrcode/" + customerId, ms);
                var httpResponse = HttpContext.Response;
                httpResponse.Clear();
                httpResponse.Buffer = true;
                httpResponse.Charset = Encoding.UTF8.BodyName;
                httpResponse.AppendHeader("Content-Disposition", "attachment;filename=" + model.ShopName + ".png");
                httpResponse.ContentEncoding = Encoding.UTF8;
                httpResponse.ContentType = "application/x-plt; charset=UTF-8";
                ms.WriteTo(httpResponse.OutputStream);
            }

            return null;
        }

        /// <summary>
        /// 下载二维码压缩包
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public void DownLoad()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "Content\\ImageFiles\\";

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
                Directory.CreateDirectory(baseDirectory + "QrCode\\");
            }

            var qrCodePackageDirectory = baseDirectory + "\\download\\";
            if (Directory.Exists(qrCodePackageDirectory))
            {
                Directory.Delete(qrCodePackageDirectory, true);
            }
            Directory.CreateDirectory(qrCodePackageDirectory);
            Directory.CreateDirectory(qrCodePackageDirectory + "\\二维码\\");

            var idList = new List<int> {1, 2, 3, 4, 5, 6, 7, 8};

            var qrCodeDirectory = baseDirectory + "QrCode\\";
            foreach (var item in idList.Where(item => !System.IO.File.Exists(qrCodeDirectory + item + ".png")))
            {
                using (var ms = new MemoryStream())
                {
                    QrCodeHelper.GetQrCode("http://www.ciwong.com/qrcode/" + item, ms);
                    var image = Image.FromStream(ms);
                    image.Save(qrCodeDirectory + item + ".png");
                }
            }

            foreach (var item in idList)
            {
                System.IO.File.Copy(qrCodeDirectory + item + ".png", qrCodePackageDirectory + "二维码//" + item + ".png",
                    true);
            }

            ZipHelper.ZipFileDirectory(qrCodePackageDirectory, qrCodePackageDirectory + "qrCode.zip");

            var httpResponse = HttpContext.Response;
            httpResponse.Clear();
            httpResponse.Buffer = true;
            httpResponse.Charset = Encoding.UTF8.BodyName;
            httpResponse.AppendHeader("Content-Disposition",
                "attachment;filename=二维码_" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip");
            httpResponse.ContentEncoding = Encoding.UTF8;
            httpResponse.ContentType = "application/x-zip-compressed; charset=UTF-8";

            using (var files =
                new FileStream(qrCodePackageDirectory + "qrCode.zip", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var byteFile = files.Length == 0 ? new byte[1] : new byte[files.Length];
                files.Read(byteFile, 0, byteFile.Length);
                files.Close();
                httpResponse.BinaryWrite(byteFile);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult Export()
        {
            var list = CustomerService.Instance.GetList(null).OrderBy(t => t.CreateDate).ToList();

            var workbook = new XLWorkbook();
            workbook.Worksheets.Add("Sheet1");
            var workSheet = workbook.Worksheet(1);
            workSheet.Cell(1, 1).Value = "序号";
            workSheet.Cell(1, 2).Value = "商家编码";
            workSheet.Cell(1, 3).Value = "商家名称";
            workSheet.Cell(1, 4).Value = "简称";
            workSheet.Cell(1, 5).Value = "联系人";
            workSheet.Cell(1, 6).Value = "固定电话";
            workSheet.Cell(1, 7).Value = "手机";
            workSheet.Cell(1, 8).Value = "地址";
            workSheet.Cell(1, 9).Value = "状态";
            workSheet.Cell(1, 10).Value = "业务员";
            workSheet.Cell(1, 11).Value = "创建时间";

            int rows = 2, index = 1;
            foreach (var model in list)
            {
                workSheet.Cell(rows, 1).Value = index++;
                workSheet.Cell(rows, 2).Value = model.ShopNo;
                workSheet.Cell(rows, 3).Value = model.ShopName;
                workSheet.Cell(rows, 4).Value = model.Abbreviation;
                workSheet.Cell(rows, 5).Value = model.ContactName;
                workSheet.Cell(rows, 6).Value = model.Phone;
                workSheet.Cell(rows, 7).Value = model.Mobile;
                workSheet.Cell(rows, 8).Value = model.Address;
                workSheet.Cell(rows, 9).Value = model.Status == 0 ? "有效" : "无效";
                workSheet.Cell(rows, 10).Value = model.Salesman;
                workSheet.Cell(rows, 11).Value = model.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
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

        [HttpPost]
        [AdminAuthorize]
        public JsonResult AddCustomer(HttpPostedFileBase file, CustomerInfo model)
        {
            if (file != null && (string.Empty.Equals(file.FileName) || !file.ContentType.StartsWith("image/")))
            {
                return Json(new ApiResult<int>(2)
                {
                    ErrorCode = 1,
                    Message = "当前文件格式不正确,请确保正确的图片!"
                });
            }

            if (file != null)
            {
                var fileName = Guid.NewGuid() +
                               file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal));

                file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "Content\\ImageFiles\\Logo\\" + fileName);

                model.Logo = "/Content/ImageFiles/Logo/" + fileName;
            }

            var result = CustomerService.Instance.Create(model);

            return Json(result);
        }

        [HttpPost]
        [AdminAuthorize]
        public JsonResult EditCustomer(HttpPostedFileBase file, CustomerInfo model)
        {
            if (file != null && (string.Empty.Equals(file.FileName) || !file.ContentType.StartsWith("image/")))
            {
                return Json(new ApiResult<int>(2)
                {
                    ErrorCode = 1,
                    Message = "当前文件格式不正确,请确保正确的图片!"
                });
            }

            var customer = CustomerService.Instance.Get(new {model.CustomerId});

            if (customer == null)
            {
                return Json(new ApiResult<int>(3)
                {
                    ErrorCode = 1,
                    Message = "参数错误!"
                });
            }

            if (file != null)
            {
                var fileName = Guid.NewGuid() +
                               file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal));

                file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "Content\\ImageFiles\\Logo\\" + fileName);

                model.Logo = "/Content/ImageFiles/Logo/" + fileName;
            }
            else
            {
                model.Logo = Request.Form["deleteLogo"] == "1" ? string.Empty : customer.Logo;
            }

            var result = CustomerService.Instance.Update(new
            {
                model.ContactName,
                model.ShopName,
                model.Address,
                model.Remark,
                model.Salesman,
                model.Status,
                model.Abbreviation,
                model.Logo,
            }, new {model.CustomerId});

            return Json(result ? 1 : 0);
        }
    }
}