using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Common.Extensions;
using TrueWays.Core.Models;
using TrueWays.Core.Repository;
using TrueWays.Core.Utilities;

namespace TrueWays.Core.Service
{
    public class UserService : Singleton<UserService>
    {
        private const string PassWordSplitString = "admin#trueWays#com";
        private readonly UserInfoRepository _userInfoRepository = new UserInfoRepository();

        private UserService()
        {
        }


        public UserInfo GetUserInfo(object condition)
        {
            return _userInfoRepository.Get(condition);
        }


        public List<UserInfo> GetPageList(object condition, int page, int pageSize, out int totalItem)
        {
            return _userInfoRepository.GetPageList(condition, "", page, pageSize, out totalItem).ToList();
        }

        public UserInfo Login(string loginName, string passWord, out bool isPass)
        {
            var user = _userInfoRepository.Get(new {loginName});

            if (user == null)
            {
                isPass = false;
                return null;
            }

            isPass =
                string.Concat(user.LoginName, PassWordSplitString, user.UserRole.GetHashCode(), passWord)
                    .Hmacsha1(user.SaltValue)
                    .Equals(user.PassWord);
            return user;
        }

        public int UpdatePassWord(UserInfo user, string oldPwd, string newPwd)
        {
            if (!string.Concat(user.UserName, PassWordSplitString, user.UserRole.GetHashCode(), oldPwd)
                .Hmacsha1(user.SaltValue).Equals(user.PassWord))
            {
                return 2;
            }

            var newPassWord =
                string.Concat(user.UserName, PassWordSplitString, user.UserRole.GetHashCode(), newPwd)
                    .Hmacsha1(user.SaltValue);

            return _userInfoRepository.Update(new {passWord = newPassWord}, new {user.UserId}) ? 1 : 0;
        }

        //注册用户
        public int RegisterUser(UserInfo model)
        {
            if (_userInfoRepository.Count(new {model.UserName}) > 0)
            {
                return 2;
            }

            model.SaltValue = StringExtensions.GetRandomString();
            model.Mobile = model.Mobile ?? string.Empty;
            model.Phone = model.Phone ?? string.Empty;
            model.PassWord =
                string.Concat(model.LoginName, PassWordSplitString, model.UserRole.GetHashCode(), model.PassWord)
                    .Hmacsha1(model.SaltValue);
            model.CreateDate = DateTime.Now;

            return _userInfoRepository.Insert(model) > 0 ? 1 : 0;
        }

        public bool Update(object model, object condition)
        {
            return _userInfoRepository.Update(model, condition);
        }
    }
}
