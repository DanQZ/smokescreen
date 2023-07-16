using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Player currentPlayer;
    // set in unity physics that this will ONLY collide with the player
    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("player shit");
        currentPlayer.TakeDamage(2f * Time.deltaTime);
    }
}
