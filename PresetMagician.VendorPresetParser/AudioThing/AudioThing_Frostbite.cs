﻿using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using PresetMagician.Core.Interfaces;
using PresetMagician.Core.Models;

namespace PresetMagician.VendorPresetParser.AudioThing
{
    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public class AudioThing_Frostbite : AudioThing, IVendorPresetParser
    {
        public override List<int> SupportedPlugins => new List<int> {1181905730};

        protected override PresetBank GetRootBank()
        {
            return RootBank.CreateRecursive(BankNameFactory);
        }

        protected override string GetParseDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"AudioThing\Presets\Frostbite");
        }
    }
}