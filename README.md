# RubberBandSharp

RubberBandSharp is a .NET Core wrapper around the [Rubber Band audio time-stretching and pitch-shifting library](https://github.com/breakfastquay/rubberband).

Currently I only wrap the functionality I need. If you need other functionality, please open an issue.

# Usage

RubberBandSharp is available on NuGet here:

https://www.nuget.org/packages/RubberBandSharp

The **RubberBandStretcherStereo** and **RubberBandStretcherMono** class are provided for simple stereo and mono use cases.

The more general **RubberBandStretcher** base class supports any number of channels, but you are currently responsible for mapping input audio buffers into an IntPtr which is a "float**" to your audio buffers.

Have a look at the provided test program for a usage example:

https://github.com/mikeoliphant/RubberBandSharp/blob/master/RubberBandTest/Program.cs

