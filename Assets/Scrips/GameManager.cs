using UnityEngine;

namespace Scrips
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab; // Prefab del jugador
        public FoodSpawner foodSpawner; // Spawner de comida

        void Start()
        {
            SpawnPlayer(); // Spawnear jugador al inicio//
        }

        void SpawnPlayer()
        {
            GameObject player = Instantiate(playerPrefab, new Vector2(0, 0), Quaternion.identity);
    
            // Hacer que la cámara sea un hijo del jugador
            Camera.main.transform.SetParent(player.transform);
    
            // Ajustar la posición de la cámara si es necesario
            Camera.main.transform.localPosition = new Vector3(0, 0, -10);
        }
    }
}
