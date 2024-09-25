using System;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Connecting to server...");
var client = new TcpClient();
client.Connect("192.168.1.52", 44444);
Console.WriteLine("Connected to server: " + client.Client.RemoteEndPoint);

var stream = client.GetStream();

string ReceiveQuestion()
{
    var buffer = new byte[1000];
    int bytesReceived = stream.Read(buffer, 0, buffer.Length);
    return Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();
}

void SendResponse(string response)
{
    byte[] responseBytes = Encoding.ASCII.GetBytes(response);
    stream.Write(responseBytes, 0, responseBytes.Length);
    stream.Flush();
}

// Responder preguntas del servidor
Console.WriteLine(ReceiveQuestion());
string name = Console.ReadLine();
SendResponse(name);

Console.WriteLine(ReceiveQuestion());
string age = Console.ReadLine();
SendResponse(age);

Console.WriteLine(ReceiveQuestion());
string gender = Console.ReadLine();
SendResponse(gender);

Console.WriteLine(ReceiveQuestion());
string characterClass = Console.ReadLine();
SendResponse(characterClass);

Console.WriteLine(ReceiveQuestion());
string race = Console.ReadLine();
SendResponse(race);

// Recibir y mostrar la ficha del personaje
string characterSheet = ReceiveQuestion();
Console.WriteLine(characterSheet);

client.Close();