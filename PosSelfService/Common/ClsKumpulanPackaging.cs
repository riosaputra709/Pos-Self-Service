using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    [Serializable]
    public class ClsKumpulanPackaging
    {
        public ClsPackaging[] DaftarPackaging;
        public string DaftarPackagingAll;

        public void TambahPackaging(ClsPackaging[] DaftarPackagingNew)
        {
            // Menambahkan Packaging ke dalam daftar Packaging
            Array.Resize(ref DaftarPackaging, DaftarPackaging.Length + DaftarPackagingNew.Length);
            DaftarPackagingNew.CopyTo(DaftarPackaging, DaftarPackaging.Length - DaftarPackagingNew.Length);
        }

        internal void SetDaftarPackagingAll()
        {
            DaftarPackagingAll = "";
            for (int counter = 0; counter < DaftarPackaging.Length; counter++)
            {
                if (!string.IsNullOrEmpty(DaftarPackagingAll))
                {
                    DaftarPackagingAll += ",";
                }
                DaftarPackagingAll += DaftarPackaging[counter].PLU;
            }

        }
    }

}