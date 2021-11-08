using System;

namespace DeviceInterfaces
{
    interface IAlarm
    {
        void Arm();
        void Disarm();

        event Action<int> AlarmStateChanged;
    }
}
