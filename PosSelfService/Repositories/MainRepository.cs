using MySql.Data.MySqlClient;
using PosSelfService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace PosSelfService.Repositories
{
    public class MainRepository : BaseRepository
    {
        public static MainRepository instance = null;
        public static MainRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainRepository();
                }
                return instance;
            }
        }


        internal IList<ProdukModel> SearchProduk(ProdukModel produkRequest)
        {
            MySqlConnection conn = openConnection();
            try
            {
                List<ProdukModel> produkList = new List<ProdukModel>();
                ProdukModel produk;
                conn.Open();
                string query = "SELECT PRDCD,MERK, Nama,Desc2,Singkatan,TRUNCATE(Price, 0),Productimage FROM prodmast " +
                    "WHERE TRIM(IFNULL(RECID,''))<> 1 AND TRIM(IFNULL(CAT_COD,'')) <> '' AND LENGTH(IFNULL(CAT_COD,'')) = 5 " +
                    "AND IFNULL(PTAG,'') NOT IN('N', 'R') AND IFNULL(PRODUCTTREE,'') <> '' ";
                if (!String.IsNullOrEmpty(produkRequest.NAMA))
                {
                    query += "AND Nama like '%" + produkRequest.NAMA + "%' ";
                }
                if (!String.IsNullOrEmpty(produkRequest.MERK))
                {
                    query += "AND MERK = '" + produkRequest.MERK + "' ";
                }
                MySqlCommand comm = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        produk = new ProdukModel();
                        produk.PRDCD = reader.IsDBNull(0) ? null : reader.GetString(0);
                        produk.MERK = reader.IsDBNull(1) ? null : reader.GetString(1);
                        produk.NAMA = reader.IsDBNull(2) ? null : reader.GetString(2);
                        produk.DESC2 = reader.IsDBNull(3) ? null : reader.GetString(3);
                        produk.SINGKATAN = reader.IsDBNull(4) ? null : reader.GetString(4);
                        produk.PRICE = reader.IsDBNull(5) ? null : reader.GetString(5);

                        byte[] blobData = reader.GetValue(6) == DBNull.Value || string.IsNullOrEmpty(reader.GetValue(6).ToString())
                        ? new byte[0]  // Ketika nilainya null atau string kosong, mengatur blobData menjadi array byte kosong
                        : (byte[])reader.GetValue(6);
                        string base64String = Convert.ToBase64String(blobData);
                        string mimeType = "image/jpeg"; // Ganti dengan tipe konten yang sesuai jika perlu
                        produk.PRODUCTIMAGE = string.Format("data:{0};base64,{1}", mimeType, base64String);

                        produkList.Add(produk);
                    }
                }
                conn.Close();
                return produkList;
            }
            catch (Exception ex)
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                }
                throw new Exception(ex.Message);
            }
        }

        internal ProdukModel GetByKeyWithDtl(int id)
        {
            MySqlConnection conn = openConnection();
            try
            {
                ProdukModel produk = new ProdukModel();
                conn.Open();
                string query = "SELECT PRDCD,MERK, Nama,Desc2,Singkatan,TRUNCATE(Price, 0),Productimage FROM prodmast " +
                    "WHERE TRIM(IFNULL(RECID,''))<> 1 AND TRIM(IFNULL(CAT_COD,'')) <> '' AND LENGTH(IFNULL(CAT_COD,'')) = 5 " +
                    "AND IFNULL(PTAG,'') NOT IN('N', 'R') AND IFNULL(PRODUCTTREE,'') <> '' AND PRDCD = '" + id + "'";
                MySqlCommand comm = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = comm.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        produk.PRDCD = reader.IsDBNull(0) ? null : reader.GetString(0);
                        produk.MERK = reader.IsDBNull(1) ? null : reader.GetString(1);
                        produk.NAMA = reader.IsDBNull(2) ? null : reader.GetString(2);
                        produk.DESC2 = reader.IsDBNull(3) ? null : reader.GetString(3);
                        produk.SINGKATAN = reader.IsDBNull(4) ? null : reader.GetString(4);
                        produk.PRICE = reader.IsDBNull(5) ? null : reader.GetString(5);

                        byte[] blobData = reader.GetValue(6) == DBNull.Value || string.IsNullOrEmpty(reader.GetValue(6).ToString())
                        ? new byte[0] : (byte[])reader.GetValue(6);
                        string base64String = Convert.ToBase64String(blobData);
                        string mimeType = "image/jpeg"; // Ganti dengan tipe konten yang sesuai jika perlu
                        produk.PRODUCTIMAGE = string.Format("data:{0};base64,{1}", mimeType, base64String);
                    }
                }
                return produk;
            }
            catch (Exception ex)
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                }
                throw new Exception(ex.Message);
            }
        }
    }
}