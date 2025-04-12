using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Services.Hardware
{
    public static class BlurhashDecoder
    {
        private static readonly string Base83Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#$%*+,-.:;=?@[]^_{|}~";

        private static float SRGBToLinear(int value)
        {
            float v = (float)value / 255.0f;
            return v <= 0.04045f ? v / 12.92f : (float)Math.Pow((v + 0.055f) / 1.055f, 2.4f);
        }

        private static float SignPow(float value, float exp)
        {
            return (float)Math.Sign(value) * (float)Math.Pow(Math.Abs(value), exp);
        }

        private static int Decode83(char c)
        {
            return Base83Chars.IndexOf(c);
        }

        private static int Decode83(string str)
        {
            int value = 0;
            foreach (char c in str)
                value = value * 83 + Decode83(c);
            return value;
        }

        private static float[] DecodeDC(int value)
        {
            int intR = value >> 16;
            int intG = (value >> 8) & 255;
            int intB = value & 255;
            return new float[] { SRGBToLinear(intR), SRGBToLinear(intG), SRGBToLinear(intB) };
        }

        private static float[] DecodeAC(int value, float maximumValue)
        {
            int quantR = value / (19 * 19);
            int quantG = (value / 19) % 19;
            int quantB = value % 19;

            return new float[]
            {
            SignPow((quantR - 9) / 9.0f, 2.0f) * maximumValue,
            SignPow((quantG - 9) / 9.0f, 2.0f) * maximumValue,
            SignPow((quantB - 9) / 9.0f, 2.0f) * maximumValue
            };
        }

        public static DecodedBlurhash Decode(string blurhash, float punch = 1.0f)
        {
            int sizeFlag = Decode83(blurhash[0]);
            int numY = sizeFlag / 9 + 1;
            int numX = (sizeFlag % 9) + 1;

            int quantisedMaximumValue = Decode83(blurhash[1]);
            float maximumValue = (quantisedMaximumValue + 1) / 166.0f;

            int numColors = numX * numY;
            float[][] colors = new float[numColors][];

            for (int i = 0; i < numColors; i++)
            {
                if (i == 0)
                {
                    int value = Decode83(blurhash.Substring(2, 4));
                    colors[i] = DecodeDC(value);
                }
                else
                {
                    int value = Decode83(blurhash.Substring(4 + i * 2, 2));
                    colors[i] = DecodeAC(value, maximumValue * punch);
                }
            }

            return new DecodedBlurhash
            {
                NumX = numX,
                NumY = numY,
                Colors = colors
            };
        }
    }

    public class DecodedBlurhash
    {
        public int NumX { get; set; }
        public int NumY { get; set; }
        public float[][] Colors { get; set; }
    }

}
