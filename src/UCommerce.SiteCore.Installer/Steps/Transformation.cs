using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class Transformation
    {
        public string path { get; set; }
        public bool onlyIfIsIntegrated  { get; set; }

        public Transformation (string path ,bool onlyIfIsIntegrated) { 
            this.path = path;
            this.onlyIfIsIntegrated = onlyIfIsIntegrated;
        }

    }
}
