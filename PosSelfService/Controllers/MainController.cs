using PosSelfService.Common;
using PosSelfService.Models;
using PosSelfService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosSelfService.Controllers
{
    public class MainController : Controller
    {
        private MainRepository mainRepo = MainRepository.Instance;
        // GET: Main
        public ActionResult Index()
        {
            DoSearch("POINT COFFEE", "");
            return View();
        }

        private void DoSearch(string merk, string nama)
        {
            var data = new ClsKumpulanBarang();
            var filteredData = new List<ClsBarang>();

            if (System.Web.HttpContext.Current.Session["objKumpulanBarangMain"] != null)
            {
                data = Session["objKumpulanBarangMain"] as ClsKumpulanBarang;

                if (String.IsNullOrEmpty(nama))
                {
                    filteredData = data.DaftarBarang.Where(p => p.Merk == merk.ToUpper()).ToList();
                }
                else
                {
                    filteredData = data.DaftarBarang.Where(p => p.Merk == merk.ToUpper() && p.Nama.Contains(nama.ToUpper())).ToList();
                }
            }

            ViewData["produklist"] = filteredData;
        }

        public ActionResult Search(ProdukModel produk)
        {
            try
            {
                DoSearch(produk.MERK, produk.NAMA);
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_GridView");
        }

        public ActionResult Specialadditional(string id)
        {
            var data = Session["objKumpulanBarangMain"] as ClsKumpulanBarang;

            List<ClsBarang> produkList = data.DaftarBarang.Where(p => p.PLU == id).ToList();

            ClsBarang produk = new ClsBarang();
            if (produkList.Count > 0)
            {
                produk = produkList[0];
            }

            ViewData["produk"] = produk;
            return View();
        }


    }
}