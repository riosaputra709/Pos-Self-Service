using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsInitial
    {
        public DateTime AddTime { get; set; }
        public string AddTimeChar { get; set; }
        public ClsKasir Kasir { get; set; }
        public ClsKumpulanBarang KumpulanBarang { get;set; }
        public int LastStruk { get; set; }
        public DateTime LoadBarangTime { get; set; }
        public string LoadBarangTimeChar { get; set; }
        public DateTime LoadPromoTime { get; set; }
        public string LoadPromoTimeChar { get; set; }
        public bool PakaiPromoMatriks { get; set; }
        public int Shift { get; set; }
        public string Station { get; set; }
        public DateTime Tanggal { get; set; }
        public string TanggalChar { get; set; }
        public string Ttd { get; set; }
        public DateTime TtdTime { get; set; }

    }
}