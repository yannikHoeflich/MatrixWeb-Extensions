using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.Logging;
using USM.Devices;

namespace RoomHumidty.SensorServices;
public class SensorService : IService, IInitializable {
    private readonly List<SensorDevice> _devices = new();

    private readonly ILogger _logger;

    public bool IsEnabled => _devices.Count > 0;

    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;

    public SensorService(ILogger<SensorService> logger) {
        _logger = logger;
    }

    public async Task ScanAsync() {
        _logger.LogInformation("Searching for sensor devices");
        while (_devices.Count == 0) {
            _logger.LogDebug("Scanning . . .");
            await foreach (IDevice device in Device.ScanAsync()) {
                if (device is not SensorDevice sensorDevice) {
                    continue;
                }

                if (_devices.Exists(x => x.Id == sensorDevice.Id)) {
                    continue;
                }

                await sensorDevice.InitAsync();
                _devices.Add(sensorDevice);
            }
        }

        _logger.LogInformation("Found {deviceCount} devices", _devices.Count);
    }

    public SensorDevice? GetDeviceBySuffix(string suffix) => _devices.Find(x => x.Suffix == suffix);

    public async Task<double?> GetValueBySuffixAsync(string suffix) {
        SensorDevice? device = GetDeviceBySuffix(suffix);
        if (device is null) {
            return null;
        }

        try {
            return await device.GetValueAsync(TimeSpan.FromSeconds(1));
        } catch {
            _logger.LogWarning("Value request timed out");
            return null;
        }
    }

    public InitResult Init() {
        _ = ScanAsync();
        return InitResult.Success;
    }
}
