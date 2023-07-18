using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public GameManager GM;
    public Player currentPlayer;

    // physics settings only lets it collide with player
    void OnTriggerEnter2D(Collider2D collision)
    {
        currentPlayer.isInside = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        currentPlayer.isInside = false;
    }
}
