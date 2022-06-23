using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{

	class Program
	{
		static string myname = "";
		static Boolean flag = false;
		static void Main(string[] args)
		{
			IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
			Socket CilentSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			CilentSocket.Connect(localEndPoint);
			// Registration
			Console.WriteLine("Enter Your Name");
			while (true)
			{
				string nameofclient = null;
				nameofclient = Console.ReadLine();
				CilentSocket.Send(Encoding.ASCII.GetBytes(nameofclient), 0, nameofclient.Length, SocketFlags.None);
				byte[] msg = new Byte[1024];
				int size = CilentSocket.Receive(msg);
				string value = Encoding.ASCII.GetString(msg, 0, size);
				if (value == "Ok")
				{
					myname = nameofclient.ToLower();
					break;
				}
				Console.WriteLine("Message from Server -> {0}", value);
				Console.WriteLine("Enter Your Name Again");
			}
			Console.WriteLine("You Are Registered");
			Console.WriteLine("\nEnter Your Friend Name");
			//Connection With Friend
			CON_FRIEND(CilentSocket);
		}
		public static void CON_FRIEND(Socket CilentSocket)
		{
			while (true)
			{
				string nameoffriend = null;
				nameoffriend = Console.ReadLine();
				if (nameoffriend.ToLower() == myname)
				{
					Console.WriteLine("Please Dont Write Your Name");
					Console.WriteLine("Enter Your Friend Name Again");
				}
				else
				{
					CilentSocket.Send(Encoding.ASCII.GetBytes(nameoffriend), 0, nameoffriend.Length, SocketFlags.None);
					byte[] msg = new Byte[1024];
					int size = CilentSocket.Receive(msg);
					string value = Encoding.ASCII.GetString(msg, 0, size);
					if (value == "Friend Not Found")
					{
						Console.WriteLine("\nMessage from Server -> {0}", value);
						Console.WriteLine("\nEnter Your Friend Name Again");
					}
					else if (value == "Your Friend Is Busy With Other")
					{
						Console.WriteLine("\nMessage from Server -> {0}", value);
						Console.WriteLine("\nEnter Another Friend Name or Try Later");
					}
					else if (value == "Your Friend Is Waiting For Other") {
						Console.WriteLine("\nMessage from Server -> {0}" ,value);
						Console.WriteLine("\nEnter Another Friend Name or Try Later");
					}
					else if (value == "Waiting For Your Friend Connection")
					{
						Console.WriteLine("\nMessage from Server -> {0}", value);
						byte[] wait = new Byte[1024];
						int wait_size = CilentSocket.Receive(wait);
						string wait_value = Encoding.ASCII.GetString(wait, 0, wait_size);
						if (wait_value == "GOOD")
						{
							CilentSocket.Send(Encoding.ASCII.GetBytes("OK"));
							flag = true;
							break;
						}
						else if (wait_value == "Your Friend Want To Chat With Other")
						{
							CilentSocket.Send(Encoding.ASCII.GetBytes("NOT_OK"));
							Console.WriteLine("\nMessage from Server -> {0}", wait_value);
							Console.WriteLine("\nEnter Another Friend Name or Try Later");
						}
                        else {
						CilentSocket.Send(Encoding.ASCII.GetBytes("NOT_OK"));
						Console.WriteLine("\nMessage from Server -> Your Friend Is Connected With Other");
						Console.WriteLine("\nEnter Another Friend Name or Try Later");
					}
					}
					else
					{
						break;
					}
				}
			}
			Console.WriteLine("\n********************You Are Connected With Your Friend*******************");
			CHATING(CilentSocket);
		}

		public static void CHATING(Socket CilentSocket)
		{
			if (flag) {
				byte[] msg = new Byte[1024];
				int size = CilentSocket.Receive(msg);
				Console.WriteLine(Encoding.ASCII.GetString(msg, 0, size));
			}
			while (true)
			{
				string messageFromClient = null;
				Console.WriteLine("Enter The Message");
				messageFromClient = Console.ReadLine();
				CilentSocket.Send(Encoding.ASCII.GetBytes(messageFromClient), 0, messageFromClient.Length, SocketFlags.None);
				if (messageFromClient == "END") {
					flag = false;
					Console.WriteLine("\n*************** CHAT ENDED **************");
					Console.WriteLine("\nEnter Your Friend Name");
					break;

				}
				byte[] msg = new Byte[1024];
				int size = CilentSocket.Receive(msg);
				Console.WriteLine(Encoding.ASCII.GetString(msg, 0, size));
				String messageToClient = Encoding.ASCII.GetString(msg, 0, size);
				if (messageToClient == "END")
				{
					CilentSocket.Send(Encoding.ASCII.GetBytes("END1"));
					flag = false;
					Console.WriteLine("\n*************** CHAT ENDED **************");
					Console.WriteLine("\nEnter Your Friend Name");
					break;

				}
			}
			//Connection With Friend
			CON_FRIEND(CilentSocket);
		}
	}
}