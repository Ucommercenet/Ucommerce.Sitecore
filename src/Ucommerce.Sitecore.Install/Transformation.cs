using System.IO;

namespace Ucommerce.Sitecore.Install
{
    public class Transformation
    {
        public Transformation(FileInfo path, bool isIntegrated = false)
        {
            Path = path;
            OnlyIfIsIntegrated = isIntegrated;
        }

        public bool OnlyIfIsIntegrated { get; set; }
        public FileInfo Path { get; set; }
    }
}
