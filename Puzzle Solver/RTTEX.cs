using System.IO.Compression;

namespace Puzzle_Solver;

public static class RTTEX
{
    public static class RTTEXConverter
    {
        public static async Task<byte[]> RTTEXUnpack(byte[] buffer)
        {
            byte[] data = buffer;
            if (System.Text.Encoding.ASCII.GetString(data, 0, 6) == "RTPACK")
            {
                data = Decompress(data, 32, data.Length - 32);
            }
            
            if (System.Text.Encoding.ASCII.GetString(data, 0, 6) == "RTTXTR")
            {
                int width = BitConverter.ToInt32(data, 12);
                int height = BitConverter.ToInt32(data, 8);
                int channels = 3 + data[0x1c];
                byte[] imageBuffer = data[0x7c..];
                using var image = Image.LoadPixelData<Rgba32>(imageBuffer, width, height);
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                using var memoryStream = new MemoryStream();
                await image.SaveAsPngAsync(memoryStream);
                return memoryStream.ToArray();
            }
            else
            {
                Console.WriteLine("This is not a RTTEX file");
                return null;
            }
        }

        private static byte[] Decompress(byte[] data, int offset, int count)
        {
            using var inputStream = new MemoryStream(data, offset, count);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}