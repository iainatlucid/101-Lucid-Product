using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Apollo.Routing;

namespace Apollo
{
    internal class Router
    {
        private ControlSystem _system; //A reference to the control system.      
        internal Router(ControlSystem system)
        {
            try
            {
                _system = system;
                CrestronConsole.AddNewConsoleCommand(CreateDevice, "createdevice", "", ConsoleAccessLevelEnum.AccessOperator, true);
                CrestronConsole.AddNewConsoleCommand(DeleteDevice, "deletedevice", "", ConsoleAccessLevelEnum.AccessOperator, true);
                CrestronConsole.AddNewConsoleCommand(GetDevices, "getdevices", "", ConsoleAccessLevelEnum.AccessOperator, true);
                CrestronConsole.AddNewConsoleCommand(GetRoutes, "getroutes", "", ConsoleAccessLevelEnum.AccessOperator, true);
                CrestronConsole.AddNewConsoleCommand(MakeRoute, "makeroute", "", ConsoleAccessLevelEnum.AccessOperator, true);
            }
            catch (Exception e)
            {
                ErrorLog.Exception("Router>Constructor # Error in the constructor.", e);
            }
        }

        private void CreateDevice(string parameters)
        {
            try
            {
                bool componentTypeFlag = false;
                bool nameFlag = false;
                string componentType = "";
                ComponentType componentTypeCommand = ComponentType.Unknown;
                string name = "";
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                    {
                        CreateDeviceDefaultResponse();
                        return;
                    }
                    foreach (var parameter in parameterArray)
                    {
                        if (parameter.Contains("-t:") || parameter.Contains("-T:"))
                        {
                            componentType = parameter.Replace("-t:", "").Replace("-T:", "").Trim();
                            componentTypeCommand = (ComponentType)Enum.Parse(typeof(ComponentType), componentType, true);
                            componentTypeFlag = true;
                        }
                        else if (parameter.Contains("-n:") || parameter.Contains("-N:"))
                        {
                            name = parameter.Replace("-n:", "").Replace("-N:", "").Trim();
                            nameFlag = true;
                        }
                    }
                    if (componentTypeFlag == true && nameFlag == true) //Check both parameters have been filled in successfully before running.
                    {
                        _system.Devices.Add(new Device(componentTypeCommand, name));
                        var deviceIndex = _system.Devices.Count - 1;
                        CrestronConsole.ConsoleCommandResponse("Device Guid '{0}' of type '{1}' with friendly name '{2}' created.\r\n",
                            _system.Devices[deviceIndex].Guid, (ComponentType)_system.Devices[deviceIndex].ComponentType, _system.Devices[deviceIndex].Name);
                    }
                    else
                        CreateDeviceDefaultResponse();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Router>CreateDeviceCallback # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
        private void CreateDeviceDefaultResponse()
        {
            CrestronConsole.ConsoleCommandResponse("createdevice -T:componentType -N:name\r\n" +
                "\t-T: Replace 'componentType' with device type name.\r\n" +
                "\t-N: Replace 'name' with device friendly name.\r\n");
        }
        private void DeleteDevice(string parameters)
        {
            try
            {
                bool nameFlag = false;
                bool guidNameFlag = false;
                string name = "";
                string guidName = "";
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                    {
                        DeleteDeviceDefaultResponse();
                        return;
                    }
                    foreach (var parameter in parameterArray)
                    {
                        if (parameter.Contains("-n:") || parameter.Contains("-N:"))
                        {
                            name = parameter.Replace("-n:", "").Replace("-N:", "").Trim();
                            nameFlag = true;
                        }
                        if (parameter.Contains("-g:") || parameter.Contains("-G:"))
                        {
                            guidName = parameter.Replace("-g:", "").Replace("-G:", "").Trim();
                            guidNameFlag = true;
                        }
                    }
                    if (nameFlag == true)
                    {
                        _system.Devices.RemoveAll(x => x.Name == name);
                    }
                    if (guidNameFlag == true)
                    {
                        _system.Devices.RemoveAll(x => x.Guid.ToString() == guidName);
                    }
                    else
                        DeleteDeviceDefaultResponse();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Router>DeleteDevice # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
        private void DeleteDeviceDefaultResponse()
        {
            CrestronConsole.ConsoleCommandResponse("deletedevice -N:name\r\n" +
                "deletedevice -G:guid\r\n" +
                "\t-N: Replace 'name' with device friendly name.\r\n" +
                "\t-G: Replace 'guid' with device Guid.\r\n");
        }
        private void GetDevices(string parameters)
        {
            try
            {
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                    {
                        GetDevicesDefaultResponse();
                        return;
                    }
                    else
                    {
                        if (_system.Devices.Count > 0)
                        {
                            CrestronConsole.ConsoleCommandResponse("Devices defined in system:\r\n");
                            foreach (var device in _system.Devices)
                            {
                                CrestronConsole.ConsoleCommandResponse("Device Guid '{0}' of type '{1}' with friendly name '{2}'.\r\n", device.Guid, device.ComponentType, device.Name);
                            }
                        }
                        else
                            CrestronConsole.ConsoleCommandResponse("No devices defined in system.");
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Router>GetDevices # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
        private void GetDevicesDefaultResponse()
        {

        }
        private void GetRoutes(string parameters)
        {
            try
            {
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                    {
                        GetRoutesDefaultResponse();
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Router>GetRoutes # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
        private void GetRoutesDefaultResponse()
        {

        }
        private void MakeRoute(string parameters)
        {
            try
            {
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                    {
                        MakeRouteDefaultResponse();
                        return;
                    }
                    foreach (var parameter in parameterArray)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Router>MakeRoute # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
        private void MakeRouteDefaultResponse()
        {

        }
    }
}
