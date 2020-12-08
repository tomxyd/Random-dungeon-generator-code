
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum DungeonType { Caverns, Rooms, WindingHalls}
public class DungeonGenerator : MonoBehaviour
{
    [Header("Values")]
    [HideInInspector]public float minX, minY, maxX, maxY;
    [Range(50, 1000)]public int totalFloorCount;
    private Vector2 hitSize;
    [Range(1,100)]public int hallwayPercentage;
    public DungeonType dungeonType;

    [Space]
    [Header("Bools")]
    public bool useRoundedEdges;
    
    [Space]
    [Header("Lists & Arrays")]
    private List<Vector3> floorList = new List<Vector3>();
    public GameObject[] topWall;
    public GameObject[] randomItems;
    public GameObject wall, floor, exit, tilePrefab;
    public LayerMask wallMask, floorMask;
   

    private void Start()
    {
        switch (dungeonType)
        {
            case DungeonType.Caverns:RandomWalker(); break;
            case DungeonType.Rooms: RoomWalker(); break;
            case DungeonType.WindingHalls: HallRoomWalker(); break;
        }
        hitSize = Vector2.one * 0.8f;
    
    }
    private void Update()
    {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void RandomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos += RandomDirection();
            if (!InFloorList(currentPos))
            {
                floorList.Add(currentPos);
            }
        } //set at a new position
        StartCoroutine(DelayProgress());
       
    }
    void RoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            RandomRoom(currentPos);

        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    void HallRoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            int roll = Random.Range(1, 100);
            if(roll > hallwayPercentage)
            {
                RandomRoom(currentPos);
            }
           

        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    Vector3 LongWalk(Vector3 myPos)
    {
        Vector3 walkDir = RandomDirection();
        int walkLength = Random.Range(9, 10);
        myPos += RandomDirection();
        for (int i = 0; i < walkLength; i++)
        {
            if (!InFloorList(myPos + walkDir))
            {
                floorList.Add(myPos + walkDir);
            }
            myPos += walkDir;
        }
        return myPos;
    }
    void RandomRoom(Vector3 myPos)
    {
        int width = Random.Range(1, 5);
        int height = Random.Range(1, 5);
        for (int w = -width; w <= width; w++)
        {
            for (int h = -height; h < height; h++)
            {
                Vector3 offset = new Vector3(w, h, 0);
                if (!InFloorList(myPos + offset))
                {
                    floorList.Add(myPos+ offset);
                }
            }
        }
    }
    bool InFloorList(Vector3 myPos)
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            if(Vector3.Equals(myPos, floorList[i]))
            {
                return true;
            }      
        }
        return false;
    }
    Vector3 RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                return Vector3.up;

            case 2:
                return Vector3.right;

            case 3:
                return Vector3.down;

            case 4:
                return Vector3.left;
        }
        return Vector3.zero;
    }
    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity) as GameObject;
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoor();
        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; y++)
            {
                //Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize, floorMask);
                //if (hitFloor)
                //{
                //if (Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 2]))
                //{
                //Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, wallMask);
                //Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, wallMask);
                //Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, wallMask);
                //Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, wallMask);
                //}

                //}
                RoundedEdges(x, y);
            }
        }
    }
    void RoundedEdges(int x, int y)
    {
        if (useRoundedEdges)
        {
            Collider2D hitWall = Physics2D.OverlapBox(new Vector2(x, y), hitSize, wallMask);
            if (hitWall)
            {
                Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                int bitValue = 0;
                if (!hitTop) { bitValue += 1; }
                if (!hitRight) { bitValue += 2; }
                if (!hitBottom) { bitValue += 4; }
                if (!hitLeft) { bitValue += 8; }
            
                if(bitValue == 2 || bitValue == 5 || bitValue == 12)
                {
                    GameObject goTop = Instantiate(topWall[Random.Range(0, topWall.Length)], new Vector2(x, y) - Vector2.one, Quaternion.identity)as GameObject;
                    goTop.name = wall.name;
                    goTop.transform.SetParent(transform);
                }
            }
        }
    }
    void ExitDoor()
    {
        Vector3 doorPos = floorList[floorList.Count - 1];
        {
            GameObject goExit = Instantiate(exit, doorPos, Quaternion.identity) as GameObject;
            goExit.name = tilePrefab.name;
            goExit.transform.SetParent(transform);
        }
    }
}
