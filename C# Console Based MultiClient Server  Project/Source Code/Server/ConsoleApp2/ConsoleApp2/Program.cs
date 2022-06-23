using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
	class Program
	{
		static ArrayList name_arr = new ArrayList();
		static List<Socket> socket_arr = new List<Socket>();
		static ArrayList Request_From = new ArrayList();
		static ArrayList Request_To = new ArrayList();
		static ArrayList Confirm_From = new ArrayList();
		static ArrayList Confirm_To = new ArrayList();
		static void Main(string[] args)
		{
			IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
			Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(localEndPoint);
			listener.Listen(10);
			Console.WriteLine("Server Is Running ... ");
			Socket clientSocket = default(Socket);
			Program p = new Program();
			while (true)
			{
				clientSocket = listener.Accept();
				Thread newthread = new Thread(new ThreadStart(() => p.User(clientSocket)));
				newthread.Start();

			}
		}
		public void User(Socket Client)
		{   //user name added into name array
			string myname;
			string friendname;
				while (true)
				{
					byte[] name = new Byte[1024];
					int name_len = Client.Receive(name);
					string strname = Encoding.ASCII.GetString(name, 0, name_len);
					strname = strname.Trim();
					strname = strname.ToLower();
					if (!name_arr.Contains(strname))
					{
						name_arr.Add(strname);
						socket_arr.Add(Client);
						myname = strname;
						Client.Send(Encoding.ASCII.GetBytes("Ok"));
						break;
					}
					else
					{
						Client.Send(Encoding.ASCII.GetBytes("User Already Exist"));
					}

				}

			while (true)
			{
				Boolean stopuser = false;
				Socket FriendSocket = default(Socket);
				Socket MySocket = default(Socket);
				// checking friend validatio
				while (true)
				{
					byte[] friend = new Byte[1024];
					int friend_len = Client.Receive(friend);
					friendname = Encoding.ASCII.GetString(friend, 0, friend_len);
					friendname = friendname.Trim();
					friendname = friendname.ToLower();
					if (!name_arr.Contains(friendname))
					{
						Client.Send(Encoding.ASCII.GetBytes("Friend Not Found"));
					}
					else if (Confirm_From.Contains(friendname) || Confirm_To.Contains(friendname))
					{
						Client.Send(Encoding.ASCII.GetBytes("Your Friend Is Busy With Other"));
					}
					else
					{
						Boolean flag = false;
						Boolean flag2 = false;
						string[] indexer = new string[10];
						Console.WriteLine(Request_To.Count);
						for (int i = 0; i < Request_To.Count; i++)
						{
							if (Request_From[i].Equals(friendname))
							{
								flag2 = true;
								if (Request_To[i].Equals(myname))
								{
									Request_From.RemoveAt(i);
									Request_To.RemoveAt(i);
									while (true)
									{
										int index = -1;
										index = Request_To.IndexOf(myname);
										if (index == -1)
										{
											break;
										}
										socket_arr[name_arr.IndexOf(Request_From[index])].Send(Encoding.ASCII.GetBytes("Not Ok"));
										Request_From.RemoveAt(index);
										Request_To.RemoveAt(index);
									}
									
									Confirm_From.Add(friendname);
									Confirm_To.Add(myname);
									FriendSocket = socket_arr[name_arr.IndexOf(friendname)];
									MySocket = socket_arr[name_arr.IndexOf(myname)];
									socket_arr[name_arr.IndexOf(friendname)].Send(Encoding.ASCII.GetBytes("GOOD"));
									Client.Send(Encoding.ASCII.GetBytes("OK"));
									flag = true;
									flag2 = false;
								}
								else
								{
									Client.Send(Encoding.ASCII.GetBytes("Your Friend Is Waiting For Other"));
								}
								break;
							}
						}
						if (flag)
						{
							break;
						}
						else if (!flag2)
						{
							Request_From.Add(myname);
							Request_To.Add(friendname);
							while (true)
							{
								int index = -1;
								index = Request_To.IndexOf(myname);
								if (index == -1)
								{
									break;
								}
								socket_arr[name_arr.IndexOf(Request_From[index])].Send(Encoding.ASCII.GetBytes("Your Friend Want To Chat With Other"));
								Request_From.RemoveAt(index);
								Request_To.RemoveAt(index);
							}
							Client.Send(Encoding.ASCII.GetBytes("Waiting For Your Friend Connection"));
							byte[] okornot = new Byte[1024];
							int okornot_len = Client.Receive(okornot);
							string strokornot = Encoding.ASCII.GetString(okornot, 0, okornot_len);
							Console.WriteLine(strokornot == "OK");
							if (strokornot == "OK")
							{
								FriendSocket = socket_arr[name_arr.IndexOf(friendname)];
								MySocket = socket_arr[name_arr.IndexOf(myname)];
								break;
							}
						}
					}
				}

				while (true)
				{
					byte[] msg = new Byte[1024];
					int size = MySocket.Receive(msg);
					string value = Encoding.ASCII.GetString(msg, 0, size);
					if (value == "END")
					{
						FriendSocket.Send(Encoding.ASCII.GetBytes("END"));
						Confirm_From.Remove(friendname);
						Confirm_To.Remove(myname);
						break;
					}
					if (value == "END1")
					{
						break;
					}
					Console.WriteLine("From: " + myname + "  To: " + friendname + "==>" + value);
					value = "From " + myname + "-->  " + value;
					FriendSocket.Send(Encoding.ASCII.GetBytes(value));
				}
			}
		}
		}
	}
