using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TrueWays.Core;
using TrueWays.Core.Models;
using TrueWays.Core.Models.Result;
using TrueWays.Core.Service;
using TrueWays.Core.Utilities;
using TrueWays.Web.Fillter;
using TrueWays.Web.FormsAuth;

namespace TrueWays.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Home()
        {
            UserService.Instance.RegisterUser(new UserInfo()
            {
                UserName = "管理员",
                LoginName = "admin",
                PassWord = "123456",
                UserRole = UserRole.系统管理员,
                Phone = "7771730",
                Mobile = "18923803593"
            });
            return View();
        }

        public ActionResult QrCode()
        {
            using (var ms = new MemoryStream())
            {
                QrCodeHelper.GetQrCode("http://www.baidu.com", ms);
                HttpResponseBase httpResponse = HttpContext.Response;
                httpResponse.Clear();
                httpResponse.Buffer = true;
                httpResponse.Charset = Encoding.UTF8.BodyName;
                httpResponse.AppendHeader("Content-Disposition", "attachment;filename=1.png");
                httpResponse.ContentEncoding = Encoding.UTF8;
                httpResponse.ContentType = "application/x-plt; charset=UTF-8";

                ms.WriteTo(httpResponse.OutputStream);
            }

            return null;
        }


        public JsonResult LoginCheck(string loginName, string passWord)
        {
            bool isPass;
            var userInfo = UserService.Instance.Login(loginName, passWord, out isPass);
            if (isPass)
            {
                FormsAuthenticationWrapper.Instance.SetAuthCookie(userInfo.UserId.ToString(), true);
            }
            return Json(isPass, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize, HttpGet]
        public ActionResult UpdatePassWord(UserInfo user, string oldPwd, string newPwd)
        {
            if (string.IsNullOrWhiteSpace(oldPwd) || string.IsNullOrWhiteSpace(newPwd))
            {
                return Json(new ApiResult<int>(3) {Ret = 1, Message = "密码不能为空"});
            }

            return Json(UserService.Instance.UpdatePassWord(user, oldPwd, newPwd));
        }

        public ActionResult Logout()
        {
            FormsAuthenticationWrapper.Instance.SignOut();
            return Redirect("/home/login");
        }
    }
}