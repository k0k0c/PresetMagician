using System.IO;
using System.Linq;
using System.Net.Sockets;
using Catel.IO;
using Drachenkatze.PresetMagician.VendorPresetParser.Properties;
using GSF;

namespace Drachenkatze.PresetMagician.VendorPresetParser.AIRMusicTechnology.Tfx
{
    public class TfxXpand2 : Tfx
    {
        public override byte[] BlockMagic { get; } = {0x0C, 0x0B, 0xA2, 0x28};

        public override void PostProcess()
        {
            // Fix for some tfx files not containing all required parameters
            if (Parameters.Count == 114)
            {
                // fill missing parameters for all parts
                Parameters.Add(0);             // Part A Voice Mode
                Parameters.Add(0);             // Part A Mono Mode
                Parameters.Add(0.49);          // Part A Polyphony
                Parameters.Add(0.16666);       // Part A PB Range
                Parameters.Add(0.5);           // Part B Attack
                Parameters.Add(0.5);           // Part B Decay
                Parameters.Add(0.5);           // Part B Release
                Parameters.Add(0.5);           // Part B Cutoff
                Parameters.Add(0.5);           // Part B Env Depth
                Parameters.Add(0.5);           // Part B Fine Tune
                Parameters.Add(0);             // Part B Voice Mode
                Parameters.Add(0);             // Part B Mono Mode
                Parameters.Add(0.49);          // Part B Polyphony
                Parameters.Add(0.16666);       // Part B PB Range
                Parameters.Add(0.5);           // Part C Attack
                Parameters.Add(0.5);           // Part C Decay
                Parameters.Add(0.5);           // Part C Release
                Parameters.Add(0.5);           // Part C Cutoff
                Parameters.Add(0.5);           // Part C Env Depth
                Parameters.Add(0.5);           // Part C Fine Tune
                Parameters.Add(0);             // Part C Voice Mode
                Parameters.Add(0);             // Part C Mono Mode
                Parameters.Add(0.49);          // Part C Polyphony
                Parameters.Add(0.16666);       // Part C PB Range
                Parameters.Add(0.5);           // Part D Attack
                Parameters.Add(0.5);           // Part D Decay
                Parameters.Add(0.5);           // Part D Release
                Parameters.Add(0.5);           // Part D Cutoff
                Parameters.Add(0.5);           // Part D Env Depth
                Parameters.Add(0.5);           // Part D Fine Tune
                Parameters.Add(0);             // Part D Voice Mode
                Parameters.Add(0);             // Part D Mono Mode
                Parameters.Add(0.49);          // Part D Polyphony
                Parameters.Add(0.16666);       // Part D PB Range
                
            }

            using (var ms = new MemoryStream())
            {
                ms.Write(LittleEndian.GetBytes(PatchName.Length+1), 0, 4);
                ms.Write(PatchName, 0, PatchName.Length);
                ms.WriteByte(0);

                EndChunk = Resource1.Xpand2EndChunk
                    .Concat(ms.ToByteArray()).ToArray();
            }

            ParseMidi();
        }

        public override byte[] GetBlockDataToWrite()
        {
            if (!WzooBlock.IsMagicBlock)
            {
                WzooBlock.PluginName = new byte[]
                    {0x00, 0x58, 0x00, 0x70, 0x00, 0x61, 0x00, 0x6e, 0x00, 0x64, 0x00, 0x21, 0x00, 0x32};
                WzooBlock.IsMagicBlock = true;

                var oldData = WzooBlock.BlockData;

                using (var ms = new MemoryStream())
                {
                    // add 4 null bytes
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    
                    //add block length
                    ms.Write(BigEndian.GetBytes(oldData.Length), 0, 4);
                    ms.Write(oldData, 0, oldData.Length);
                    
                    //add deadbeef
                    ms.WriteByte(0xDE);
                    ms.WriteByte(0xAD);
                    ms.WriteByte(0xBE);
                    ms.WriteByte(0xEF);

                    WzooBlock.BlockData = ms.ToByteArray();
                }
                 
                
            }
            else
            {
                WzooBlock.PluginName = WzooBlock.PluginName.Concat(new byte[] {0x00, 0x32}).ToArray();
            }

            if (!MidiBlock.IsMagicBlock)
            {
                MidiBlock.PluginName = new byte[]
                    {0x00, 0x58, 0x00, 0x70, 0x00, 0x61, 0x00, 0x6e, 0x00, 0x64, 0x00, 0x21, 0x00, 0x32};
                MidiBlock.IsMagicBlock = true;
                
                using (var ms = new MemoryStream())
                {
                    // add 4 null bytes
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    ms.WriteByte(0x00);
                    
                    // add 4 FF bytes
                    ms.WriteByte(0xFF);
                    ms.WriteByte(0xFF);
                    ms.WriteByte(0xFF);
                    ms.WriteByte(0xFF);
                    
                    //add block length
                    ms.Write(BigEndian.GetBytes(Resource1.Xpand2DefaultMidi.Length), 0, 4);
                    ms.Write(Resource1.Xpand2DefaultMidi, 0, Resource1.Xpand2DefaultMidi.Length);
                    
                    //add deadbeef
                    ms.WriteByte(0xDE);
                    ms.WriteByte(0xAD);
                    ms.WriteByte(0xBE);
                    ms.WriteByte(0xEF);

                    MidiBlock.BlockData = ms.ToByteArray();
                }
            }else
            {
                MidiBlock.PluginName = MidiBlock.PluginName.Concat(new byte[] {0x00, 0x32}).ToArray();
            }


            var data = WzooBlock.GetDataToWrite().Concat(MidiBlock.GetDataToWrite()).ToArray();


            return data;
        }
    }
}