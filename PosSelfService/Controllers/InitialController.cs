﻿using MySql.Data.MySqlClient;
using PosSelfService.Common;
using PosSelfService.Models;
using PosSelfService.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.EnterpriseServices.Internal;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static System.Collections.Specialized.BitVector32;

namespace PosSelfService.Controllers
{
    public class InitialController : Controller
    {
        public ClsError _ObjError = new ClsError();
        public ClsSql _ObjSQL = new ClsSql();
        public ClsFungsi _ObjFungsi = new ClsFungsi();
        public BaseRepository br = new BaseRepository();
        public static ClsKasir objKasir;

        // GET: Initial
        public ActionResult Index()
        {
            MySqlConnection MasterMcon = br.openConnection();
            try
            {
                LoadMaster(MasterMcon);
                return View();
            }
            finally
            {
                MasterMcon.Close();
            }
            
        }

        private void LoadMaster(MySqlConnection Mcon)
        {
            string station = "01";
            DataTable dt = new DataTable();
            string folderPath = Server.MapPath("~/IDMPosStandAlone/");
            _ObjError.Tracelog("Load Master Data mulai");
            string SQLQuery = "";

            #region init
            _ObjError.TraceLogDebugOnly("init 1 mulai");
            SQLQuery = "SELECT Shift, IfNull(Last_Struk, 0) as Last_Struk, Kasir_Name as Nama, NIK";
            SQLQuery += " FROM Initial";
            SQLQuery += " WHERE Tanggal = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            SQLQuery += " AND Station= '" + station + "'";
            SQLQuery += " AND Trim(IfNull(RecId, '')) = ''";
            dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

            objKasir = new ClsKasir();
            ClsInitial objInitial = new ClsInitial();
            if(dt != null)
            {
                if(dt.Rows.Count > 0)
                {
                    objInitial.Tanggal = DateTime.Now;
                    objInitial.Station = station;
                    objInitial.Shift = int.Parse(dt.Rows[0]["SHIFT"].ToString());
                    objInitial.LastStruk = int.Parse(dt.Rows[0]["LAST_STRUK"].ToString());

                    objKasir.Nama = (dt.Rows[0]["NAMA"] + "").Substring(0, 8);
                    objKasir.NIK = dt.Rows[0]["NIK"] + "";

                }
                else
                {
                    _ObjError.Tracelog("Gagal query Initial (rows.count = 0)");
                }
            }
            else
            {
                _ObjError.Tracelog("Gagal query Initial (dt is nothing)");
            }

            _ObjError.TraceLogDebugOnly("init 1 selesai");
            #endregion

            /*#region Toko
            _ObjError.TraceLogDebugOnly("toko mulai");
            string ConstCON = "A";
            try
            {
                SQLQuery = "Select Jenis From Const Where RKey='" + "CON" + "' And Recid='" + " " + "'";
                dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConstCON = _ObjFungsi.Nb(dt.Rows[i]["Jenis"]);
                }
            }
            catch(Exception ex)
            {
                ConstCON = "A";
            }

            string ConstCLX = "XX";
            try
            {
                SQLQuery = "Select `Desc` From Const Where RKey='" + "CL" + ConstCON + "' And Recid='" + " " + "'";
                dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConstCLX = _ObjFungsi.Nb(dt.Rows[i]["Desc"]);
                }
            }
            catch (Exception ex)
            {
                ConstCLX = "XX";
            }

            bool AdaKolomAPKA;
            try
            {
                AdaKolomAPKA = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Toko", "APKA");
            }
            catch (Exception ex)
            {
                AdaKolomAPKA = false;
            }

            SQLQuery = "SELECT KdTk as KodeToko, Nama as NamaToko, NamaFrc as Perusahaan, NPWP";
            SQLQuery += ", Almt as Alamat, Kode_Pos as KodePos, Kota, Telp, Telp_1";
            SQLQuery += ", FlagToko, Tok24, Conv as TipeConv, Kirim as Cabang, Peduli1, Peduli2";
            if(AdaKolomAPKA)
            {
                SQLQuery += ", APKA";
            }
            SQLQuery += " FROM Toko";
            dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

            //revisi danbud
            ClsToko objToko = new ClsToko();

            if(dt.Rows != null)
            {
                if(dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        objToko.KodeToko = dt.Rows[i]["KodeToko"] + "";
                        objToko.NamaToko = dt.Rows[i]["NamaToko"] + "";
                        objToko.Perusahaan = dt.Rows[i]["Perusahaan"] + "";
                        objToko.NPWP = dt.Rows[i]["NPWP"] + "";
                        objToko.Alamat = dt.Rows[i]["Alamat"] + "";
                        objToko.KodePos = dt.Rows[i]["KodePos"] + "";
                        objToko.Kota = dt.Rows[i]["Kota"] + "";
                        objToko.Telepon = dt.Rows[i]["Telp"] + "";
                        if (objToko.Telepon.Trim() == "")
                        {
                            objToko.Telepon = dt.Rows[i]["Telp_1"] + "";
                        }
                        objToko.FlagToko = dt.Rows[i]["FlagToko"] + "";
                        objToko.Cabang = dt.Rows[i]["Cabang"] + "";
                        if((dt.Rows[i]["TOK24"] + "").ToString().ToUpper().Trim() == "Y")
                        {
                            objToko.IsToko24Jam = true;
                        }
                        else
                        {
                            objToko.IsToko24Jam = false;
                        }
                        if (objToko.KodeToko.Trim().StartsWith("T"))
                        {
                            objToko.Kepemilikan = ClsToko.TipeKepemilikan.Regular;
                        }
                        else
                        {
                            objToko.Kepemilikan = ClsToko.TipeKepemilikan.Franchise;
                        }
                        if (!String.IsNullOrEmpty(dt.Rows[i]["TipeConv"].ToString()))
                        {
                            if (dt.Rows[i]["TipeConv"].ToString().Substring(0, 1) == "C")
                            {
                                objToko.TipeConv = ClsToko.TipeConvenience.Convenience;
                            }
                            else
                            {
                                objToko.TipeConv = ClsToko.TipeConvenience.NonConvenience;
                            }
                        }
                        
                        if (dt.Rows[i]["Peduli1"] + "" == "" || dt.Rows[i]["Peduli2"] + "" == "")
                        {
                            objToko.TanggalPeduliAwal = DateTime.Parse("2000-01-01");
                            objToko.TanggalPeduliAkhir = objToko.TanggalPeduliAwal;
                        }
                        else
                        {
                            objToko.TanggalPeduliAwal = (DateTime)dt.Rows[i]["Peduli1"];
                            objToko.TanggalPeduliAkhir = (DateTime)dt.Rows[i]["Peduli2"];
                        }
                        objToko.ConstCON = ConstCON;
                        objToko.ConstCLX = ConstCLX;
                        if (AdaKolomAPKA)
                        {
                            objToko.IsTokoAPKA = (dt.Rows[i]["APKA"].ToString().ToUpper() == "Y");
                        }
                        else
                        {
                            objToko.IsTokoAPKA = false;
                        }
                        objToko.GenerateTtd(objInitial.Tanggal);
                    }
                }
                else
                {
                    _ObjError.Tracelog("Gagal query Toko !");
                    throw new Exception("Gagal query Toko !");
                }
            }
            else
            {
                _ObjError.Tracelog("Gagal query Toko !");
                throw new Exception("Gagal query Toko !");
            }
            _ObjError.TraceLogDebugOnly("toko selesai");
            #endregion*/

            /*#region barang
            _ObjError.TraceLogDebugOnly("barang mulai");
            bool IsAdaTableProductCombo = _ObjSQL.CekTableSQL_New(Mcon, "ProductCombo");
            bool IsProdmastAdaKolomPeriodeJam = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Prodmast", "PeriodeJam");
            bool IsMtranAdaKolomPpn_rate = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Mtran", "ppn_rate");
            bool IsMtranAdaKolomGross_dpp = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Mtran", "gross_dpp");
            bool IsBayarAdaKolomPpn_rate = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Bayar", "ppn_rate");
            bool IsBayarAdaKolomGross_dpp = _ObjSQL.CekColumnFromTableSQL_New(Mcon, "Bayar", "gross_dpp");

            ClsKumpulanBarang objKumpulanBarangMain = new ClsKumpulanBarang();
            objKumpulanBarangMain.DaftarBarang = new ClsBarang[0];

            ClsKumpulanPackaging objKumpulanPackagingMain = new ClsKumpulanPackaging(); // sekalian tambahin daftar packaging
            objKumpulanPackagingMain.DaftarPackaging = new ClsPackaging[0];

            if (_ObjSQL.CekTableSQL_New(Mcon, "PRODMAST"))
            {
                SQLQuery = "SELECT P.PRDCD AS PLU, IFNULL(P.SINGKATAN, '') AS SINGKATAN, IFNULL(P.DESC2,'') AS DESC2, IFNULL(P.UNIT,'') AS UNIT";
                SQLQuery += "\n" + ", IFNULL(IFNULL(R.PRICE,P.PRICE),0) AS HARGAJUALRP";
                SQLQuery += "\n" + ", TRIM(IFNULL(P.CAT_COD,'')) AS CAT_COD, IFNULL(P.DEPART,'') AS DEPART, IFNULL(P.DIVISI,'') AS DIVISI, IFNULL(P.KONS,'') AS KONS, IFNULL(P.ACOST,0) AS ACOST";
                SQLQuery += "\n" + ", TRIM(IFNULL(P.PTAG,'')) AS PTAG, IFNULL(P.BKP,'') AS BKP, IFNULL(P.SUB_BKP,'') AS SUB_BKP, IFNULL(P.PLUMD,P.PRDCD) AS PLUMD";
                SQLQuery += "\n" + ", IFNULL(P.FLAGPROD,'') AS FLAGPROD, TRIM(IFNULL(P.RECID,'')) AS RECID, IFNULL(P.CTGR,'') AS CTGR, TGL_TAMBAH";
                SQLQuery += "\n" + ", P.MERK";
                SQLQuery += "\n" + ", P.PRODUCTTREE, TREE.DESKRIPSI AS PRODUCTTREEDESC, P.IMAGEPATH, P.PRODUCTIMAGE, P.NAMA, IFNULL(P.VNPPN,999) AS VAT";

                if (IsAdaTableProductCombo)
                {
                    SQLQuery += "\n" + ", IFNULL(COMBO.PLU,'') AS COMBO";
                }
                else
                {
                    SQLQuery += "\n" + ", '' AS COMBO";
                }
                if (IsProdmastAdaKolomPeriodeJam)
                {
                    SQLQuery += "\n" + ", P.PERIODEJAM";
                }
                else
                {
                    SQLQuery += "\n" + ", '' AS PERIODEJAM";
                }
                SQLQuery += "\n" +" FROM PRODMAST P";
                SQLQuery += "\n" +" LEFT JOIN (";
                SQLQuery += "\n" +" SELECT PRDCD, MULAI, AKHIR, IF((PRICE - (IF(DISCP>0,DISCP/100*PRICE,0)) - DISCR) > 0,(PRICE - (IF(DISCP>0,DISCP/100*PRICE,0)) - DISCR),0) 'PRICE' FROM PROMOSI WHERE IFNULL(RECID, '') <> '1'"; //link dengan table lain untuk overwrite harga jual, berdasarkan tanggal main
                SQLQuery += "\n" +" ) R ON P.PRDCD = R.PRDCD AND CURDATE() BETWEEN DATE(R.MULAI) AND DATE(R.AKHIR)";
                if(IsAdaTableProductCombo)
                {
                    SQLQuery += "\n" + " LEFT JOIN (SELECT DISTINCT PLU FROM PRODUCTCOMBO) COMBO ON P.PRDCD = COMBO.PLU";
                }

                SQLQuery += "\n" + " LEFT JOIN (SELECT CONCAT(LEVEL, ',', NOURUT, ',', PARENTID) AS PRODUCTTREE, DESKRIPSI FROM PRODUCTTREE) TREE ON P.PRODUCTTREE = TREE.PRODUCTTREE";
                SQLQuery += "\n" + " WHERE 1=1"; //validasi dipindah ke pos net > ClsTransaksi > CekBarang
                SQLQuery += "\n" + " AND TRIM(IFNULL(P.RECID,''))<> 1"; //untuk jualan RecID <> 1
                SQLQuery += "\n" + " AND TRIM(IFNULL(P.CAT_COD,'')) <> '' AND LENGTH(IFNULL(P.CAT_COD,'')) = 5"; //CatCod harus ada, terdiri atas divisi (1 char), departemen (2 char), kategori (2 char)
                SQLQuery += "\n" + " AND IFNULL(P.PTAG,'') NOT IN ('N', 'R')"; //bukan barang tag N / tag R
                SQLQuery += "\n" + " AND IFNULL(P.PRODUCTTREE,'') <>''";
                SQLQuery += "\n" + " ORDER BY P.PRODUCTTREE, NAMA"; //agar tampilan di POS urut berdasarkan label tampilan pos

                dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

                if (dt != null)
                {
                    bool BerlakuPajakRestoran = objToko.TipeConv == ClsToko.TipeConvenience.NonConvenience && objToko.FlagToko.ToUpper().Replace(" ", "").Contains("NPD=");
                    objKumpulanBarangMain.DaftarBarang = new ClsBarang[dt.Rows.Count];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        objKumpulanBarangMain.DaftarBarang[i] = new ClsBarang();
                        objKumpulanBarangMain.DaftarBarang[i].PLU = dt.Rows[i]["PLU"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Singkatan = dt.Rows[i]["SINGKATAN"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Desc2 = dt.Rows[i]["DESC2"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Unit = dt.Rows[i]["UNIT"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].HargaJualRp = int.Parse(dt.Rows[i]["HARGAJUALRP"].ToString());
                        objKumpulanBarangMain.DaftarBarang[i].BisaJual = true;
                        objKumpulanBarangMain.DaftarBarang[i].BisaVoid = true; //dibuat default = true, akan diubah menjadi false untuk transaksi virtual saat transaksi barang
                        //objKumpulanBarangMain.DaftarBarang[i].BisaScanKasir = !(dt.Rows[i]["PSNDULU"].ToString().ToUpper() == "Y");
                        objKumpulanBarangMain.DaftarBarang[i].CatCod = dt.Rows[i]["CAT_COD"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Department = dt.Rows[i]["DEPART"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Divisi = dt.Rows[i]["DIVISI"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Konsinyasi = dt.Rows[i]["KONS"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].HPP = (dt.Rows[i]["PTAG"].ToString().ToUpper() == "N" || dt.Rows[i]["PTAG"].ToString().ToUpper() == "R") ? 0 : double.Parse(dt.Rows[i]["ACOST"].ToString());
                        //objKumpulanBarangMain.DaftarBarang[i].HPPMarkUp = 0 'belum terpakai
                        objKumpulanBarangMain.DaftarBarang[i].PTag = dt.Rows[i]["PTAG"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].BKP = dt.Rows[i]["BKP"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].BKPSub = dt.Rows[i]["SUB_BKP"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].PLU_MD = dt.Rows[i]["PLUMD"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].FlagProd = dt.Rows[i]["FLAGPROD"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].RecId = dt.Rows[i]["RECID"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].Category = dt.Rows[i]["CTGR"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].VAT = int.Parse(dt.Rows[i]["VAT"].ToString());

                        double _VAT = 10;
                        double nVAT = Convert.ToDouble(dt.Rows[i]["VAT"]);

                        try
                        {
                            if (Convert.IsDBNull(dt.Rows[i]["TGL_TAMBAH"]))
                            {
                                objKumpulanBarangMain.DaftarBarang[i].TglTambah = null;
                            }
                            else
                            {
                                objKumpulanBarangMain.DaftarBarang[i].TglTambah = DateTime.ParseExact(dt.Rows[i]["TGL_TAMBAH"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            }
                        }
                        catch
                        {
                            objKumpulanBarangMain.DaftarBarang[i].TglTambah = null;
                        }

                        if (objKumpulanBarangMain.DaftarBarang[i].TglTambah == null) 
                        {
                            objKumpulanBarangMain.DaftarBarang[i].BisaJual = false; //bila tidak valid, maka barang tidak bisa dijual
                        }

                        *//*if (!string.IsNullOrEmpty(dt.Rows[i]["PLU_PAAI"].ToString()) || !string.IsNullOrEmpty(dt.Rows[i]["PLU_AMBIL"].ToString()) || !string.IsNullOrEmpty(dt.Rows[i]["PLU_ANTAR"].ToString()))
                        {
                            objKumpulanBarangMain.DaftarBarang[i].IsItemPAAI = true;
                        }
                        else
                        {
                            objKumpulanBarangMain.DaftarBarang[i].IsItemPAAI = false;
                        }*//*

                        if (!string.IsNullOrEmpty(dt.Rows[i]["COMBO"].ToString()))
                        {
                            objKumpulanBarangMain.DaftarBarang[i].IsProductCombo = true;
                        }
                        else
                        {
                            objKumpulanBarangMain.DaftarBarang[i].IsProductCombo = false;
                        }

                        objKumpulanBarangMain.DaftarBarang[i].Merk = dt.Rows[i]["MERK"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].ProductTree = dt.Rows[i]["PRODUCTTREE"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].ProductTreeDesc = dt.Rows[i]["PRODUCTTREEDESC"].ToString().ToUpper();
                        objKumpulanBarangMain.DaftarBarang[i].ImagePath = dt.Rows[i]["IMAGEPATH"].ToString();
                        if (Convert.IsDBNull(dt.Rows[i]["PRODUCTIMAGE"]))
                        {
                            objKumpulanBarangMain.DaftarBarang[i].ProductImage = null;
                        }
                        else
                        {
                            objKumpulanBarangMain.DaftarBarang[i].ProductImage = (byte[])dt.Rows[i]["PRODUCTIMAGE"];
                        }
                        objKumpulanBarangMain.DaftarBarang[i].Nama = dt.Rows[i]["NAMA"].ToString();
                        objKumpulanBarangMain.DaftarBarang[i].PeriodeJam = dt.Rows[i]["PERIODEJAM"].ToString() + "";

                        objKumpulanBarangMain.DaftarBarang[i].GenerateInfoPajak(_VAT, BerlakuPajakRestoran);


                        //khusus plu packaging, ditambahkan ke ClsKumpulanPackaging
                        if (dt.Rows[i]["PRODUCTTREEDESC"].ToString().ToUpper() == "PACKAGING")
                        {
                            ClsPackaging[] DaftarPackagingNew = new ClsPackaging[] { new ClsPackaging() };
                            DaftarPackagingNew[0].PLU = objKumpulanBarangMain.DaftarBarang[i].PLU;
                            DaftarPackagingNew[0].GenerateTtd(objInitial.Tanggal);
                            objKumpulanPackagingMain.TambahPackaging(DaftarPackagingNew);
                        }
                    }
                }
                else
                {
                    _ObjError.Tracelog("Gagal query Prodmast !");
                    throw new Exception("Gagal query Prodmast !");
                }
            }
            objKumpulanPackagingMain.SetDaftarPackagingAll();
            _ObjError.TraceLogDebugOnly("barang selesai");
            #endregion*/
        }

        public ActionResult MakanDisini()
        {
            string a =  objKasir.NIK;
            return null;
        }

        public ActionResult UbahClass(String INPUTTEXT)
        {
            try
            {
                if(String.IsNullOrEmpty(objKasir.NIK))
                {
                    ViewData["oldNikKasir"] = "Nik Kosong";
                }
                else
                {
                    ViewData["oldNikKasir"] = objKasir.NIK.ToString();
                }

                objKasir.NIK = INPUTTEXT; //perubahan

                ViewData["newNikKasir"] = objKasir.NIK.ToString();

            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_GridView");
        }
    }
}