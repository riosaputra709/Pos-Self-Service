using PosSelfService.Models;
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

        public ActionResult UpdateKeKeranjang(int id, int qty)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

                int indexListCart = listCart.FindIndex(item => item.id == id); //mencari data yang sama di keranjang

                KeranjangModel itemToUpdate = listCart[indexListCart];
                int hargasatuan = itemToUpdate.price / itemToUpdate.qty;
                itemToUpdate.qty = qty;
                itemToUpdate.price = hargasatuan * qty;

                Session["keranjang_belanja"] = listCart; //perbarui panel cart

                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch(Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }

            return Json(ajaxResult);
        }

        public ActionResult HapusProdukKeranjang(int id)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

                int indexListCart = listCart.FindIndex(item => item.id == id); //mencari data yang sama di keranjang
                listCart.RemoveAt(indexListCart);

                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch (Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }

            return Json(ajaxResult);

        }
    }
}