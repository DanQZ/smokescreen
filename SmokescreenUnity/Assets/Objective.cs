using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameManager GM;
    public string type;
    public AudioSource airSound;
    public AudioSource healthSound;
    public AudioSource objectiveSound;
    // set in physics to only collide with player
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Objective collected");
        switch (type)
        {
            default:
                Debug.Log("ERROR: invalid objective type");
                break;
            case "primary":
                GM.primaryObjectivesLeft--;
                objectiveSound.Play();
                break;
            case "secondary":
                GM.secondaryObjectivesLeft--;
                objectiveSound.Play();
                break;
            case "air":
                GM.currentPlayer.air += 50f;
                airSound.Play();
                break;
            case "water":
                GM.currentPlayer.water += 50f;
                break;
            case "health":
                GM.currentPlayer.hp += 25f;
                healthSound.Play();
                break;
        }
        Destroy(this.gameObject);
    }
}
