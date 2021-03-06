using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MDrumLeveler : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296331340};

        protected override string PresetFile { get; } = "MDrumLevelerpresets.xml";

        protected override string RootTag { get; } = "MDrumLevelerpresets";
    }
}