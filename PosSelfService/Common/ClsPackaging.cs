using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsPackaging
    {
        public string PLU { get; set; } //informasi plu Packaging
        private string Ttd { get; set; } //signature untuk data/file integrity
        public DateTime TtdTime { get; set; }

        internal void GenerateTtd(DateTime TglInitial)
        {
            Ttd = RumusTtd(TglInitial);
        }

        private string RumusTtd(DateTime TglInitial)
        {
            ClsFungsi Enc = new ClsFungsi();
            return Enc.Encrypt(
                Enc.Nb(PLU).ToString() + "", Enc.EncPassPhrase(TglInitial), Enc.EncSaltValue);
        }
    }
}