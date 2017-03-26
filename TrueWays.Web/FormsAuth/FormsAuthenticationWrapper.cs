using System;
using System.Web;
using TrueWays.Core;
using TrueWays.Core.Utilities;

namespace TrueWays.Web.FormsAuth
{
    /// <summary>
    /// 加密写用户cookie
    /// </summary>
    internal class FormsAuthenticationWrapper : Singleton<FormsAuthenticationWrapper>
    {
        private FormsAuthenticationWrapper()
        {
        }

        private const string CurrDomain = "www.trueways.com";

        public void SetAuthCookie(string userKeyId, bool createPersistentCookie)
        {
            var skey = Guid.NewGuid().ToString("N").Substring(0, 20);

            var authCookie = new HttpCookie("authKey")
            {
                Value =
                    DecryptHelper.TripleDesc(ConfigHelper.GetAppSettingString("authKey"),
                        userKeyId + "|@|" + skey),
                Domain = CurrDomain
            };

            var sKey = new HttpCookie("skey")
            {
                Value = skey,
                Domain = CurrDomain
            };
            if (createPersistentCookie)
            {
                authCookie.Expires = DateTime.Now.AddDays(30);
                sKey.Expires = DateTime.Now.AddDays(30);
            }

            HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpContext.Current.Response.Cookies.Add(sKey);
        }

        public void SignOut()
        {
            var authCookie = HttpContext.Current.Request.Cookies["authKey"];
            if (authCookie != null)
            {
                authCookie.Value = null;
                authCookie.Path = "/";
                authCookie.Domain = CurrDomain;
                authCookie.Expires = DateTime.Now.AddYears(-30);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }

            var skeyCookie = HttpContext.Current.Request.Cookies["skey"];
            if (skeyCookie == null) return;

            skeyCookie.Value = null;
            skeyCookie.Domain = CurrDomain;
            skeyCookie.Expires = DateTime.Now.AddYears(-30);
            HttpContext.Current.Response.Cookies.Add(skeyCookie);
        }
    }
}
