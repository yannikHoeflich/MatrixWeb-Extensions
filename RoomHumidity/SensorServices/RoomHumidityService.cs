using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System;
using USM.Devices;

namespace RoomHumidty.SensorServices;
public class RoomHumidityService : ISensorService {
    public string SensorSuffix { get; } = "%";
    public SensorService SensorService { get; }

    public bool IsEnabled => this.GetSensorDevice() is not null;

    public RoomHumidityService(SensorService sensorService) {
        SensorService = sensorService;
    }
}
