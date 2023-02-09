using CliFx.Attributes;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("upgrade", Description = "Upgrade the current Ucommerce installation.")]
    public class Upgrade : Install { }
}
