using PosSelfService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosSelfService.Controllers
{
    public class PembayaranController : Controller
    {
        // GET: Pembayaran
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PembayaranBarcode()
        {
            return View();
        }

        public ActionResult PembayaranKartu()
        {
            return View();
        }
    }
}