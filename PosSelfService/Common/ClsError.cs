using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsError
    {
        public string Tracelog(string msg)
        {
            string hasil = "";
            if (msg.EndsWith(@"\"))
            {
                msg = msg.Substring(0, (msg.Length - 1));
            }

            if (msg.Length > 4000)
            {
                msg = msg.Substring(0, 4000);
            }

            while (msg.EndsWith("'") && !msg.EndsWith("''"))
            {
                msg = msg.Substring(0, msg.Length - 1);
            }

            try
            {
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Tracelog")))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Tracelog"));
                }
                string sFile = HttpContext.Current.Server.MapPath("~/Tracelog/TraceLog" + DateTime.Now.ToString("yyMMddHH") + ".txt");
                StreamWriter sw = new StreamWriter(sFile, true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + (": " + msg));
                sw.Flush();
                sw.Close();
            }
            catch(Exception ex1)
            {
                hasil = ex1.Message;
            }
            return hasil;
        }

        internal void ErrorTryCatch(Exception ex)
        {
            Tracelog(ex.Message + " " + ex.StackTrace);
        }

        internal void TraceLogDebugOnly(string teks)
        {
            //aktifkan untuk debug saja
            Tracelog("Debug : " + teks);
        }
    }
}