using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace Apollo
{
    internal class Logger
    {
        internal DebugMode DebugMode { get; set; }
        internal Logger()
        {
            CrestronConsole.AddNewConsoleCommand(DebugToConsoleCallback, "debugtoconsole", "Print debug data to console.", ConsoleAccessLevelEnum.AccessOperator, true);
        }
        private void DebugToConsoleCallback(string parameters)
        {
            try
            {
                string[] parameterArray = parameters.Split(' ');
                if (parameterArray.Length > 0)
                {
                    if (parameterArray[0].Contains("?"))
                        //Add command response for help.
                        if (parameterArray[0] != String.Empty)
                            DebugMode = (DebugMode)Enum.Parse(typeof(DebugMode), parameterArray[0], true);
                    CrestronConsole.ConsoleCommandResponse("Debug mode '{0}'.", DebugMode);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Exception(String.Format("Logger>DebugToConsoleCommandCallback # {0} - {1}ms #",
                    DateTime.Now, CrestronEnvironment.TickCount), e);
            }
        }
    }
    
    internal enum DebugMode
    {
        Off,
        All
    }
}
