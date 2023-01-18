using System.Threading.Tasks;

namespace Ucommerce.Sitecore.Installer.Steps
{
    /// <summary>Defines a basic method of a step</summary>
    public interface IStep
    {
        /// <summary>Runs this step</summary>
        Task Run();
    }
}