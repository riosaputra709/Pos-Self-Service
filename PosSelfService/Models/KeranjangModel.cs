using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Models
{
    public class KeranjangModel
    {
        public string prdcd { get; set; }
        public string name { get;set; }
        public int price { get;set; }
        public string image { get; set; }
        public int qty { get; set;}

        List<AdditionalRequestModel> additionalRequests { get; set; }
    }

    public class AdditionalRequestModel
    {
        public string kelSpecReq { get; set; }
        public List<string> objAdditional { get; set; }
    }
}