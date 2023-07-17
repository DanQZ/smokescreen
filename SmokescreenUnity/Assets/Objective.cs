using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public GameManager GM;
    public string type;
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
                break;
            case "secondary":
                GM.secondaryObjectivesLeft--;
                break;
        }
        Destroy(this.gameObject);
    }
}
