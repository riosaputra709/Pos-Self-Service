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
            List<KeranjangModel> listCart = new List<KeranjangModel>();

            if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
            {
                listCart = Session["keranjang_belanja"] as List<KeranjangModel>;
            }
            ViewData["cartlist"] = listCart;

            int totalHarga = 0;
            for (int i = 0; i < listCart.Count; i++)
            {
                totalHarga += listCart[i].price;
            }
            ViewData["totalHarga"] = totalHarga;

            return View();
        }

        public ActionResult PembayaranKartu()
        {
            List<KeranjangModel> listCart = new List<KeranjangModel>();

            if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
            {
                listCart = Session["keranjang_belanja"] as List<KeranjangModel>;
            }
            ViewData["cartlist"] = listCart;

            int totalHarga = 0;
            for (int i = 0; i < listCart.Count; i++)
            {
                totalHarga += listCart[i].price;
            }
            ViewData["totalHarga"] = totalHarga;

            return View();
        }

        public ActionResult Selesai()
        {
            return View();
        }
    }
}