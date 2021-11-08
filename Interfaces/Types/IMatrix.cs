namespace DeviceInterfaces
{
	interface IMatrix
	{
		bool Route();   //don't know how we are addressing sources and destinations yet
		bool Deroute(); //don't know how we are derouteing yet
		bool DerouteAll();
	}
}
