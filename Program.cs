using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Lab3
{
    class Program
    {

        static void Main(string[] args)
        {
            string fileNamepng = "testbild.png";  //testa med denna för PNG bilden
            string fileNamebmp = "bildtest.bmp";  //testa med denna för BMP
            string fileNameNeither = "testitestbild.jpg"; //testa med denna för JPG (varken PNG eller BMP)


            var png = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }; //png signatur (dec) 137, 80, 78, 71, 13, 10, 26, 10 (hex) 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            var bmp = new byte[] { 66, 77 }; // bmp signatur (dec) 66, 77 (hex) 0x42, 0x4D

            // välj vilken bild, läs alla bytes från bilden till byte array och spara som bytes
            var bytes = File.ReadAllBytes(fileNamepng);

            var reader = new BinaryReader(new MemoryStream(bytes));

            // läser 2 första bytsen och sparar det som filesig, är filesig samma som bmp så är isbmp true
            var fileSig = reader.ReadBytes(2);
            var isBmp = Enumerable.SequenceEqual(bmp, fileSig);
            // om isBmp är sann läss igenom alla bytes i filen och skriv ut width o height
            if (isBmp)
            {
                var fileSize = reader.ReadInt32();
                var reserved = reader.ReadInt32();
                var offset = reader.ReadInt32();
                var bitmapInfoHeader = reader.ReadInt32();
                var bmpWidth = reader.ReadInt32();
                var bmpHeight = reader.ReadInt32();
                var planes = reader.ReadInt16();
                var bpp = reader.ReadInt16();
                var compressionType = reader.ReadInt32();
                var imageSize = reader.ReadInt32();
                var horizontalRes = reader.ReadInt32();
                var verticalRes = reader.ReadInt32();
                var color = reader.ReadInt32();
                var importantColors = reader.ReadInt32();
                var red = reader.ReadByte();
                var green = reader.ReadByte();
                var blue = reader.ReadByte();

                //skriv ut till konsolen
                Console.WriteLine($"\nThis is a BMP file! and the dimensions are:{bmpWidth}x{bmpHeight}");
            }
            else
            {
                // om isbmp inte är sann  kopiera vår filesig till tempbuffer läs in 6 resterande bytes och kolla om dessa stämmer in med png signatur
                var tempBuffer = new byte[8];
                Buffer.BlockCopy(fileSig, 0, tempBuffer, 0, 2);
                fileSig = reader.ReadBytes(6);
                Buffer.BlockCopy(fileSig, 0, tempBuffer, 2, 6);

                var isPng = Enumerable.SequenceEqual(png, tempBuffer);
                if (isPng)
                {
                    // om ispng är sann läs igenom alla bytes och skriv ut width och height
                    var length = reader.ReadBytes(4);
                    var type = reader.ReadBytes(4);
                    var info = reader.ReadBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(length)));
                    var CRC = reader.ReadBytes(4);

                    reader = new BinaryReader(new MemoryStream(info));

                    var pngWidth = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(reader.ReadBytes(4)));
                    var pngHeight = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(reader.ReadBytes(4)));
                    var pngBitDepth = reader.ReadByte();
                    var pngColorType = reader.ReadByte();
                    var pngCompressionMethod = reader.ReadByte();
                    var pngFilterMethod = reader.ReadByte();
                    var pngInterlaceMethod = reader.ReadByte();

                    //skriv ut till konsolen
                    Console.WriteLine($"\nThis is a PNG file! and the dimensions are:{pngWidth}x{pngHeight}");
                }
                else
                {
                    // stämmer varken av signaturerna in med BMP elr PNG så skriv ut: 
                    Console.WriteLine("\nFile is neither of BMP or PNG format!");
                }

            }

        }
    }
}