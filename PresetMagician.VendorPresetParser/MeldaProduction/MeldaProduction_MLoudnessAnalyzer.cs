using System.Collections.Generic;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;

namespace PresetMagician.VendorPresetParser.MeldaProduction
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class MeldaProduction_MLoudnessAnalyzer : MeldaProduction, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1296851310};

        protected override string PresetFile { get; } =
            "MLoudnessAnalyzerpresets.xml";

        protected override string RootTag { get; } = "MLoudnessAnalyzerpresetspresets";
    }
}