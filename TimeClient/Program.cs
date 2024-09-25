using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Connecting to server...");
var client = new TcpClient();
client.Connect("192.168.1.52", 44444);
Console.WriteLine("Connected to server: " + client.Client.RemoteEndPoint);

// Create an array in which we can store the received bytes
var buffer = new byte[1000];
var stream = client.GetStream();

// Read as many bytes as we can from the stream. Maximum 1000. The first byte of the array
// can be used.
var bytesReceived = stream.Read(buffer, 0, buffer.Length);

// Convert the bytes to readable text using ASCCI format
var serverAnswer = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

Console.WriteLine(serverAnswer);
client.Close();
