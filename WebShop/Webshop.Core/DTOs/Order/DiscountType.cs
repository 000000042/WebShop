using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.DTOs.Order
{
    public enum DiscountType
    {
        Success,
        ExpireDate,
        NotFound,
        Used,
        Finished
    }
}
