using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScript : MonoBehaviour
{
    public Tilemap tilemap; // assign this in Inspector
    public TileBase tile; // assign this in Inspector (RuleTile or any other tile)

    void Update()
    {
        if (Input.GetMouseButton(0)) // if pressing down left mouse
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // zero z
            var cell = tilemap.WorldToCell(mouseWorldPos);
            tilemap.SetTile(cell, tile);
        }
    }
}