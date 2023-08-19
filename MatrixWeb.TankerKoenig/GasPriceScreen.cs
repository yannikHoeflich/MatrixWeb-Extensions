using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeb.TankerKoenig;
public class GasPriceScreen : IScreenGenerator {
    private readonly GasPriceService _gasPrices;
    private readonly SymbolLoader _symbolLoader;

    public string Name { get; } = "Benzin Preis";

    public string Description { get; } = "Zeigt den günstigsten Benzin preis im Umkreis an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public bool IsEnabled => _gasPrices.IsEnabled;

    public bool RequiresInternet => true;

    public GasPriceScreen(GasPriceService gasPrices, SymbolLoader symbolLoader) {
        _gasPrices = gasPrices;
        _symbolLoader = symbolLoader;
    }

    public async Task<Screen> GenerateImageAsync() {
        double price = await _gasPrices.GetCheapestPriceAsync(51.954607, 8.668700);
        int priceCents = (int)(price * 100);

        Color color = ColorHelper.MapGasPrice(price, _gasPrices.MinPrice, _gasPrices.MaxPrice);

        var image = new Image<Rgb24>(16, 16);

        _symbolLoader.DrawNumber(image, priceCents, 3, 0, 4, color);
        _symbolLoader.DrawSymbol(image, 12, 4, 'c', color);

        return new Screen(image, ScreenTime);
    }
}