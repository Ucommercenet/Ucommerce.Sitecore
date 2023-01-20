using CliFx.Attributes;

namespace Ucommerce.Sitecore.Cli.Commands.db
{
    /// <summary>
    /// Base class for db commands exposing shared Options
    /// </summary>
    public abstract class Db
    {
        [CommandOption("ConnectionString",
            'c',
            Description = "Connection string for the database.",
            IsRequired = true,
            EnvironmentVariable = "UCOMMERCE_CONNECTION_STRING")]
        // ReSharper disable once MemberCanBeProtected.Global
        public string ConnectionString { get; set; }
    }
}
