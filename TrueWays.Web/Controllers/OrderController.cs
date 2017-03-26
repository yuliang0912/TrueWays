using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrueWays.Web.Fillter;

namespace TrueWays.Web.Controllers
{
    [AdminAuthorize]
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
    }
}