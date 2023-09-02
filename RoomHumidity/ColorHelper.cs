using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using SixLabors.ImageSharp;
using System;

namespace RoomHumidity;
public class ColorHelper : IService, IInitializable {
    private const string s_configName = "colors";
    private const string s_badHumidityDifferenceName = "bad-humidity-difference";

    private readonly ConfigService _configService;
    private double _badHumidityDifference = 10;

    public bool IsEnabled { get; } = true;

    public ConfigLayout ConfigLayout { get; } = new ConfigLayout() { 
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new(s_badHumidityDifferenceName, typeof(double))
        }
    };

    public ColorHelper(ConfigService configService) {
        _configService = configService;
    }

    public InitResult Init() {
        RawConfig? config = _configService.GetConfig(s_configName);
        if (config is null) {
            return InitResult.NoConfig();
        }

        if (config.TryGetDouble(s_badHumidityDifferenceName, out double badHumidityDifference)) {
            _badHumidityDifference = badHumidityDifference;
        } else {
            config.Set(s_badHumidityDifferenceName, _badHumidityDifference);
        }
        return InitResult.Success;
    }

    public Color MapRoomHumidity(double roomHumidity, double outsideHumidity) {
        double difference = roomHumidity - outsideHumidity;
        return MapColor(difference, 0, _badHumidityDifference, 120, 0);
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
