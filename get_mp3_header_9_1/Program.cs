using System;
using System.Diagnostics;
using System.IO;

namespace get_mp3_header_9_1
{
    class Program
    {
        static bool left_frame_not_packet = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string _filePath = "Binz2.mp3";
            //string _filePath = @"E:\truyenthanhproject\read_mp3\Binz22.mp3";
            //string _filePath = @"E:\truyenthanhproject\read_mp3\LoveIsBlue.mp3";
            //string _filePath = "LoveIsBlue.mp3"; //in server
            //string _filePath = @"E:\truyenthanhproject\read_mp3\Binz2.mp3";
            //                                                    CRC   ethnernet   IPv4   UDP header
            const int Max_send_buff_length = 1472; //MTU = 1518 - 4 -     14 -      20 -      8  

            byte[] send_buff = new byte[Max_send_buff_length];
            
            byte[] buff = File.ReadAllBytes(_filePath);
            int buff_Length = buff.Length;

            int header_count = 0, padding = 0;

            double ave_bitrate = 0;

            

            //create timer
            Stopwatch stopWatch;
            stopWatch = new Stopwatch();
            stopWatch.Start();

            var mp3_reader = new MP3_header(buff, buff_Length);
            mp3_reader.IsValidMp3();

            int numOfFrame = 1;
            while(numOfFrame > 0)
                numOfFrame = packet_udp_frameMP3(send_buff, Max_send_buff_length, mp3_reader);

            //get first header
            //while (i_buff < buff_Length - 4)
            //{
            //    if ((buff[i_buff] == 0xFF) && ((buff[i_buff + 1] & 0xE0) == 0xE0)) //sync bit
            //    {
            //        //check it is first time
            //        if (MP3_header_first.Version == 0) //null
            //        {
            //            if (MP3_header_first.IsValidHeader(false, null, buff, i_buff, buff_Length))
            //            {
            //                header_count++;
            //                padding += MP3_header_first.Padding;
            //                //buff.CopyTo()
            //                //Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_first.Frame_size);
            //                beggin_index_frame = i_buff;
            //                offset_frame = MP3_header_first.Frame_size;

            //                i_buff += MP3_header_first.Frame_size - 1;
            //            }
            //        }
            //        else //next frame
            //        {
            //            if (MP3_header_next.IsValidHeader(true, MP3_header_first, buff, i_buff, buff_Length))
            //            {
            //                header_count++;
            //                padding += MP3_header_next.Padding;
            //                //Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_next.Frame_size);
            //                beggin_index_frame = i_buff;
            //                offset_frame = MP3_header_next.Frame_size;
            //                i_buff += MP3_header_next.Frame_size - 1; //because i_buff++ after
            //                                                          //i_buff += MP3_header_first.Frame_size; continue;
            //            }
            //        }

            //    }               
            //    i_buff++;

            //    //if offset_frame > 0, mean that found a frame
            //    //packet frame by beggin_index_frame, offset_frame
            //    //reset offset_frame = 0
            //    //send 
            //}

            Console.WriteLine("Total frame: {0}", header_count);

            Console.WriteLine("Padding frame: {0}", padding);

            if(header_count > 0)
            {
                Console.WriteLine("Average bitrate: {0}", ave_bitrate / header_count);
            }           

            double endTime = stopWatch.Elapsed.TotalMilliseconds;

            Console.WriteLine("Total time: {0} ms", endTime);
            Console.ReadLine();
        }

        static public int packet_udp_frameMP3(byte[] _send_buff, int _max_send_buff_length, MP3_header _mp3_reader)
        {
            //packet: 4-byte
            int numOfFrame = 0, totalLength = 0;
            while (true)
            {
                if(left_frame_not_packet)
                {
                    left_frame_not_packet = false;
                }
                else if(_mp3_reader.ReadNextFrame())
                {

                }
                else
                {
                    break;
                }
                //check space for cmemcpy //(4+4) for numOfFrame, totalLength
                if (_mp3_reader.Frame_size <= (_max_send_buff_length - (4+4) - totalLength))
                {
                    Buffer.BlockCopy(_mp3_reader.Mp3_buff, _mp3_reader.Start_frame, _send_buff,(4+4+totalLength), _mp3_reader.Frame_size);
                    totalLength += _mp3_reader.Frame_size;
                    numOfFrame++;
                }
                else
                {
                    left_frame_not_packet = true;
                    break;
                }
            }
            //copy num of frame (little edian)
            byte[] tmp_byte = BitConverter.GetBytes(numOfFrame);
            Buffer.BlockCopy(tmp_byte, 0, _send_buff, 0, 4);

            //copy total length frame (byte) (little edian)
            tmp_byte = BitConverter.GetBytes(totalLength);
            Buffer.BlockCopy(tmp_byte, 0, _send_buff, 4, 4);

            return numOfFrame;
        }
    }
}






//using System;
//using System.Diagnostics;
//using System.IO;

//namespace get_mp3_header_9_1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World!");

//            //string _filePath = "Binz22.mp3";
//            string _filePath = @"E:\truyenthanhproject\read_mp3\Binz22.mp3";
//            //string _filePath = @"E:\truyenthanhproject\read_mp3\LoveIsBlue.mp3";
//            //string _filePath = "LoveIsBlue.mp3"; //in server

//            byte[] send_buff = new byte[1500];

//            byte[] buff = File.ReadAllBytes(_filePath);
//            int buff_Length = buff.Length;

//            int i_buff = 0, header_count = 0, padding = 0; ;

//            var MP3_header_first = new MP3_header();

//            var MP3_header_next = new MP3_header();

//            //create timer
//            Stopwatch stopWatch;
//            stopWatch = new Stopwatch();
//            stopWatch.Start();

//            //get first header
//            while (i_buff < buff_Length - 4)
//            {
//                if((buff[i_buff] == 0xFF) &&((buff[i_buff + 1] & 0xE0) == 0xE0)) //sync bit
//                {
//                    if (MP3_header_first.IsValidHeader(false, null, buff, i_buff, buff_Length))
//                    {
//                        header_count++;
//                        padding += MP3_header_first.Padding;
//                        //buff.CopyTo()
//                        Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_first.Frame_size);
//                        i_buff += MP3_header_first.Frame_size;
//                        break;
//                    }

//                }

//                i_buff++;
//            }

//            //get next header
//            while (i_buff < buff_Length - 4)
//            {
//                if ((buff[i_buff] == 0xFF) && ((buff[i_buff + 1] & 0xE0) == 0xE0)) //sync bit
//                {
//                    if(MP3_header_next.IsValidHeader(true, MP3_header_first, buff, i_buff, buff_Length))
//                    {
//                        header_count++;
//                        padding += MP3_header_next.Padding;
//                        Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_next.Frame_size);
//                        i_buff += MP3_header_next.Frame_size - 1; //because i_buff++ after
//                        //i_buff += MP3_header_first.Frame_size; continue;
//                    }
//                }

//                i_buff++;
//            }

//            Console.WriteLine("Total frame: {0}", header_count);

//            Console.WriteLine("Padding frame: {0}", padding);

//            double endTime = stopWatch.Elapsed.TotalMilliseconds;

//            Console.WriteLine("Total time: {0} ms", endTime);
//            Console.ReadLine();
//        }
//    }
//}
