using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Xml.Linq;

namespace PosSelfService.Common
{
    public class ClsSql
    {
        public DataTable SQLInsertIntoDatatable_New(MySqlConnection mcon, string sQLQuery)
        {
            try
            {
                
                if (mcon.State != ConnectionState.Open)
                {
                    mcon.Open();
                }

                MySqlCommand comm = new MySqlCommand(sQLQuery, mcon);
                MySqlDataReader dr = comm.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(dr);

                return dt;
            }
            catch(Exception ex)
            {
                ClsError r = new ClsError();
                r.ErrorTryCatch(ex);

                return null;
            }
            finally
            {
                if (mcon.State != ConnectionState.Closed)
                {
                    mcon.Close();
                }
            }
        }

        internal bool CekColumnFromTableSQL_New(MySqlConnection mcon, string TbName, string ColName)
        {
            try
            {
                string SQLQuery = "";

                SQLQuery = "Show Columns From `" + TbName + "`"; //cek apakah ada kolom pada table sql yang dimaksud
                SQLQuery += " Where Field = '" + ColName + "'";
                if(SQLExecuteScalar_New(mcon, SQLQuery) + "" != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ClsError r = new ClsError();
                r.ErrorTryCatch(ex);
                return false;
            }
        }

        internal bool CekTableSQL_New(MySqlConnection mcon, string tbName)
        {
            try
            {
                if (SQLExecuteScalar_New(mcon, "Show Tables like '" + tbName + "'") + "" != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ClsError r = new ClsError();
                r.ErrorTryCatch(ex);

                return false;
            }
        }

        private Object SQLExecuteScalar_New(MySqlConnection mcon, string sQLQuery)
        {
            MySqlCommand mcom = new MySqlCommand();
            try
            {
                mcom = new MySqlCommand("",mcon);

                if(mcon.State != ConnectionState.Open)
                {
                    mcon.Open();
                }

                mcom.CommandText = sQLQuery;
                return mcom.ExecuteScalar();
            }
            catch(Exception ex)
            {
                ClsError r = new ClsError();
                r.ErrorTryCatch(ex);

                return 0; //dikembalikan sebagai nol, agar bisa dibaca sebagai string maupun angka
            }
            finally
            {
                mcom.Dispose();
            }

        }
    }
}