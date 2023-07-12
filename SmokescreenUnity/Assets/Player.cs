using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hp;
    float air;
    float stamina;
    float water;
    float speed = 3f;
    public Rigidbody2D playerRB;
    public GameObject playerSprite;
    public Vector3 lookDirection;
    // Start is called before the first frame update
    void Start()
    {
        InitStats();
    }

    void InitStats(){
        hp = 100f;
        air = 100f;
        stamina = 100f;
        water = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardControls();
    }
    void KeyboardControls()
    {
        Vector3 moveVector = new Vector3(0f,0f,0f);
        if (Input.GetKey("w"))
        {
            moveVector += transform.up;
        }
        if (Input.GetKey("s"))
        {
            moveVector -= transform.up;
        }
        if (Input.GetKey("d"))
        {
            moveVector += transform.right;
        }
        if (Input.GetKey("a"))
        {
            moveVector -= transform.right;
        }
        if(Input.GetKey("space")){
            SprayWater();
        }
        moveVector = Vector3.Normalize(moveVector);
        lookDirection = moveVector;
        playerRB.velocity = moveVector * speed;
    }
    void SprayWater(){
        Debug.Log("spraying water");
    }
}
