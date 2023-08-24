using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Services;
using System;
using USM.Devices;

namespace RoomHumidty.SensorServices;
public interface ISensorService : IEnableable, IService {
    public string SensorSuffix { get; }
    public SensorService SensorService { get; }
}

public static class ISensorServiceExtensions {
    private static readonly TimeSpan s_timeout = TimeSpan.FromMilliseconds(500);

    public static async Task<double> GetValueAsync(this ISensorService sensorService) {
        SensorDevice? device = sensorService.GetSensorDevice();
        try {
            return device is not null
                ? await device.GetValueAsync(s_timeout)
                : double.NaN;
        } catch (TimeoutException) {
            return double.NaN;
        }
    }

    public static SensorDevice? GetSensorDevice(this ISensorService sensorService) => sensorService.SensorService.GetDeviceBySuffix(sensorService.SensorSuffix);
}