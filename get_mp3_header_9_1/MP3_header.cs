using System;
using System.Collections.Generic;
using System.Text;

namespace get_mp3_header_9_1
{
    class MP3_header
    {
        //mp3 is MPEG 1 Layer III or MPEG 2 layer III, detail: https://en.wikipedia.org/wiki/MP3

        //mp3 header include 32-bit
        // byte0    byte1   byte2   byte3
        //bit 31                        0
        //detail: http://www.mp3-tech.org/programmer/frame_header.

        //bit 31-21 is frame sync, all bit is 1, (byte0 == FF) && (byte1 & 0xE0 == 0xE0)

        //bit 20-19 : MPEG version, 11: V1, 10: V2
        int version;

        public int Version { get => version; }

        //bit 18-17 Layer, just consider layer III, 01
        const int layer = 3;
        public static int Layer => layer;



        //bit 16, protected bit, don't count

        //bit 15-12 bitrate
        static readonly int[] bitrate_V1_L3 = { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0 }; // MPEG 1, layer III
        static readonly int[] bitrate_V2_L3 = { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 }; // MPEG 2, layer III
        int bitrate;
        public int Bitrate { get => bitrate; }


        //bit 11-10 sample rate
        static readonly int[] sample_rate_V1 = { 44100, 48000, 32000, 0 };
        static readonly int[] sample_rate_V2 = { 22050, 24000, 16000, 0 };
        int sample_rate;
        public int Sample_rate { get => sample_rate; }

        //sample per frame
        int sample_per_frame;
        static readonly int[] sample_per_frame_version = { 1152, 576 };

        public int Sample_per_frame { get => sample_per_frame; }

        //bit 9, padding bit
        int padding;
        public int Padding { get => padding; }
        
        int frame_size;
        public int Frame_size { get => frame_size; }

        public bool IsValidHeader(bool first_header, MP3_header MP3_header_first, byte[] buff, int i_buff, int buff_length)
        {
            //get infor header
            int header = (int)buff[i_buff + 3] | ((int)buff[i_buff + 2] << 8) | ((int)buff[i_buff + 1] << 16) | ((int)buff[i_buff] << 24);

            //get version
            int tmp = (header >> 19) & 0b11;
            if (tmp == 0b11)
                version = 1;
            else if (tmp == 0b10)
                version = 2;
            else
                return false;

            //get layer
            tmp = (header >> 17) & 0b11;
            if (tmp != 0b01) //layer III
                return false;

            //get bitrate
            tmp = (header >> 12) & 0b1111;
            if ((tmp == 0) || (tmp == 0b1111))
                return false;
            if (version == 1)
                bitrate = bitrate_V1_L3[tmp];
            else if (version == 2)
                bitrate = bitrate_V2_L3[tmp];

            //get smaple rate
            tmp = (header >> 10) & 0b11;
            if (tmp == 0b11)
                return false;
            if (version == 1)
                sample_rate = sample_rate_V1[tmp];
            else if (version == 2)
                sample_rate = sample_rate_V2[tmp];

            if (first_header)
            {
                if ((MP3_header_first.version != version) || (MP3_header_first.sample_rate != sample_rate))
                    return false;
            }

            //get padding
            padding = (header >> 9) & 1;

            //get sample per frame
            sample_per_frame = sample_per_frame_version[version - 1];

            //get frame size
            double frame_size_tmp = bitrate * 1000 * sample_per_frame / 8 / sample_rate + padding;
            frame_size = (int)frame_size_tmp;

            //check next frame
            if ((i_buff + frame_size - 1) < buff_length)
            {
                //i_buff += (frame_size - 1);
            }
            else
                return false;

            return true;
        }
    }
}
