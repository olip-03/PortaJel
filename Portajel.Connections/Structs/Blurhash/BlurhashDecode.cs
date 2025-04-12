using System;

namespace Portajel.Connections.Structs.Blurhash;
public class BlurhashDecode
{
    private static readonly string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#$%*+,-.:;=?@[]^_{|}~";

    private static double SRGBToLinear(int value)
    {
        double v = value / 255.0;
        if (v <= 0.04045)
        {
            return v / 12.92;
        }
        else
        {
            return Math.Pow((v + 0.055) / 1.055, 2.4);
        }
    }

    private static double SignPow(double val, double exp)
    {
        return Math.Sign(val) * Math.Pow(Math.Abs(val), exp);
    }

    private static int Decode83(string str)
    {
        int value = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            int digit = Characters.IndexOf(c);
            value = value * 83 + digit;
        }
        return value;
    }

    private static double[] DecodeDC(int value)
    {
        int intR = value >> 16;
        int intG = (value >> 8) & 255;
        int intB = value & 255;
        return new double[] { SRGBToLinear(intR), SRGBToLinear(intG), SRGBToLinear(intB) };
    }

    private static double[] DecodeAC(int value, double maximumValue)
    {
        int quantR = (int)Math.Floor(value / (19.0 * 19.0));
        int quantG = (int)Math.Floor(value / 19.0) % 19;
        int quantB = value % 19;

        double[] rgb = new double[]
        {
            SignPow((quantR - 9) / 9.0, 2.0) * maximumValue,
            SignPow((quantG - 9) / 9.0, 2.0) * maximumValue,
            SignPow((quantB - 9) / 9.0, 2.0) * maximumValue
        };

        return rgb;
    }

    public static (int numX, int numY, double[][] colors) Decode(string blurhash, double punch = 1.0)
    {
        int sizeFlag = Decode83(blurhash.Substring(0, 1));

        int numY = (int)Math.Floor(sizeFlag / 9.0) + 1;
        int numX = (sizeFlag % 9) + 1;

        int quantisedMaximumValue = Decode83(blurhash.Substring(1, 1));
        double maximumValue = (quantisedMaximumValue + 1) / 166.0;

        int numColors = numX * numY;
        double[][] colors = new double[numX * numY][];

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

        return (numX, numY, colors);
    }
}
