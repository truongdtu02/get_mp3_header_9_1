using System;
using System.IO;

namespace get_mp3_header_9_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //string _filePath = @"E:\truyenthanhproject\read_mp3\read_mp3\LoveIsBlue.mp3";
            string _filePath = "LoveIsBlue.mp3"; //in server

            byte[] buff = File.ReadAllBytes(_filePath);
            int buff_Length = buff.Length;

            int i_buff = 0;

            while (i_buff < buff_Length - 3)
            {

                i_buff++;
            }

            Console.WriteLine(i_buff);

            Console.ReadLine();
        }
    }
}
