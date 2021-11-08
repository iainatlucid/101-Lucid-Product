namespace DeviceInterfaces
{
	interface IDialler
	{
		bool Dial(string resource); //pretty sure it's a string
		bool Hangup();
	}
}
