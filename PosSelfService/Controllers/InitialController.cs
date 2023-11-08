using MySql.Data.MySqlClient;
using PosSelfService.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.EnterpriseServices.Internal;
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
        public MySqlConnection MasterMcon = new MySqlConnection();

        // GET: Initial
        public ActionResult Index()
        {
            LoadMaster(MasterMcon);
            return View();
        }

        private void LoadMaster(MySqlConnection Mcon)
        {
            string station = "01";
            DataTable dt = new DataTable();
            string folderPath = Server.MapPath("~/IDMPosStandAlone/");
            _ObjError.Tracelog("Load Master Data mulai");
            string SQLQuery = "";
            MySqlCommand mcom = new MySqlCommand("", Mcon);
            MySqlDataAdapter mdap = new MySqlDataAdapter();

            #region init
            _ObjError.TraceLogDebugOnly("init 1 mulai");
            SQLQuery = "SELECT Shift, IfNull(Last_Struk, 0) as Last_Struk, Kasir_Name as Nama, NIK";
            SQLQuery += " FROM Initial";
            SQLQuery += " WHERE Tanggal = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            SQLQuery += " AND Station= '" + station + "'";
            SQLQuery += " AND Trim(IfNull(RecId, '')) = ''";
            dt = _ObjSQL.SQLInsertIntoDatatable_New(Mcon, SQLQuery);

            mdap.Fill(dt);

            ClsKasir objKasir = new ClsKasir();
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

            #region Toko
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

            #endregion
        }
    }
}