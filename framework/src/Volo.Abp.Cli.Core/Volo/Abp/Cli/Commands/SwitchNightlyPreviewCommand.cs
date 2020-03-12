﻿using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.ProjectModification;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Cli.Commands
{
    public class SwitchNightlyPreviewCommand : IConsoleCommand, ITransientDependency
    {
        private readonly PackageSourceSwitcher _packageSourceSwitcher;

        public SwitchNightlyPreviewCommand(PackageSourceSwitcher packageSourceSwitcher)
        {
            _packageSourceSwitcher = packageSourceSwitcher;
        }

        public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
        {
            await _packageSourceSwitcher.SwitchToPreview(commandLineArgs);
        }

        public string GetUsageInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine("Usage:");
            sb.AppendLine("  abp switch-to-preview [options]");
            sb.AppendLine(""); 
            sb.AppendLine("Options:");
            sb.AppendLine("-sp|--solution-path");
            sb.AppendLine("");
            sb.AppendLine("See the documentation for more info: https://docs.abp.io/en/abp/latest/CLI");

            return sb.ToString();
        }

        public string GetShortDescription()
        {
            return "Switches packages to nightly preview ABP version.";
        }
    }
}