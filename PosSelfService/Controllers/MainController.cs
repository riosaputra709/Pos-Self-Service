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
        public ProdukModel produkHold = new ProdukModel();
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
                produkHold.NAMA = produk.NAMA.ToUpper();

                string session;
                if (System.Web.HttpContext.Current.Session["produk_nama"] != null)
                {
                    session = System.Web.HttpContext.Current.Session["produk_nama"].ToString();
                }
                Session["produk_nama"] = String.IsNullOrEmpty(produk.NAMA) ? "" : produk.NAMA.ToUpper();
                session = Session["produk_nama"].ToString();



                DoSearch(produk);
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_GridView");
        }

        public ActionResult Specialadditional(int id)
        {
            ProdukModel produk = mainRepo.GetByKeyWithDtl(id);

            ViewData["produk"] = produk;
            return View();
        }

        /*public JsonResult GetById(int id)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ProdukModel result = null;

            try
            {
                result = mainRepo.GetByKeyWithDtl(id);

                if (result == null)
                {
                    ajaxResult.Result = AjaxResult.VALUE_ERROR;
                    ajaxResult.ErrMesgs = new string[] {
                        string.Format("no data with the selected key found," +
                        "plaese refresh the screen first")
                    };
                    return Json(ajaxResult);
                }

                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
                ajaxResult.Params = new object[] {
                    result
                };
            }
            catch (Exception ex)
            {
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
                ajaxResult.ErrMesgs = new string[] {
                    string.Format("{0} = {1}", ex.GetType().FullName, ex.Message)
                };
            }
            return Json(ajaxResult);
        }*/


    }
}