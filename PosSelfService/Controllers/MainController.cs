using PosSelfService.Common;
using PosSelfService.Models;
using PosSelfService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace PosSelfService.Controllers
{
    public class MainController : Controller
    {
        private MainRepository mainRepo = MainRepository.Instance;
        // GET: Main
        public ActionResult Index()
        {
            DoSearch("POINT COFFEE", "");

            //Load Panel Cart
            List<KeranjangModel> listCart = new List<KeranjangModel>();

            if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
            {
                listCart = Session["keranjang_belanja"] as List<KeranjangModel>;
            }
            ViewData["cartlist"] = listCart;

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

        public ActionResult TambahKeKeranjang(string prdcd, string name, int price, string image, int qty)
        {
            KeranjangModel cart = new KeranjangModel();
            
            cart.prdcd = prdcd;
            cart.name = name;
            cart.price = price*qty;
            cart.image = image;
            cart.qty = qty;

            List<KeranjangModel> listCart = new List<KeranjangModel>();
            
            if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
            {
                listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

                int indexListCart = listCart.FindIndex(item => item.prdcd == prdcd);
                if (indexListCart > -1) {
                    KeranjangModel itemToUpdate = listCart[indexListCart];
                    itemToUpdate.qty += qty;
                    itemToUpdate.price += cart.price;
                }
                else
                {
                    listCart.Add(cart);
                }
            }
            else
            {
                listCart.Add(cart);
            }


            Session["keranjang_belanja"] = listCart; //perbarui panel cart
            ViewData["cartlist"] = listCart;

            return PartialView("_PanelCart");
        }

        public ActionResult UpdateKeKeranjang(string prdcd, int price, int qty)
        {
            List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

            int indexListCart = listCart.FindIndex(item => item.prdcd == prdcd); //mencari data yang sama
            KeranjangModel itemToUpdate = listCart[indexListCart];
            int hargasatuan = itemToUpdate.price / itemToUpdate.qty;
            itemToUpdate.qty = qty;
            itemToUpdate.price = hargasatuan * qty;

            Session["keranjang_belanja"] = listCart; //perbarui panel cart
            ViewData["cartlist"] = listCart;

            return PartialView("_PanelCart");
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