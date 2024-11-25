using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Blob : MonoBehaviour
{
    public Vector2 Direction { get; set; }
    private float _size = 1;
    public float Size
    {
        get => _size;
        set
        {
            transform.localScale = new Vector3(value, value, 1f);
            _size = value;
        } 
    }
    
    public Color BlobColor { get; private set; }
    
    public float BaseSpeed = 3f;
    
    SpriteRenderer spriteRenderer;
   
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BlobColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); // Color aleatorio
        GetComponent<Renderer>().material.color = BlobColor; // Asigna el color
    }

    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Direction.normalized * BaseSpeed / Size;
    }

    // If it is smaller than the other eat it.

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collided");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // If the other object is another blob (player)
        var otherBlob = other.GetComponent<Blob>();
        if (otherBlob != null && Size >= otherBlob.Size * 1.2f)
        {
            // Eat the smaller blob
            Size += otherBlob.Size; // Increase size
            Destroy(otherBlob.gameObject); // Remove the smaller blob from the game
        }
        
        // If it's food
        var food = other.GetComponent<Food>();
        if (food != null)
        {
            Debug.Log("Food detected!");
            Size += food.NutritionValue; // Increase size based on food's nutrition value
            Destroy(food.gameObject); // Remove food after eating
        }
    }
}

// Script to control player behavior
public class BlobController : MonoBehaviour
{
    private Blob _blob;
    
    void Start()
    {
        _blob = GetComponent<Blob>();
        AssignRandomColor(); // Assign a random color at the start
    }
    
    void Update()
    {
        // Move towards the cursor position
        var cursorInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _blob.Direction = cursorInWorld - transform.position;
    }

    // Assign a random color to the player
    private void AssignRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value); // Generate a random color
        GetComponent<Renderer>().material.color = randomColor; // Set the player's color
    }
}

// Script to represent the food item
public class Food : MonoBehaviour
{
    public float NutritionValue = 0.5f; // The amount of size the player gains when eating this food
}



