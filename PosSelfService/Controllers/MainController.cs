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
            ProdukModel produkModel = new ProdukModel();
            produkModel.MERK = "POINT COFFEE";
            DoSearch(produkModel);
            return View();
        }

        private void DoSearch(ProdukModel produkModel)
        {
            IList<ProdukModel> listData = mainRepo.SearchProduk(produkModel);
            ViewData["produklist"] = listData;
        }

        public ActionResult Search(ProdukModel produk)
        {
            try
            {
                DoSearch(produk);
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_GridView");
        }
    }
}