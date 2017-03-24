using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return null;
        }

        public UserInfo Login(string userName, string passWord, out bool isPass)
        {
            var user = _userInfoRepository.Get(new {userName});

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
    }
}
