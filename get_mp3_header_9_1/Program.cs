using System;
using System.Diagnostics;
using System.IO;

namespace get_mp3_header_9_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string _filePath = "Binz22.mp3";
            //string _filePath = @"E:\truyenthanhproject\read_mp3\Binz22.mp3";
            //string _filePath = @"E:\truyenthanhproject\read_mp3\LoveIsBlue.mp3";
            //string _filePath = "LoveIsBlue.mp3"; //in server

            byte[] send_buff = new byte[1500];

            byte[] buff = File.ReadAllBytes(_filePath);
            int buff_Length = buff.Length;

            int i_buff = 0, header_count = 0, padding = 0; ;

            var MP3_header_first = new MP3_header();

            var MP3_header_next = new MP3_header();

            //create timer
            Stopwatch stopWatch;
            stopWatch = new Stopwatch();
            stopWatch.Start();

            //get first header
            while (i_buff < buff_Length - 4)
            {
                if((buff[i_buff] == 0xFF) &&((buff[i_buff + 1] & 0xE0) == 0xE0)) //sync bit
                {
                    if (MP3_header_first.IsValidHeader(false, null, buff, i_buff, buff_Length))
                    {
                        header_count++;
                        padding += MP3_header_first.Padding;
                        //buff.CopyTo()
                        Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_first.Frame_size);
                        i_buff += MP3_header_first.Frame_size;
                        break;
                    }
                        
                }
                    
                i_buff++;
            }

            //get next header
            while (i_buff < buff_Length - 4)
            {
                if ((buff[i_buff] == 0xFF) && ((buff[i_buff + 1] & 0xE0) == 0xE0)) //sync bit
                {
                    if(MP3_header_next.IsValidHeader(true, MP3_header_first, buff, i_buff, buff_Length))
                    {
                        header_count++;
                        padding += MP3_header_next.Padding;
                        Buffer.BlockCopy(buff, i_buff, send_buff, 0, MP3_header_next.Frame_size);
                        i_buff += MP3_header_next.Frame_size - 1; //because i_buff++ after
                        //i_buff += MP3_header_first.Frame_size; continue;
                    }
                }
                    
                i_buff++;
            }

            Console.WriteLine("Total frame: {0}", header_count);

            Console.WriteLine("Padding frame: {0}", padding);

            double endTime = stopWatch.Elapsed.TotalMilliseconds;

            Console.WriteLine("Total time: {0} ms", endTime);
            Console.ReadLine();
        }
    }
}
