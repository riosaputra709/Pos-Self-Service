using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Common
{
    public class ClsFungsi
    {
        public string Nb(object AnyValue)
        {
            return (AnyValue == null || AnyValue is DBNull) ? "" : AnyValue.ToString().Trim();
        }

    }
}