using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    [Serializable]
    public class ClsToko
    {
        private string _Alamat;
        private string _Cabang;
        private string _ConstCLX;
        private string _ConstCON;
        private string _FlagToko;
        private bool _IsTokoAPKA;
        private bool _IsToko24Jam;
        private TipeKepemilikan _Kepemilikan;
        private string _KodePos;
        private string _KodeToko;
        private string _Kota;
        private string _NamaToko;
        private string _NPWP;
        private string _Perusahaan;
        private DateTime _TanggalPeduliAkhir;
        private DateTime _TanggalPeduliAwal;
        private string _Telepon;
        private TipeConvenience _TipeConv;
        private string _Ttd; // signature untuk data/file integrity
        private DateTime _TtdTime;

        public enum TipeConvenience
        {
            Convenience,
            NonConvenience
        }

        public enum TipeKepemilikan
        {
            Regular,
            Franchise
        }

        public string Alamat
        {
            get { return _Alamat; }
            set { _Alamat = value; }
        }

        public string Cabang
        {
            get { return _Cabang; }
            set { _Cabang = value; }
        }

        public string ConstCLX
        {
            get { return _ConstCLX; }
            set { _ConstCLX = value; }
        }

        public string ConstCON
        {
            get { return _ConstCON; }
            set { _ConstCON = value; }
        }

        public string FlagToko
        {
            get { return _FlagToko; }
            set { _FlagToko = value; }
        }

        public bool IsTokoAPKA
        {
            get { return _IsTokoAPKA; }
            set { _IsTokoAPKA = value; }
        }

        public bool IsToko24Jam
        {
            get { return _IsToko24Jam; }
            set { _IsToko24Jam = value; }
        }

        public TipeKepemilikan Kepemilikan
        {
            get { return _Kepemilikan; }
            set { _Kepemilikan = value; }
        }

        public string KodePos
        {
            get { return _KodePos; }
            set { _KodePos = value; }
        }

        public string KodeToko
        {
            get { return _KodeToko; }
            set { _KodeToko = value; }
        }

        public string Kota
        {
            get { return _Kota; }
            set { _Kota = value; }
        }

        public string NamaToko
        {
            get { return _NamaToko; }
            set { _NamaToko = value; }
        }

        public string NPWP
        {
            get { return _NPWP; }
            set { _NPWP = value; }
        }

        public string Perusahaan
        {
            get { return _Perusahaan; }
            set { _Perusahaan = value; }
        }

        public DateTime TanggalPeduliAkhir
        {
            get { return _TanggalPeduliAkhir; }
            set { _TanggalPeduliAkhir = value; }
        }

        public DateTime TanggalPeduliAwal
        {
            get { return _TanggalPeduliAwal; }
            set { _TanggalPeduliAwal = value; }
        }

        public string Telepon
        {
            get { return _Telepon; }
            set { _Telepon = value; }
        }

        public TipeConvenience TipeConv
        {
            get { return _TipeConv; }
            set { _TipeConv = value; }
        }

        public string Ttd
        {
            get { return _Ttd; }
            set
            {
                _Ttd = value;
                _TtdTime = DateTime.Now;
            }
        }

        public DateTime TtdTime
        {
            get { return _TtdTime; }
            set { _TtdTime = value; }
        }

        internal void GenerateTtd(DateTime TglInitial)
        {
            _Ttd = RumusTtd(TglInitial);
        }

        private string RumusTtd(DateTime TglInitial)
        {
            ClsFungsi Enc = new ClsFungsi();
            return Enc.Encrypt(
                Enc.Nb(_Alamat).ToString() +
                Enc.Nb(_Cabang).ToString() +
                Enc.Nb(_FlagToko).ToString() +
                Enc.Nb(_IsToko24Jam).ToString() +
                Enc.Nb(_Kepemilikan.ToString()).ToString() +
                Enc.Nb(_KodePos).ToString() +
                Enc.Nb(_KodeToko).ToString() +
                Enc.Nb(_Kota).ToString() +
                Enc.Nb(_NamaToko).ToString() +
                Enc.Nb(_NPWP).ToString() +
                Enc.Nb(_Perusahaan).ToString() +
                Enc.Nb(_TanggalPeduliAkhir).ToString() +
                Enc.Nb(_TanggalPeduliAwal).ToString() +
                Enc.Nb(_Telepon).ToString() +
                Enc.Nb(_TipeConv.ToString()).ToString() +
                "", Enc.EncPassPhrase(TglInitial), Enc.EncSaltValue);
        }

    }

}