using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class JoinButton : MonoBehaviour
{
    public string localIpAddress = "127.0.0.1"; // Dirección local del host
    public int port = 5000; // Puerto donde corre el host

    public void OnButtonClick()
    {
        // Primero intenta conectarse al host (tu propio ordenador)
        TcpClient client = new TcpClient();
        try
        {
            client.Connect(localIpAddress, port);
            Debug.Log("Successfully connected to the host.");
            
            // Si la conexión es exitosa, carga la escena del juego
            SceneManager.LoadScene("Game");
        }
        catch (SocketException)
        {
            Debug.LogError("Failed to connect to host. Is the server running?");
        }
    }
}