using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Models
{
    public class KeranjangModel
    {
        public int id { get; set; }
        public string prdcd { get; set; }
        public string name { get;set; }
        public int price { get;set; }
        public string image { get; set; }
        public int qty { get; set;}
        public string merk { get; set; }
        public string mmsr { get; set; }
        public string masr { get; set; }
        public List<AdditionalRequestModel> additionalRequests { get; set; }
    }

    public class AdditionalRequestModel
    {
        public string kelSpecReq { get; set; }
        public string typeAdditional { get; set; }
        public int maxAdditional { get; set; }
        public string objAdditional { get; set; }
    }
}