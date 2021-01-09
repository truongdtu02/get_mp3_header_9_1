using System;
using System.Collections.Generic;
using System.Text;

namespace get_mp3_header_9_1
{
    class MP3_header
    {
        //mp3 is MPEG 1 Layer III or MPEG 2 layer III, detail: https://en.wikipedia.org/wiki/MP3

        //mp3 header include 32-bit, consider 32-bit as little edian
        // byte0    byte1   byte2   byte3
        //bit 0                         31
        //detail: http://www.mp3-tech.org/programmer/frame_header.

        //bit 0-10 is frame sync, all bit is 1, (byte0 == FF) && (byte1 & 0xE0 == 0xE0)

        //bit 11-12 : MPEG version, 11: V1, 10: V2
        int version;

        public int Version { get => version; }

        //bit 13-14 Layer, just consider layer III, 01
        const int layer = 3;
        public static int Layer => layer;

        

        //bit 15, protected bit, don't count

        //bit 16-19 bitrate
        static readonly int?[] bitrate_V1_L3 = { null, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, null }; // MPEG 1, layer III
        static readonly int?[] bitrate_V2_L3 = { null, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, null }; // MPEG 2, layer III
        int bitrate;
        public int Bitrate { get => bitrate; }


        //bit 20-21 sample rate
        static readonly int?[] sample_rate_V1 = { 44100, 48000, 32000, null };
        static readonly int?[] sample_rate_V2 = { 22050, 24000, 16000, null };
        int sample_rate;
        public int Sample_rate { get => sample_rate; }

        //bit 22, padding bit
        int padding;
        public int Padding { get => padding; }

        

    }
}
