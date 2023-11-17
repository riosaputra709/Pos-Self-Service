using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosSelfService.Controllers
{
    public class KeranjangController : Controller
    {
        // GET: Keranjang
        public ActionResult Index()
        {
            return View();
        }
    }
}