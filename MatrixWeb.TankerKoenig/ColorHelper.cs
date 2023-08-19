using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using System;

namespace MatrixWeb.TankerKoenig;
internal static class ColorHelper {
    public static Color MapGasPrice(double price, double minPrice, double maxPrice) {
        double diffrence = maxPrice - minPrice;

        double min;
        double max;

        if (diffrence < 0.01) {
            min = price - 0.01;
            max = price + 0.01;
        } else {
            min = minPrice + (diffrence * 0.1);
            max = maxPrice - (diffrence * 0.1);
        }

        return MapColor(price, min, max, 120, 0);
    }

    private static Color MapColor(double value, double min, double max, double hueFrom, double hueTo) {
        if (value < min) {
            return HueToColor(hueFrom);
        }

        if (value > max) {
            return HueToColor(hueTo);
        }

        double mapedValue = (value - min) / (max - min);
        double hue = (mapedValue * (hueTo - hueFrom)) + hueFrom;
        return HueToColor(hue);
    }

    private static Color HueToColor(double hue) => HsvToColor(hue % 360, 1, 1);

    private static Color HsvToColor(double hue, double saturation, double value) {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = (hue / 60) - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)Convert.ToInt32(value);
        byte p = (byte)Convert.ToInt32(value * (1 - saturation));
        byte q = (byte)Convert.ToInt32(value * (1 - (f * saturation)));
        byte t = (byte)Convert.ToInt32(value * (1 - ((1 - f) * saturation)));

        return hi switch {
            0 => Color.FromRgb(v, t, p),
            1 => Color.FromRgb(q, v, p),
            2 => Color.FromRgb(p, v, t),
            3 => Color.FromRgb(p, q, v),
            4 => Color.FromRgb(t, p, v),
            _ => Color.FromRgb(v, p, q)
        };
    }
}
