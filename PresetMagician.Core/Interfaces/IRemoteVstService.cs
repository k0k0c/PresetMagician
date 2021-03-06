using System;
using System.Collections.Generic;
using System.ServiceModel;
using PresetMagician.Core.Models;
using PresetMagician.Core.Models.Audio;
using PresetMagician.Core.Models.MIDI;
using PresetMagician.RemoteVstHost.Faults;

namespace PresetMagician.Core.Interfaces
{
    [ServiceContract(Namespace = "https://presetmagician.com")]
    public interface IRemoteVstService
    {
        [OperationContract]
        Guid RegisterPlugin(string dllPath, bool backgroundProcessing = true);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(AccessViolationFault))]
        [FaultContract(typeof(NoEntryPointFoundFault))]
        void LoadPlugin(Guid pluginGuid, bool debug = false);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        bool OpenEditorHidden(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        void CloseEditor(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        [FaultContract(typeof(PluginEditorNotOpenFault))]
        byte[] CreateScreenshot(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        void ReloadPlugin(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        void UnloadPlugin(Guid pluginGuid);

        [OperationContract]
        bool Ping();

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        string GetPluginName(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        List<PluginInfoItem> GetPluginInfoItems(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        string GetPluginVendor(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        VstPluginInfoSurrogate GetPluginInfo(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        [FaultContract(typeof(AccessViolationFault))]
        void SetProgram(Guid pluginGuid, int program);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        byte[] GetChunk(Guid pluginGuid, bool isPreset);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PresetDataNullFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        [FaultContract(typeof(AccessViolationFault))]
        void SetChunk(Guid pluginGuid, byte[] data, bool isPreset);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        string GetCurrentProgramName(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        void ExportNksAudioPreview(Guid pluginGuid, PresetExportInfo preset, byte[] presetData,
            int initialDelay);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void ExportNks(Guid pluginGuid, PresetExportInfo preset, byte[] presetData);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        bool OpenEditor(Guid pluginGuid, bool topmost = true);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        int GetPluginVendorVersion(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        string GetPluginProductString(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        string GetEffectivePluginName(Guid pluginGuid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        [FaultContract(typeof(PluginNotRegisteredFault))]
        [FaultContract(typeof(PluginNotLoadedFault))]
        float GetParameter(Guid pluginGuid, int parameterIndex);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        bool Exists(string file);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        long GetSize(string file);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        string GetHash(string file);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        byte[] GetContents(string file);

        [OperationContract]
        void KillSelf();

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        DateTime GetLastModifiedDate(string file);

        [OperationContract]
        void UnregisterPlugin(Guid guid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void PatchPluginToAudioOutput(Guid guid, AudioOutputDevice device, int latency);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void UnpatchPluginFromAudioOutput();

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void PatchPluginToMidiInput(Guid guid, MidiInputDevice device);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void UnpatchPluginFromMidiInput();

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void PerformIdleLoop(Guid guid, int loops);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void DisableTimeInfo(Guid guid);

        [OperationContract]
        [FaultContract(typeof(GenericFault))]
        void EnableTimeInfo(Guid guid);
    }
}