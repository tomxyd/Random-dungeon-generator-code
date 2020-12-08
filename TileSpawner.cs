using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    DungeonGenerator dungeonGenerator;
    // Start is called before the first frame update
    private void Awake()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        GameObject goFLoor = Instantiate(dungeonGenerator.floor, transform.position, Quaternion.identity);
        goFLoor.name = dungeonGenerator.floor.name;
        goFLoor.transform.SetParent(dungeonGenerator.transform);
        //using this for setting up item spawnings
        if (transform.position.x > dungeonGenerator.maxX)
        {
            dungeonGenerator.maxX = transform.position.x;
        }
        if (transform.position.x < dungeonGenerator.minX)
        {
            dungeonGenerator.minX = transform.position.x;
        }
        if (transform.position.y > dungeonGenerator.maxY)
        {
            dungeonGenerator.maxY = transform.position.y;
        }
        if (transform.position.y < dungeonGenerator.minY)
        {
            dungeonGenerator.minY = transform.position.y;
        }
    }
    void Start()
    {
        LayerMask envMask = LayerMask.GetMask("Wall", "Floor");
        Vector2 size = Vector2.one * 0.8f;
        //for loop to check each side of a tile if there is a wall surrounding the floor tile
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 targetPos = new Vector2(transform.position.x + x, transform.position.y + y);
                Collider2D hit = Physics2D.OverlapBox(targetPos, size, 0, envMask);
                if (!hit)
                {
                    GameObject goWall = Instantiate(dungeonGenerator.wall, targetPos, Quaternion.identity)as GameObject;
                    goWall.name = dungeonGenerator.wall.name;
                    goWall.transform.SetParent(dungeonGenerator.transform);
                }
            }
          
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
