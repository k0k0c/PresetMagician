using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel.Collections;
using Drachenkatze.PresetMagician.VendorPresetParser;
using Drachenkatze.PresetMagician.VSTHost.VST;
using PresetMagician.Models;
using SharedModels;

namespace PresetMagician.Services.Interfaces
{
    public interface IVstService
    {
        event EventHandler SelectedPluginChanged;
        Plugin SelectedPlugin { get; set; }
        FastObservableCollection<Plugin> SelectedPlugins { get; }
        ObservableCollection<Plugin> Plugins { get; }
        VstHost.VST.VstHost VstHost { get; set; }
        FastObservableCollection<Preset> PresetExportList { get; }
        Preset SelectedExportPreset { get; set; }
        FastObservableCollection<Preset> SelectedPresets { get; }
        FastObservableCollection<Plugin> CachedPlugins { get; }
        event EventHandler SelectedExportPresetChanged;
        Task SavePlugins();
        byte[] GetPresetData(Preset preset);
        Task<IRemoteVstService> LoadVst(Plugin plugin, bool backgroundProcessing = true, bool sharedPool = true);
        Task<IRemoteVstService> LoadVstInteractive(Plugin plugin);
        Task UnloadVst(Plugin plugin);
    }
}