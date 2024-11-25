using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;



public class Client : MonoBehaviour
{
    private TcpClient _client;
    private NetworkStream _stream;
    private Blob _blob;

    private Dictionary<string, GameObject> _otherPlayers = new Dictionary<string, GameObject>(); // Store other players

    void Start()
    {
        _blob = GetComponent<Blob>();
        ConnectToServer("127.0.0.1", 5000);
    }

    void ConnectToServer(string ipAddress, int port)
    {
        _client = new TcpClient();
        _client.Connect(ipAddress, port);
        _stream = _client.GetStream();
        Debug.Log("Connected to server");
    }

    void Update()
    {
        if (_stream != null && _stream.CanWrite)
        {
            SendPlayerData();
        }

        if (_stream != null && _stream.CanRead)
        {
            ReceivePlayersData();
        }
    }

    void SendPlayerData()
    {
        // Asegúrate de que el sprite renderer esté en el mismo objeto
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            string playerData = JsonUtility.ToJson(new PlayerData(
                spriteRenderer.color.ToString(),
                _blob.Size,
                transform.position
            ));
            byte[] data = Encoding.UTF8.GetBytes(playerData);
            _stream.Write(data, 0, data.Length);
        }
        else
        {
            Debug.LogError("No Renderer found on player object!");
        }
    }

    void ReceivePlayersData()
    {
        if (_stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            List<PlayerData> playersData = JsonUtility.FromJson<List<PlayerData>>(receivedData);
            foreach (var playerData in playersData)
            {
                if (!_otherPlayers.ContainsKey(playerData.color))
                {
                    // Create new player if not exists
                    GameObject newPlayer = CreateNewPlayer(playerData.color);
                    _otherPlayers[playerData.color] = newPlayer;
                }

                // Update existing player position and size
                var existingPlayerBlob = _otherPlayers[playerData.color].GetComponent<Blob>();
                existingPlayerBlob.Size = playerData.size;
                _otherPlayers[playerData.color].transform.position = playerData.position;
            }
        }
    }

    GameObject CreateNewPlayer(string color)
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.AddComponent<SpriteRenderer>().color = ColorFromHex(color);
        playerObject.AddComponent<Blob>();
        return playerObject;
    }

    Color ColorFromHex(string hex)
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(hex, out newColor);
        return newColor;
    }

    private void OnApplicationQuit()
    {
        _stream?.Close();
        _client?.Close();
    }
}

