using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public int foodAmount = 50;
    public float spawnRadius = 20f;

    private List<GameObject> spawnedFood = new List<GameObject>();

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")

        {
            for (int i = 0; i < foodAmount; i++)
            {
                SpawnFood();
            }
        }
    }

    void SpawnFood()
    {
        Vector2 spawnPosition = Random.insideUnitCircle * spawnRadius;
        // spawnPosition.z = 0;
        GameObject newFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        spawnedFood.Add(newFood);
    }
}