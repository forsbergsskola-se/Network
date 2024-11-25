using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    private TcpListener _server;
    private List<ClientHandler> _clients = new List<ClientHandler>();
    private bool _isRunning;

    public string ipAddress = "127.0.0.1"; // IP del servidor, puedes cambiarla si necesitas
    public int port = 5000; // Puerto donde el servidor escucha
    public GameObject playerPrefab; // Prefab de los jugadores
    public GameObject foodPrefab;   // Prefab de la comida
    public int foodCount = 10;      // Número de comida a spawnear

    private List<GameObject> foodObjects = new List<GameObject>(); // Lista de los objetos de comida en el mapa
    private Queue<Action> _mainThreadActions = new Queue<Action>();


    // Inicializa el servidor cuando el objeto se activa
    void Start()
    {
        StartServer();
    }

    // Iniciar el servidor
    void StartServer()
    {
        try
        {
            _server = new TcpListener(IPAddress.Parse(ipAddress), port);
            _server.Start();
            _isRunning = true;
            Debug.Log("Server started on " + ipAddress + ":" + port);

            // Aceptar clientes en un hilo separado
            Thread acceptClientsThread = new Thread(AcceptClients);
            acceptClientsThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting server: " + e.Message);
        }
    }

    // Aceptar conexiones de clientes
    private void AcceptClients()
    {
        while (_isRunning)
        {
            TcpClient newClient = _server.AcceptTcpClient();
            ClientHandler clientHandler = new ClientHandler(newClient, this);
            _clients.Add(clientHandler);

            // Encolar la acción para ejecutarla en el hilo principal
            lock (_mainThreadActions)
            {
                _mainThreadActions.Enqueue(() => SpawnPlayer(clientHandler));
            }

            // Correr la lógica del cliente en un hilo separado
            Thread clientThread = new Thread(clientHandler.Run);
            clientThread.Start();
        }
    }


    // Spawnear comida aleatoriamente
    private void SpawnFood()
    {
        for (int i = 0; i < foodCount; i++)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0.5f, UnityEngine.Random.Range(-10f, 10f));
            GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
            foodObjects.Add(food);
        }
    }

    // Spawnear un jugador
    private void SpawnPlayer(ClientHandler client)
    {
        // Elegir una posición aleatoria para el jugador
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0.5f, UnityEngine.Random.Range(-10f, 10f));
        GameObject player = Instantiate(playerPrefab, randomPosition, Quaternion.identity);

        // Asignar posición inicial y otros parámetros
        client.Position = randomPosition;
        client.Size = 1f;
    }

    // Enviar los datos de todos los jugadores a los clientes conectados
    public void BroadcastAllPlayerData()
    {
        foreach (var client in _clients)
        {
            string allPlayersData = GetAllPlayersData();
            client.Send(allPlayersData);
        }
    }

    // Recopilar los datos de todos los jugadores
    private string GetAllPlayersData()
    {
        List<PlayerData> allPlayerData = new List<PlayerData>();
        foreach (var client in _clients)
        {
            allPlayerData.Add(new PlayerData(client.Color, client.Size, client.Position));
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(allPlayerData);
    }

    // Remover un cliente si se desconecta
    public void RemoveClient(ClientHandler client)
    {
        _clients.Remove(client);
    }

    // Verificar colisiones entre jugadores y la mecánica de "comerse"
    private void CheckCollisions()
    {
        for (int i = 0; i < _clients.Count; i++)
        {
            for (int j = i + 1; j < _clients.Count; j++)
            {
                float distance = Vector3.Distance(_clients[i].Position, _clients[j].Position);

                // Verificar si están lo suficientemente cerca como para colisionar
                if (distance < 0.5f) // Ajusta el valor según el tamaño de los jugadores
                {
                    if (_clients[i].Size >= _clients[j].Size * 1.2f)
                    {
                        // i es lo suficientemente grande como para comer a j
                        _clients[i].Size += _clients[j].Size * 0.5f; // El jugador grande crece un 50% del tamaño del jugador comido
                        _clients[j].Size = 0; // El jugador pequeño es eliminado (lo podrías destruir o reubicar)
                        Debug.Log($"{_clients[i].Color} ha comido a {_clients[j].Color}");
                    }
                    else if (_clients[j].Size >= _clients[i].Size * 1.2f)
                    {
                        // j es lo suficientemente grande como para comer a i
                        _clients[j].Size += _clients[i].Size * 0.5f;
                        _clients[i].Size = 0;
                        Debug.Log($"{_clients[j].Color} ha comido a {_clients[i].Color}");
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Procesar las acciones encoladas
        lock (_mainThreadActions)
        {
            while (_mainThreadActions.Count > 0)
            {
                var action = _mainThreadActions.Dequeue();
                action();
            }
        }
        // Cada frame, verificar colisiones
        CheckCollisions();
    }

    private void OnApplicationQuit()
    {
        // Detener el servidor al cerrar el juego
        _isRunning = false;
        _server?.Stop();
    }
}

public class ClientHandler
{
    private TcpClient _client;
    private NetworkStream _stream;
    private Server _server;
    private System.Random _random = new System.Random();


    public string Color { get; private set; }
    public float Size { get; set; } = 1f;
    public Vector3 Position { get; set; }

    public ClientHandler(TcpClient client, Server server)
    {
        _client = client;
        _server = server;
        _stream = _client.GetStream();

        // Asignar un color aleatorio al jugador
        Color = "#" + _random.Next(0x1000000).ToString("X6");
    }

    public void Run()
    {
        while (true)
        {
            byte[] buffer = new byte[256];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                _server.RemoveClient(this);
                break;
            }

            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            PlayerData playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerData>(receivedData);
            Position = playerData.position;
            Size = playerData.size;

            // Enviar datos actualizados a todos los jugadores
            _server.BroadcastAllPlayerData();
        }
    }

    public void Send(string data)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        _stream.Write(buffer, 0, buffer.Length);
    }
}

[Serializable]
public class PlayerData
{
    public string color;
    public float size;
    public Vector3 position;

    public PlayerData(string color, float size, Vector3 position)
    {
        this.color = color;
        this.size = size;
        this.position = position;
    }
    
    public PlayerData() { }
}

