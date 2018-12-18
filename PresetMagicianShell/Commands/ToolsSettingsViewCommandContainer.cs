﻿using System.Threading.Tasks;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using PresetMagicianShell.Helpers;
using PresetMagicianShell.ViewModels;

// ReSharper disable once CheckNamespace
namespace PresetMagicianShell
{
    public class ToolsSettingsViewCommandContainer : CommandContainerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public ToolsSettingsViewCommandContainer(ICommandManager commandManager)
            : base(Commands.Tools.SettingsView, commandManager)
        {
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            base.Execute(parameter);

            AvalonDockHelper.CreateDocument<SettingsViewModel>();
        }
    }
}