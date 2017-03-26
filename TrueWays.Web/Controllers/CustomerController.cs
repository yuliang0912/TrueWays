using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TrueWays.Core.Models;
using TrueWays.Core.Models.Result;
using TrueWays.Core.Service;
using TrueWays.Core.Utilities;
using TrueWays.Web.Fillter;

namespace TrueWays.Web.Controllers
{
    [AdminAuthorize]
    public class CustomerController : BaseController
    {
        // GET: Customer
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

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult QrCode(int id)
        {
            return View();
        }

        //[AdminAuthorize]
        public ActionResult DownLoad()
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
            foreach (var item in idList)
            {
                if (System.IO.File.Exists(qrCodeDirectory + item + ".png")) continue;
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

            return null;
        }
    }
}