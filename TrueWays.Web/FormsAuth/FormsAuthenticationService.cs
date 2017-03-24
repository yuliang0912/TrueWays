using System.Linq;
using TrueWays.Core;
using TrueWays.Core.Utilities;

namespace TrueWays.Web.FormsAuth
{
    /// <summary>
    /// 解密cookie中的用户信息
    /// </summary>
    internal class FormsAuthenticationService : Singleton<FormsAuthenticationService>
    {
        private FormsAuthenticationService()
        {
        }

        public string UserId => GetAuthenticatedUserKeyId();

        private static string GetAuthenticatedUserKeyId()
        {
            var httpContext = System.Web.HttpContext.Current;

            var authCookie = httpContext.Request.Cookies["authKey"];
            var skeyCookie = httpContext.Request.Cookies["skey"];

            if (null == authCookie || null == skeyCookie || string.IsNullOrEmpty(authCookie.Value) ||
                string.IsNullOrEmpty(skeyCookie.Value))
            {
                return null;
            }

            var authTicket =
                authCookie.Value.Decrypt<System.Security.Cryptography.TripleDESCryptoServiceProvider>(
                    ConfigHelper.GetAppSettingString("authKey"));

            var keyArrays = authTicket.Split('|', '@', '|');

            return keyArrays.Any(t => t.Equals(skeyCookie.Value))
                ? keyArrays.First()
                : null;
        }
    }
}
