using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;
    public CameraMotor mainCameraMotor;

    public Player currentPlayer;
    
    void Awake(){
        mainCameraMotor.lookAt = this.gameObject.transform;
    }
    void Start()
    {
        transform.position = new Vector3(0f,0f,0f);
        StartGame();
    }

    void StartGame(){
        GameObject newPlayer = SpawnPlayer();
        currentPlayer = newPlayer.GetComponent<Player>();
        mainCameraMotor.lookAt = newPlayer.gameObject.transform;
    }
    void EndGame(){
        Destroy(currentPlayer.gameObject);
        mainCameraMotor.lookAt = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject SpawnPlayer(){
        GameObject newPlayer = Instantiate(playerPrefab, transform.position, transform.rotation);
        return newPlayer;
    }
}
