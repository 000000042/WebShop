using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Generators
{
    public class GuidGenerator
    {
        public static string ActiveCodeGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
