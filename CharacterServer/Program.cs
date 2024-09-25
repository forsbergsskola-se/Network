using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting up server...");
var listener = new TcpListener(IPAddress.Any, 44444);
listener.Start();
Console.WriteLine("Listening for incoming connections on " + listener.LocalEndpoint + "...");

while (true)
{
    var client = listener.AcceptTcpClient(); 
    Console.WriteLine("Client accepted: " + client.Client.RemoteEndPoint);

    var stream = client.GetStream();
    
    // Function to send question and receive answer
    string AskQuestion(string question)
    {
        byte[] questionBytes = Encoding.ASCII.GetBytes(question);
        stream.Write(questionBytes, 0, questionBytes.Length);
        stream.Flush();

        var buffer = new byte[1000];
        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();
    }

    // Preguntar y recibir los datos del personaje
    string name = AskQuestion("What is your character's name?");
    string age = AskQuestion("What is your character's age?");
    string gender = AskQuestion("What is your character's gender?");
    string characterClass = AskQuestion("What is your character's class?");
    string race = AskQuestion("What is your character's race?");

    // Crear la ficha del personaje
    string characterSheet = $"Character Sheet:\nName: {name}\nAge: {age}\nGender: {gender}\nClass: {characterClass}\nRace: {race}";

    // Enviar la ficha del personaje al cliente
    byte[] sheetBytes = Encoding.ASCII.GetBytes(characterSheet);
    stream.Write(sheetBytes, 0, sheetBytes.Length);
    stream.Flush();

    client.Close();
    Console.WriteLine("Character sheet sent. Connection closed.");
}