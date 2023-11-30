using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsMasr
    {
        public string PLU_JUAL { get; set; }
        public string PLU_BHN_BAKU { get; set; }
        public string PLU_ADDITIONAL { get; set; }
        public string PLU_PENGGANTI { get; set; }
        public string KET_SPECIAL_REQUEST { get; set; }
        public int VALUE { get; set; }
        public string SATUAN { get; set; }
        public int MAX_PLU_JUAL { get; set; }
        public int MAX_ADD { get; set; }
        public string KEL_SPCL_REQ { get; set; }
        public string DESKRIPSI_SR { get; set; }
        public string ADDID { get; set; }
        public DateTime ADDTIME { get; set; }
    }

    public class ClsMmsr
    {
        public string PLU_JUAL { get; set; }
        public string PLU_BHN_BAKU { get; set; }
        public string KET_SPEC_REQ { get; set; }
        public string SINGKATAN { get; set; }
        public int VALUE_SR { get; set; }
        public string SATUAN { get; set; }
        public string KEL_SPCL_REQ { get; set; }
        public string ADDID { get; set; }
        public DateTime ADDTIME { get; set; }
    }
}