using RubberBandSharp;

namespace RubberBandTest
{
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            int sampleRate = 48000;

            var stretcher = new RubberBandStretcherStereo(sampleRate);

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

            while (inputOffset < audioSize)
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

                        samplesNeeded -= toRead;
                        outputOffset += toRead;

                        //continue;
                    }
                    else
                    {
                        uint stretchSamplesRequired = stretcher.GetSamplesRequired();

                        if ((inputOffset + stretchSamplesRequired) > audioSize)
                        {
                            inputOffset = audioSize;

                            break;
                        }

                        stretcher.Process(leftAudio.Slice((int)inputOffset, (int)stretchSamplesRequired), rightAudio.Slice((int)inputOffset, (int)stretchSamplesRequired), stretchSamplesRequired, isFinal: false);

                        inputOffset += stretchSamplesRequired;
                    }
                }
            }
        }
    }
}
