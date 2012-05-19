using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iDeal.Base
{
    public static class StringExtensions
    {
        public static bool IsNullEmptyOrWhiteSpace(this string value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.Trim());
        }
    }
}
