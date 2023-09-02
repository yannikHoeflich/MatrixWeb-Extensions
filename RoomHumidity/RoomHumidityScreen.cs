using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;
using MatrixWeb.Extensions.Weather.Data;
using MatrixWeb.Extensions.Weather.Services;
using RoomHumidity;
using RoomHumidty.SensorServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RoomHumidty;
public class RoomHumidityScreen : IScreenGenerator {
    private readonly RoomHumidityService _roomHumidityService;
    private readonly SymbolLoader _symbolLoader;
    private readonly WeatherService _weatherService;
    private readonly ColorHelper _colorHelper;

    public Text Name { get; } = new Text(new TextElement(LanguageCode.EN, "Raum Humidity"), new TextElement(LanguageCode.DE, "Raum Luftfeuchtigkeit"));

    public Text Description { get; } = new Text(
                                        new TextElement(LanguageCode.EN, "Shows the current air humidity in your room."),
                                        new TextElement(LanguageCode.DE, "Zeigt die aktuelle Luftfeuchtigkeit im Raum an.")
                                       );

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(1);

    public bool IsEnabled => _weatherService.IsEnabled && _roomHumidityService.IsEnabled;

    public bool RequiresInternet => true;

    public RoomHumidityScreen(RoomHumidityService roomHumidityService, SymbolLoader symbolLoader, WeatherService weatherService, ColorHelper colorHelper) {
        _roomHumidityService = roomHumidityService;
        _symbolLoader = symbolLoader;
        _weatherService = weatherService;
        _colorHelper = colorHelper;
    }

    public async Task<Screen> GenerateImageAsync() {
        double value = await _roomHumidityService.GetValueAsync();
        if(double.IsNaN(value)) {
            return Screen.Empty;
        }

        WeatherStatus currentWeather = await _weatherService.GetWeatherAsync();

        Color color = _colorHelper.MapRoomHumidity(value, currentWeather.Humidity);

        var image = new Image<Rgb24>(16, 16);
        _symbolLoader.DrawNumber(image, (int)value, 2, 1, 4, color);
        _symbolLoader.DrawSymbol(image, 10, 4, '%', color);

        return new Screen(image, ScreenTime);
    }
}
