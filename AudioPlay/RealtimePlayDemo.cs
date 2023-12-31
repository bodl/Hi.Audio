﻿using Hi.Audio.Ref;
using Hi.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Audio.Ref.Codec;

namespace AudioPlay
{
    internal class RealtimePlayDemo
    {
        public static async Task Run()
        {
            var format = new AudioFormat(48000, 2, 16);
            format.BufferMilliseconds = 10;
            format.DesiredLatency = 100;
            var reader = new AudioRecorder(format, 0);
            var player = new AudioPlayer(reader.AudioFormat);
            var codec = new Mp3Codec(reader.AudioFormat);
            //var codec = new OpusCodec(reader.AudioFormat, OpusApplication.OPUS_APPLICATION_RESTRICTED_LOWDELAY);
            //codec.Bitrate = 510;
            //codec.Complexity = 10;
            //codec.FrameSize = 10;
            //codec.VBR = true;
            Console.Write("Starting");
            reader.Start();

            //player.Volume = 1f;
            while (true)
            {
                while (reader.CanReadChunk)
                {
                    var chunk = reader.GetNextChunk();
                    var data = codec.Encode(chunk);
                    if (data.Length > 0)
                    {
                        var o = codec.Decode(data);
                        player.Add(o);
                        player.Play();
                    }
                }
                //player.Add(new AudioChunk(new byte[192], 48000));
            }
            //Console.Read();
        }
    }
}
