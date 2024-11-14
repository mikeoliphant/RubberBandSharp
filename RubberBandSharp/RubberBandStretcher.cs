using System;

namespace RubberBandSharp
{
    public class RubberBandStretcher
    {
        public enum Options
        {
            None = 0,

            ProcessOffline = 0x00000000,
            ProcessRealTime = 0x00000001,

            StretchElastic = 0x00000000,
            StretchPrecise = 0x00000010,

            TransientsCrisp = 0x00000000,
            TransientsMixed = 0x00000100,
            TransientsSmooth = 0x00000200,

            DetectorCompound = 0x00000000,
            DetectorPercussive = 0x00000400,
            DetectorSoft = 0x00000800,

            PhaseLaminar = 0x00000000,
            PhaseIndependent = 0x00002000,

            ThreadingAuto = 0x00000000,
            ThreadingNever = 0x00010000,
            ThreadingAlways = 0x00020000,

            WindowStandard = 0x00000000,
            WindowShort = 0x00100000,
            WindowLong = 0x00200000,

            SmoothingOff = 0x00000000,
            SmoothingOn = 0x00800000,

            FormantShifted = 0x00000000,
            FormantPreserved = 0x01000000,

            PitchHighSpeed = 0x00000000,
            PitchHighQuality = 0x02000000,
            PitchHighConsistency = 0x04000000,

            ChannelsApart = 0x00000000,
            ChannelsTogether = 0x10000000,
        }

        IntPtr nativePtr = IntPtr.Zero;
        uint numChannels = 0;

        public RubberBandStretcher(int sampleRate, uint channels, Options options = Options.None, double initialTimeRatio = 1.0, double initialPitchScale = 1.0)
        {
            this.numChannels = channels;

            nativePtr = NativeAPI.rubberband_new(new IntPtr(sampleRate), numChannels, (int)options, initialTimeRatio, initialPitchScale);
        }

        public void SetTimeRatio(double timeRatio)
        {
            NativeAPI.rubberband_set_time_ratio(nativePtr, timeRatio);
        }

        public void SetPitchScale(double pitchScale)
        {
            NativeAPI.rubberband_set_pitch_scale(nativePtr, pitchScale);
        }

        public int Available()
        {
            return NativeAPI.rubberband_available(nativePtr);
        }

        public uint GetSamplesRequired()
        {
            return NativeAPI.rubberband_get_samples_required(nativePtr);
        }

        public uint Retrieve(IntPtr audioPtr, uint numSamples)
        {
            return NativeAPI.rubberband_retrieve(nativePtr, audioPtr, numSamples);
        }

        public void Process(IntPtr audioPtr, uint numSamples, bool isFinal)
        {
            NativeAPI.rubberband_process(nativePtr, audioPtr, numSamples, isFinal ? 1 : 0);
        }
    }

    public class RubberBandStretcherStereo : RubberBandStretcher
    {
        public RubberBandStretcherStereo(int sampleRate, Options options = Options.None, double initialTimeRatio = 1.0, double initialPitchScale = 1.0)
            : base(sampleRate, 2, options, initialTimeRatio, initialPitchScale)
        {
        }

        public unsafe void Process(ReadOnlySpan<float> leftAudio, ReadOnlySpan<float> rightAudio, uint numSamples, bool isFinal)
        {
            Span<IntPtr> ptrs = stackalloc IntPtr[2];

            fixed (IntPtr* ptr = ptrs)
            {
                fixed (float* leftPtr = leftAudio)
                {
                    fixed (float* rightPtr = rightAudio)
                    {
                        ptrs[0] = (IntPtr)leftPtr;
                        ptrs[1] = (IntPtr)rightPtr;

                        Process((IntPtr)ptr, numSamples, isFinal);
                    }
                }
            }
        }

        public unsafe uint Retrieve(Span<float> leftAudio, Span<float> rightAudio, uint numSamples)
        {
            Span<IntPtr> ptrs = stackalloc IntPtr[2];

            uint samplesRead = 0;

            fixed (IntPtr* ptr = ptrs)
            {
                fixed (float* leftPtr = leftAudio)
                {
                    fixed (float* rightPtr = rightAudio)
                    {
                        ptrs[0] = (IntPtr)leftPtr;
                        ptrs[1] = (IntPtr)rightPtr;

                        samplesRead = Retrieve((IntPtr)ptr, numSamples);
                    }
                }
            }

            return samplesRead;
        }
    }

    public class RubberBandStretcherMono : RubberBandStretcher
    {
        public RubberBandStretcherMono(int sampleRate, Options options = Options.None, double initialTimeRatio = 1.0, double initialPitchScale = 1.0)
            : base(sampleRate, 1, options, initialTimeRatio, initialPitchScale)
        {
        }

        public unsafe void Process(ReadOnlySpan<float> audio, uint numSamples, bool isFinal)
        {
            Span<IntPtr> ptrs = stackalloc IntPtr[1];

            fixed (IntPtr* ptr = ptrs)
            {
                fixed (float* audioPtr = audio)
                {
                    ptrs[0] = (IntPtr)audioPtr;

                    Process((IntPtr)ptr, numSamples, isFinal);
                }
            }
        }

        public unsafe uint Retrieve(Span<float> audio, uint numSamples)
        {
            Span<IntPtr> ptrs = stackalloc IntPtr[1];

            uint samplesRead = 0;

            fixed (IntPtr* ptr = ptrs)
            {
                fixed (float* audioPtr = audio)
                {
                    ptrs[0] = (IntPtr)audioPtr;

                    samplesRead = Retrieve((IntPtr)ptr, numSamples);
                }
            }

            return samplesRead;
        }
    }
}
