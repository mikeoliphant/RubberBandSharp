using RubberBandSharp;

namespace RubberBandTest
{
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            int sampleRate = 48000;

            var stretcher = new RubberBandStretcherStereo(sampleRate,
                    RubberBandStretcher.Options.ProcessRealTime |
                    RubberBandStretcher.Options.WindowShort |
                    RubberBandStretcher.Options.FormantPreserved |
                    RubberBandStretcher.Options.PitchHighConsistency);


            stretcher.SetTimeRatio(2);

            uint bufferSize = 128;

            uint audioSize = (uint)(sampleRate * 10);

            Span<float> leftAudio = new Span<float>(new float[audioSize]);
            Span<float> rightAudio = new Span<float>(new float[audioSize]);

            for (int i = 0; i < audioSize; i++)
            {
                leftAudio[i] = rightAudio[i] =  (float)Math.Sin((double)i / (double)sampleRate);
            }

            Span<float> leftOutput = new Span<float>(new float[bufferSize]);
            Span<float> rightOutput = new Span<float>(new float[bufferSize]);

            uint inputOffset = 0;

            uint outputSamplesProcessed = 0;

            bool finished = false;

            while (!finished)
            {
                uint samplesNeeded = bufferSize;
                uint outputOffset = 0;

                while (samplesNeeded > 0)
                {
                    int avail = stretcher.Available();

                    if (avail > 0)
                    {
                        uint toRead = (uint)Math.Min(avail, samplesNeeded);

                        uint read = stretcher.Retrieve(leftOutput.Slice((int)outputOffset, (int)toRead), rightOutput.Slice((int)outputOffset, (int)toRead), toRead);

                        if (read != toRead)
                            throw new Exception();

                        samplesNeeded -= read;
                        outputOffset += read;

                        outputSamplesProcessed += read;
                    }
                    else
                    {
                        if (inputOffset >= audioSize)
                        {
                            finished = true;

                            break;
                        }

                        uint stretchSamplesRequired = stretcher.GetSamplesRequired();

                        bool isFinal = false;

                        if ((inputOffset + stretchSamplesRequired) >= audioSize)
                        {
                            stretchSamplesRequired = audioSize - inputOffset;

                            isFinal = true;
                        }

                        stretcher.Process(leftAudio.Slice((int)inputOffset, (int)stretchSamplesRequired), rightAudio.Slice((int)inputOffset, (int)stretchSamplesRequired), stretchSamplesRequired, isFinal);

                        inputOffset += stretchSamplesRequired;
                    }
                }
            }

            float outputRatio = (float)outputSamplesProcessed / (float)audioSize;

            Console.WriteLine("Output size ratio was: " + outputRatio);
        }
    }
}
