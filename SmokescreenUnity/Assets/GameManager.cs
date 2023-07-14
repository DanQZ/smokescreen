using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject wallPrefab;
    public GameObject wallsParent;
    public CameraMotor mainCameraMotor;
    int mapSizeX = 100;
    int mapSizeY = 100;
    public Player currentPlayer;

    public List<Wall> allWalls = new List<Wall>();

    public Wall[,] mapGrid;

    void Awake()
    {
        mainCameraMotor.lookAt = this.gameObject.transform;
        mapGrid = new Wall[mapSizeX, mapSizeY];
    }
    void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        StartGame();
    }

    void StartGame()
    {
        GameObject newPlayer = SpawnPlayer();
        currentPlayer = newPlayer.GetComponent<Player>();
        mainCameraMotor.lookAt = newPlayer.gameObject.transform;
        BuildMap();
    }
    void EndGame()
    {
        Destroy(currentPlayer.gameObject);
        mainCameraMotor.lookAt = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(
            playerPrefab,
            transform.position + 3f * Vector3.up + 3f * Vector3.right,
            transform.rotation
            );
        return newPlayer;
    }

    void BuildMap()
    {
        Vector3 bottomLeft = new Vector3(0f, 0f, 0f);
        Vector3 bottomRight = new Vector3(10f, 0f, 0f);
        Vector3 topLeft = new Vector3(0f, 10f, 0f);
        Vector3 topRight = new Vector3(10f, 10f, 0f);


        BuildWalls(topLeft, Vector3.right, 10);
        BuildWalls(topRight, -1f * Vector3.up, 10);
        BuildWalls(bottomRight, -1f * Vector3.right, 10);
        BuildWalls(bottomLeft, Vector3.up, 10);


    }

    // no negative numbers
    void BuildWall(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            Debug.Log($"({x},{y}) is out of bounds");
            return;
        }
        Vector3 position = new Vector3((float)x, (float)y, 0f);
        GameObject newWall = Instantiate(wallPrefab, position, transform.rotation);
        newWall.transform.parent = wallsParent.transform;

        Wall newWallScript = newWall.GetComponent<Wall>();
        newWallScript.GM = this;
        mapGrid[x, y] = newWallScript;
        allWalls.Add(newWallScript);
    }
    void BuildWalls(Vector3 startPos, Vector3 direction, int length)
    {
        Vector3 curPos = new Vector3(Mathf.Floor(startPos.x), Mathf.Floor(startPos.y), 0f);
        Vector3 change = Vector3.Normalize(direction);
        for (int i = 0; i < length; i++)
        {
            BuildWall((int)curPos.x, (int)curPos.y);
            curPos += change;
        }
    }
}
