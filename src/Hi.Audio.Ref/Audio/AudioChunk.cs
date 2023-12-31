﻿namespace Hi.Audio
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class AudioChunk
    {
        public short[] Data;
        public int SampleRate;

        /// <summary>
        /// Creates an empty 48khz audio sample
        /// </summary>
        public AudioChunk()
        {
            Data = new short[0];
            SampleRate = 48000;
        }
        public AudioChunk(float[] chunk, int sampleRate)
        {
            this.SampleRate = sampleRate;
            Data = new short[chunk.Length];
            int cursor = 0;
            float scale = (float)(short.MaxValue);
            for (int c = 0; c < chunk.Length; c++)
            {
                Data[cursor + c] = (short)(chunk[c] * scale);
            }
            cursor += chunk.Length;
        }

        /// <summary>
        /// Creates a new audio sample from a 2-byte little endian array
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="sampleRate"></param>
        public AudioChunk(byte[] rawData, int sampleRate)
            : this(AudioMath.BytesToShorts(rawData), sampleRate)
        {
        }

        /// <summary>
        /// Creates a new audio sample from a base64-encoded chunk representing a 2-byte little endian array
        /// </summary>
        /// <param name="base64Data"></param>
        /// <param name="sampleRate"></param>
        public AudioChunk(string base64Data, int sampleRate)
            : this(Convert.FromBase64String(base64Data), sampleRate)
        {
        }

        /// <summary>
        /// Creates a new audio sample from a linear set of 16-bit samples
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="sampleRate"></param>
        public AudioChunk(short[] rawData, int sampleRate)
        {
            Data = rawData;
            SampleRate = sampleRate;
        }

        /// <summary>
        /// Creates a new audio sample from a .WAV file name
        /// </summary>
        /// <param name="fileName"></param>
        //public AudioChunk(string fileName)
        //    : this(new FileStream(fileName, FileMode.Open))
        //{
        //}

        public byte[] GetDataAsBytes()
        {
            return AudioMath.ShortsToBytes(Data);
        }

        public string GetDataAsBase64()
        {
            return Convert.ToBase64String(GetDataAsBytes());
        }

        public AudioChunk Amplify(float amount)
        {
            short[] amplifiedData = new short[DataLength];
            for (int c = 0; c < amplifiedData.Length; c++)
            {
                float newVal = (float)Data[c] * amount;
                if (newVal > short.MaxValue)
                    amplifiedData[c] = short.MaxValue;
                else if (newVal < short.MinValue)
                    amplifiedData[c] = short.MinValue;
                else
                    amplifiedData[c] = (short)newVal;
            }
            return new AudioChunk(amplifiedData, SampleRate);
        }

        public float Peak()
        {
            float highest = 0;
            for (int c = 0; c < Data.Length; c++)
            {
                float test = Math.Abs((float)Data[c]);
                if (test > highest)
                    highest = test;
            }
            return highest;
        }

        public double Volume()
        {
            double curVolume = 0;
            // No Enumerable.Average function for short values, so do it ourselves
            for (int c = 0; c < Data.Length; c++)
            {
                if (Data[c] == short.MinValue)
                    curVolume += short.MaxValue;
                else
                    curVolume += Math.Abs(Data[c]);
            }
            curVolume /= DataLength;
            return curVolume;
        }

        public AudioChunk Normalize()
        {
            double volume = Peak();
            return Amplify(short.MaxValue / (float)volume);
        }

        public int DataLength
        {
            get
            {
                return Data.Length;
            }
        }

        public TimeSpan Length
        {
            get
            {
                return TimeSpan.FromMilliseconds((double)Data.Length * 1000 / SampleRate);
            }
        }

        public AudioChunk Concatenate(AudioChunk other)
        {
            AudioChunk toConcatenate = other;
            int combinedDataLength = DataLength + toConcatenate.DataLength;
            short[] combinedData = new short[combinedDataLength];
            Array.Copy(Data, combinedData, DataLength);
            Array.Copy(toConcatenate.Data, 0, combinedData, DataLength, toConcatenate.DataLength);
            return new AudioChunk(combinedData, SampleRate);
        }

        public AudioChunk GetAudioChunk(int sampleRate)
        {
            short[] resampledData = Lanczos.Resample(Data, SampleRate, sampleRate);
            return new AudioChunk(resampledData, sampleRate);
        }

        public static float[] ShortToFloat(short[] data)
        {
            var output = new float[data.Length];
            float scale = (float)(short.MaxValue);
            int ix = 0;
            while(ix < data.Length)
            {
                output[ix] = data[ix++] / scale;
            }
            return output;
        }
        //public AudioChunk RNN()
        //{
        //    var de = new RNNoiseSharp.Denoiser();
        //    var buff = ShortToFloat(Data);
        //    var length = de.Denoise(buff);
        //    float scale = (float)(short.MaxValue);
        //    int ix = 0;
        //    while (ix < Data.Length)
        //    {
        //        Data[ix] = (short)(Data[ix++] * scale);
        //    }

        //    return this;
        //}
    }
}
