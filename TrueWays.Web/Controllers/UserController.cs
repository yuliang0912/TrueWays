using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrueWays.Core;
using TrueWays.Core.Models;
using TrueWays.Core.Models.Result;
using TrueWays.Core.Service;
using TrueWays.Web.Fillter;

namespace TrueWays.Web.Controllers
{
    [AdminAuthorize(UserRole.系统管理员)]
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index(string userName = "", int page = 1, int pageSize = 20)
        {
            int totalItem;
            var list =
                string.IsNullOrWhiteSpace(userName)
                    ? UserService.Instance.GetPageList(null, page, pageSize, out totalItem)
                    : UserService.Instance.GetPageList(new {userName}, page, pageSize, out totalItem);

            ViewBag.userName = userName;
            return View(new ApiPageList<UserInfo>()
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

        public ActionResult Edit(int userId)
        {
            var userInfo = UserService.Instance.GetUserInfo(new {userId});
            if (userInfo == null)
            {
                return Content("参数错误,未找到用户");
            }
            return View(userInfo);
        }


        [HttpPost]
        public JsonResult AddUser(UserInfo model)
        {
            var result = UserService.Instance.RegisterUser(model);
            return Json(result);
        }


        [HttpPost]
        public JsonResult EditUser(UserInfo model)
        {
            var userInfo = UserService.Instance.GetUserInfo(new {model.UserId});
            if (userInfo == null)
            {
                return Json(new ApiResult<int>(2) {ErrorCode = 1, Message = "用户信息错误"});
            }
            var newPwd = model.PassWord;
            if (string.IsNullOrWhiteSpace(model.PassWord))
            {
                model.PassWord = userInfo.PassWord;
            }
            model.SaltValue = userInfo.SaltValue;
            var result = UserService.Instance.Update(model, newPwd);
            return Json(result);
        }
    }
}