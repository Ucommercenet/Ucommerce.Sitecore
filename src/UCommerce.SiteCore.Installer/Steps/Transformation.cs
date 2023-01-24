using System.IO;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class Transformation
    {
        public Transformation(FileInfo path, bool isIntegrated = false)
        {
            Path = path;
            OnlyIfIisIntegrated = isIntegrated;
        }

        public bool OnlyIfIisIntegrated { get; set; }
        public FileInfo Path { get; set; }
    }
}
