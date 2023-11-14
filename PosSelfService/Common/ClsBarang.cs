using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsBarang
    {
        public bool BisaJual { get; set; }
        public bool BisaScanKasir { get; set; }
        public bool BisaVoid { get; set; }
        public string BKP { get; set; }
        public string BKPSub { get; set; }
        public string CatCod { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
        public string Desc2 { get; set; }
        public string Divisi { get; set; }
        public string FlagProd { get; set; }
        public double HargaJualRp { get; set; }
        public double HPP { get; set; }
        public string ImagePath { get; set; }
        public bool IsItemPAAI { get; set; }
        public bool IsProductCombo { get; set; }
        public string Konsinyasi { get; set; }
        public string Merk { get; set; }
        public string Modifier { get; set; }
        public string Nama { get; set; }
        public JenisPajak PajakJenis { get; set; }
        public double PajakRp { get; set; }
        public string PeriodeJam { get; set; }
        public string PLU { get; set; }
        public string PLU_MD { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductTree { get; set; }
        public string ProductTreeDesc { get; set; }
        public string PTag { get; set; }
        public string RecId { get; set; }
        public string Singkatan { get; set; }
        public StatusBarang Status { get; set; }
        public DateTime? TglTambah { get; set; }
        public string Ttd { get; set; }
        public DateTime TtdTime { get; set; }
        public string Unit { get; set; }
        public double VAT { get; set; }

        public void GenerateInfoPajak(double nVAT, bool berlakuPajakRestoran)
        {
            if (BKP == "Y")
            {
                if (berlakuPajakRestoran && FlagProd.ToUpper().Contains("PJR=Y"))
                {
                    PajakJenis = JenisPajak.Restoran;
                }
                else
                {
                    PajakJenis = JenisPajak.PPN;
                }

                if (BKPSub == "G" || BKPSub == "W" || BKPSub == "P")
                {
                    PajakRp = nVAT / 100 * HargaJualRp;
                }
                else
                {
                    PajakRp = 1 / ((nVAT + 100) / nVAT) * HargaJualRp;
                }
            }
            else
            {
                if (BKPSub == "C")
                {
                    PajakJenis = JenisPajak.Cukai;
                }
                else
                {
                    PajakJenis = JenisPajak.TanpaPPN;
                }
            }

        }

    }

    public enum JenisPajak
    {
        Cukai,
        Restoran,
        TanpaPPN,
        PPN
    }

    public enum StatusBarang
    {
        OK,
        Qty0,
        ProdNull,
        BarcodeNull,
        TtdInvalid,
        IKiosk,
        NotOK,
    }

}