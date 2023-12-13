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

            int totalHarga = 0;
            int totalQty = 0;
            for (int i = 0; i < listCart.Count; i++)
            {
                totalHarga += listCart[i].price;
                totalQty += listCart[i].qty;
            }
            ViewData["totalHarga"] = totalHarga;
            ViewData["totalQty"] = totalQty;

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

        public ActionResult TambahKeKeranjang(string prdcd, int qty)
        {
            List<KeranjangModel> result = new List<KeranjangModel>();
            
            try
            {
                if (qty < 1)
                {
                    throw new Exception("Jumlah harus lebih dari 0");
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

                //cek apakah produk sebelumnya sudah ada
                if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
                {
                    result = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session keranjang
                    int indexListCart = result.FindIndex(item => item.prdcd == prdcd);

                    if (indexListCart > -1)
                    {
                        KeranjangModel itemToUpdate = result[indexListCart];
                        itemToUpdate.qty += qty;
                        itemToUpdate.price += cart.price;
                    }
                    else
                    {
                        cart.id = result[result.Count - 1].id + 1; //mengambil id paling terakhir keranjang ditambah 1
                        result.Add(cart);
                    }
                }
                else
                {
                    cart.id = 1; //karena keranjang sebelumnya tidak ada maka id nya 1
                    result.Add(cart);
                }

                Session["keranjang_belanja"] = result; //perbarui panel cart
                ViewData["cartlist"] = result;

                int totalHarga = 0;
                int totalQty = 0;
                
                for (int i = 0; i < result.Count; i++)
                {
                    totalHarga += result[i].price;
                    totalQty += result[i].qty;
                }
                ViewData["totalHarga"] = totalHarga;
                ViewData["totalQty"] = totalQty;

                return PartialView("_PanelCart");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("PanelCartError", ex.Message);
                return PartialView("_PanelCart");
            }
            
        }

        #region Panel Cart
        public ActionResult UpdateQtyKeranjangPanelCart(int id, int qty)
        {
            List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

            int indexListCart = listCart.FindIndex(item => item.id == id); //mencari data yang sama di keranjang
            
            KeranjangModel itemToUpdate = listCart[indexListCart];
            int hargasatuan = itemToUpdate.price / itemToUpdate.qty; 
            itemToUpdate.qty = qty;
            itemToUpdate.price = hargasatuan * qty;

            Session["keranjang_belanja"] = listCart; //perbarui panel cart
            ViewData["cartlist"] = listCart;

            int totalHarga = 0;
            int totalQty = 0;

            for (int i = 0; i < listCart.Count; i++)
            {
                totalHarga += listCart[i].price;
                totalQty += listCart[i].qty;
            }
            ViewData["totalHarga"] = totalHarga;
            ViewData["totalQty"] = totalQty;

            return PartialView("_PanelCart");
        }
        #endregion


        #region Special Additional Request Page
        public ActionResult SpecialAdditional(string prdcd, int qty, int id = 0)
        {
            var data = Session["objKumpulanBarangMain"] as ClsKumpulanBarang;
            var objMasr = Session["objMasr"] as List<ClsMasr>;
            var objMmsr = Session["objMmsr"] as List<ClsMmsr>;

            List<ClsMasr> groupKelMasr = new List<ClsMasr>();
            List<string> groupKelMmsr = new List<string>();

            ClsBarang produk = new ClsBarang();
            List<ClsMasr> filteredListMasr = new List<ClsMasr>();
            List<ClsMmsr> filteredListMmsr = new List<ClsMmsr>();

            List<ClsBarang> produkList = data.DaftarBarang.Where(p => p.PLU == prdcd && (p.mmsr == "Y" || p.masr == "Y")).ToList();

            if (produkList.Count > 0)
            {
                produk = produkList[0];

                filteredListMasr = objMasr.Where(p => p.PLU_JUAL == prdcd).ToList();
                groupKelMasr = filteredListMasr.GroupBy(p => p.KEL_SPCL_REQ).Select(group => group.First()).ToList();

                filteredListMmsr = objMmsr.Where(p => p.PLU_JUAL == prdcd).ToList();
                groupKelMmsr = filteredListMmsr.GroupBy(p => p.PLU_BHN_BAKU).Select(group => group.First().PLU_BHN_BAKU).ToList(); //hanya ambil plu_bhn_baku
            }
            else
            {
                return RedirectToAction("Index", "NotFound");
            }

            KeranjangModel cart = new KeranjangModel();
            if (id != 0)
            {
                List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session
                int indexListCart = listCart.FindIndex(item => item.id == id && item.prdcd == prdcd);
                if (indexListCart < 0) 
                {
                    return RedirectToAction("Index", "NotFound");
                }
                cart = listCart[indexListCart];

                List<AdditionalRequestModel> cartListMmsr = new List<AdditionalRequestModel>();
                List<AdditionalRequestModel> cartListMasr = new List<AdditionalRequestModel>();
                for (int i = 0; i < cart.additionalRequests.Count; i++)
                {
                    if (cart.additionalRequests[i].typeAdditional == "mmsr")
                    {
                        cartListMmsr.Add(cart.additionalRequests[i]);
                    }
                    else if (cart.additionalRequests[i].typeAdditional == "masr")
                    {
                        cartListMasr.Add(cart.additionalRequests[i]);
                    }
                }
                ViewData["cartAdditionalListMmsr"] = cartListMmsr;
                ViewData["cartAdditionalListMasr"] = cartListMasr;

            }
            else
            {
                ViewData["groupMasr"] = groupKelMasr;
                ViewData["groupMmsr"] = groupKelMmsr;
            }

            ViewData["produk"] = produk;
            ViewData["quantity"] = qty;
            ViewData["listMasr"] = filteredListMasr;
            ViewData["listMmsr"] = filteredListMmsr;
            
            ViewData["cartId"] = cart.id;

            return View();
        }

        public ActionResult TambahProdukSpecialRequest(string prdcd, int qty, List<AdditionalRequestModel> bahan)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                List<KeranjangModel> result = new List<KeranjangModel>();

                if (qty < 1)
                {
                    throw new Exception("Jumlah harus lebih dari 0");
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

                //cek apakah produk sebelumnya sudah ada
                if (System.Web.HttpContext.Current.Session["keranjang_belanja"] != null)
                {
                    result = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session keranjang
                    int indexListCart = -1;

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].prdcd == prdcd)
                        {
                            if (bahan != null)
                            {
                                bool cekObjAdditional = true;

                                for (int j = 0; j < result[i].additionalRequests.Count; j++)
                                {
                                    if (result[i].additionalRequests[j].kelSpecReq == bahan[j].kelSpecReq)
                                    {
                                        if (result[i].additionalRequests[j].objAdditional != bahan[j].objAdditional)
                                        {
                                            cekObjAdditional = false;
                                        }
                                    }
                                }
                                if (cekObjAdditional == true)
                                {
                                    indexListCart = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (indexListCart > -1)
                    {
                        KeranjangModel itemToUpdate = result[indexListCart];
                        itemToUpdate.qty += qty;
                        itemToUpdate.price += cart.price;
                    }
                    else
                    {
                        cart.id = result[result.Count - 1].id + 1; //mengambil id paling terakhir keranjang ditambah 1
                        result.Add(cart);
                    }
                }
                else
                {
                    cart.id = 1; //karena keranjang sebelumnya tidak ada maka id nya 1
                    result.Add(cart);
                }

                Session["keranjang_belanja"] = result; //perbarui panel cart
                ViewData["cartlist"] = result;

                int totalHarga = 0;
                int totalQty = 0;

                for (int i = 0; i < result.Count; i++)
                {
                    totalHarga += result[i].price;
                    totalQty += result[i].qty;
                }
                ViewData["totalHarga"] = totalHarga;
                ViewData["totalQty"] = totalQty;

                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch (Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }
            return Json(ajaxResult);
        }

        public ActionResult UbahProdukSpecialRequest (int id, string prdcd, int qty, List<AdditionalRequestModel> bahan)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                List<KeranjangModel> listCart = Session["keranjang_belanja"] as List<KeranjangModel>; //ambil data dari session

                int indexListCart = listCart.FindIndex(item => item.id == id && item.prdcd == prdcd);

                KeranjangModel itemToUpdate = listCart[indexListCart];
                int hargasatuan = itemToUpdate.price / itemToUpdate.qty;
                itemToUpdate.qty = qty;
                itemToUpdate.price = hargasatuan * qty;
                itemToUpdate.additionalRequests = bahan;

                Session["keranjang_belanja"] = listCart; //perbarui panel cart
                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch (Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }
            return Json(ajaxResult);
        }
        #endregion
    }
}