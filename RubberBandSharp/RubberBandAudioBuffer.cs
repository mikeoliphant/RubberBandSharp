using System;
using System.Runtime.InteropServices;

namespace RubberBandSharp
{
    public class RubberBandAudioBuffer
    {
        public uint NumChannels { get; private set; }
        public uint NumSamples { get; private set; } 

        IntPtr backingAudioBufferPtrs = IntPtr.Zero;

        public unsafe RubberBandAudioBuffer(uint numChannels, uint numSamples)
        {
            this.NumChannels = numChannels;
            this.NumSamples = numSamples;

            backingAudioBufferPtrs = Marshal.AllocHGlobal((int)numChannels * Marshal.SizeOf(typeof(IntPtr)));

            float ** bufferPtrs = (float**)backingAudioBufferPtrs;

            for (int i = 0; i < numChannels; i++)
            {
                bufferPtrs[i] = (float*)Marshal.AllocHGlobal((int)numSamples * sizeof(float));
            }
        }

        public virtual unsafe Span<float> GetAudioBuffer(int channel)
        {
            return new Span<float>((void*)((float**)backingAudioBufferPtrs)[channel], (int)NumSamples);
        }

        public IntPtr GetAudioBufferPtrs()
        {
            return backingAudioBufferPtrs;
        }
    }
}
