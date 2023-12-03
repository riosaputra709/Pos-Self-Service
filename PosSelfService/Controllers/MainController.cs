using PosSelfService.Common;
using PosSelfService.Models;
using PosSelfService.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ActionResult TambahKeKeranjang(string prdcd, int qty, List<AdditionalRequestModel> bahan)
        {
            List<KeranjangModel> result = new List<KeranjangModel>();
            
            try
            {
                if (qty < 1)
                {
                    throw new Exception("Jumlah harus lebih dari 0");
                }

                if (bahan == null)
                {
                    bahan = new List<AdditionalRequestModel>();
                }

                //cari data dari session kumpulan barang
                var data = Session["objKumpulanBarangMain"] as ClsKumpulanBarang;
                ClsBarang filteredData = new ClsBarang();
                filteredData = data.DaftarBarang.FirstOrDefault(p => p.PLU == prdcd);

                if (filteredData == null)
                {
                    throw new Exception("Produk yang ingin ditambahkan tidak ada");
                }

                KeranjangModel cart = new KeranjangModel();

                cart.prdcd = prdcd;
                cart.qty = qty;
                cart.name = filteredData.Nama;
                cart.price = int.Parse(filteredData.HargaJualRp.ToString()) * qty;
                cart.image = filteredData.ProductImageString;
                cart.additionalRequests = bahan;

                if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
                {
                    result = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session keranjang

                    int indexListCart = result.FindIndex(item => (item.prdcd == prdcd) && (item.additionalRequests.SequenceEqual(cart.additionalRequests) == true));
                    if (indexListCart > -1)
                    {
                        KeranjangModel itemToUpdate = result[indexListCart];
                        itemToUpdate.qty += qty;
                        itemToUpdate.price += cart.price;
                    }
                    else
                    {
                        result.Add(cart);
                    }
                }
                else
                {
                    result.Add(cart);
                }

                Session["keranjang_belanja"] = result; //perbarui panel cart
                ViewData["cartlist"] = result;

                return PartialView("_PanelCart");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("PanelCartError", ex.Message);
                return PartialView("_PanelCart");
            }

            
        }

        public ActionResult UpdateKeKeranjang(string prdcd, int qty)
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
            var objMasr = Session["objMasr"] as List<ClsMasr>;
            var objMmsr = Session["objMmsr"] as List<ClsMmsr>;

            List<ClsMasr> groupKelMasr = new List<ClsMasr>();
            List<String> groupKelMmsr = new List<String>();

            List<ClsBarang> produkList = data.DaftarBarang.Where(p => p.PLU == id).ToList();

            ClsBarang produk = new ClsBarang();
            List<ClsMasr> filteredListMasr = new List<ClsMasr>();
            List<ClsMmsr> filteredListMmsr = new List<ClsMmsr>();
            if (produkList.Count > 0)
            {
                produk = produkList[0];

                filteredListMasr = objMasr.Where(p => p.PLU_JUAL == id).ToList();
                groupKelMasr = filteredListMasr.GroupBy(p => p.KEL_SPCL_REQ).Select(group => group.First()).ToList();

                filteredListMmsr = objMmsr.Where(p => p.PLU_JUAL == id).ToList();
                groupKelMmsr = filteredListMmsr.GroupBy(p => p.PLU_BHN_BAKU).Select(group => group.First().PLU_BHN_BAKU).ToList();
            }

            ViewData["produk"] = produk;
            ViewData["listMasr"] = filteredListMasr;
            ViewData["groupMasr"] = groupKelMasr;
            ViewData["listMmsr"] = filteredListMmsr;
            ViewData["groupMmsr"] = groupKelMmsr;
            return View();
        }


    }
}