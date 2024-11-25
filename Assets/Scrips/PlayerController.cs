using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Blob _blob;
    void Start()
    {
        _blob = GetComponent<Blob>();
    }
    void Update()
    {
        var cursorInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorInWorld.z = 0; // Asegúrate de que el movimiento solo sea en 2D.
        _blob.Direction = cursorInWorld - transform.position;
        
        // var cursorInworld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // _blob.Direction = cursorInworld - transform.position;
    }
}
