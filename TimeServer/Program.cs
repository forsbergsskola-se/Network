using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting up server...");
var listener = new TcpListener(IPAddress.Any, 44444);
listener.Start();
Console.WriteLine("Listening for incoming connections on " + listener.LocalEndpoint + "...");



while (true)
{
    // Accept a new incoming connection. Wait for it.
    var client = listener.AcceptTcpClient(); // Blocking
    Console.WriteLine(("Client accepted: " + client.Client.RemoteEndPoint));

    // The text we want to send: 
    string currentTime = "Hola Putit@. The current time is: " + DateTime. Now.ToString("f");
// Encode the text using ASCII:
// The server commands the protocol:
// I send one ASSCII-Encoded String and then close the connection.
    byte[] bytes = Encoding.ASCII.GetBytes(currentTime);
    
// Send the bytes over the clients stream:
    var stream = client.GetStream();
    stream.Write(bytes);
    stream.Flush();

    client.Close();
    Console.WriteLine("Closing server...");
}
