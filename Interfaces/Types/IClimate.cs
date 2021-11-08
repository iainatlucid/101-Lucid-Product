using System;

namespace DeviceInterfaces
{
    interface IClimate
    {
        void Setpoint(decimal value);
        event Action<decimal> TemperatureChanged;
    }
}
