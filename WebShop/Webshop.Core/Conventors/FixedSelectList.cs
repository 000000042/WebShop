using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webshop.Core.Conventors
{
    public static class FixedSelectList
    {
        public static void MoveToFirst(this List<SelectListItem> list,string value)
        {
            var filter = list.FirstOrDefault(f => f.Value == value);

            list.Remove(filter);
            list.Insert(0, filter);
        }
    }
}
