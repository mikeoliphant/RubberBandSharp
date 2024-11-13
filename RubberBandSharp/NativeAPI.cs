using System;
using System.Runtime.InteropServices;

namespace RubberBandSharp
{
    internal static class NativeAPI
    {
        public const string RUBBERBAND_LIB_NAME = "rubberband-library";

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern IntPtr rubberband_new(IntPtr sampleRate, uint channels, int options = 0, double initialTimeRatio = 1.0, double initialPitchScale = 1.0);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern void rubberband_set_time_ratio(IntPtr state, double ratio);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern void rubberband_set_pitch_scale(IntPtr state, double scale);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern int rubberband_available(IntPtr state);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern uint rubberband_get_samples_required(IntPtr state);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern uint rubberband_retrieve(IntPtr state, IntPtr output, uint samples);

        [DllImport(RUBBERBAND_LIB_NAME)]
        public static extern void rubberband_process(IntPtr state, IntPtr input, uint samples, int final);
    }
}
