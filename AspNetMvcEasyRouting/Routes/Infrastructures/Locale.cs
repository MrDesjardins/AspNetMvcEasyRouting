using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class Locale
    {
        public Locale(CultureInfo culture, string domainUrl = null)
        {
            this.CultureInfo = culture;
            this.DomainUrl = domainUrl;
        }

        public CultureInfo CultureInfo { get; private set; }
        public string DomainUrl { get; private set; }
    }
}
