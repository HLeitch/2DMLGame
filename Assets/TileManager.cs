using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TileData
{
    TileData(Tile tile, JumpingState jumpType,int numberOfJumps,int xPosition)
    {
        this.tile = tile;
        this.jumpType = jumpType;
        this.numberOfJumps = numberOfJumps;
        this.xPosition = xPosition;
    }

    Tile tile;
    /// <summary>
    /// Type of jump required to complete
    /// </summary>
    JumpingState jumpType;
    /// <summary>
    /// jumps expected to complete
    /// </summary>
    int numberOfJumps;
    /// <summary>
    /// position in tilemap
    /// </summary>
    int xPosition;
}


public class TileManager : MonoBehaviour
{
    public List<List<Tile>> SingleJumpTileLists;
    public List<List<Tile>> DoubleJumpTileLists;
    public List<List<Tile>> DashJumpTileLists;
    public List<Tile> stepDownTiles;
    public Tile flatTile;

    [Header("Place Tiles Here. Sort by Jump type and number of Jumping 'events'")]
    [SerializeField]
    private List<Tile> SingleJump_1Event;
    [SerializeField]
    private List<Tile> SingleJump_2Event;
    [SerializeField]
    private List<Tile> SingleJump_3Event;
    [SerializeField]
    private List<Tile> SingleJump_4Event;

    [SerializeField]
    private List<Tile> DoubleJump_1Event;
    [SerializeField]
    private List<Tile> DoubleJump_2Event;
    [SerializeField]
    private List<Tile> DoubleJump_3Event;
    [SerializeField]
    private List<Tile> DoubleJump_4Event;

    [SerializeField]
    private List<Tile> DashJump_1Event;
    [SerializeField]
    private List<Tile> DashJump_2Event;
    [SerializeField]
    private List<Tile> DashJump_3Event;
    [SerializeField]
    private List<Tile> DashJump_4Event;



    // Start is called before the first frame update
    void Start()
    {
        ListAssignment();

    }

    private void ListAssignment()
    {
        SingleJumpTileLists.Add(SingleJump_1Event);
        SingleJumpTileLists.Add(SingleJump_2Event);
        SingleJumpTileLists.Add(SingleJump_3Event);
        SingleJumpTileLists.Add(SingleJump_4Event);

        DoubleJumpTileLists.Add(DoubleJump_1Event);
        DoubleJumpTileLists.Add(DoubleJump_2Event);
        DoubleJumpTileLists.Add(DoubleJump_3Event);
        DoubleJumpTileLists.Add(DoubleJump_4Event);

        DashJumpTileLists.Add(DashJump_1Event);
        DashJumpTileLists.Add(DashJump_2Event);
        DashJumpTileLists.Add(DashJump_3Event);
        DashJumpTileLists.Add(DashJump_4Event);
    }


    Tile GetTile(List<List<Tile>> tileListList, int tileSeed)
    {
        int countOfTileList = 0;
        foreach (List<Tile> list in tileListList)
        {
            countOfTileList += list.Count;

        }


        if (countOfTileList == 0)
        {
            return GetFlatTile();
        }

        int tileToGet = tileSeed % countOfTileList;

        int indexReached = 0;
        foreach (List<Tile> list in tileListList)
        {
            if ((tileToGet < indexReached + list.Count) && tileToGet > indexReached)
            {
                int indexOfTile = (tileToGet - indexReached);
                return list[indexOfTile];
            }

        }

        //Failure State
        Debug.LogError("Tile Retrieval Failed. Flat Tile Returned!!");
        return (GetFlatTile());
    }


    Tile GetTile(List<Tile> tileList,int tileSeed)
    {
        int tileToGet = tileSeed % tileList.Count;

        return (tileList[tileToGet]);
    }


    Tile GetStepUpTile(int tileSeed)
    {
        int i = (tileSeed % stepUpTiles.Count);
        return stepUpTiles[i];
    }

    Tile GetStepDownTile(int tileSeed)
    {
        int i = (tileSeed % stepDownTiles.Count);
        return stepDownTiles[i];
    }

    Tile GetDashJumpTile(int tileSeed)
    {
        int i = (tileSeed % dashJumpTiles.Count);
        return dashJumpTiles[i];
    }
    Tile GetFlatTile()
    {
        return flatTile;
    }
    Tile GetSingleJumpTile(int tileSeed)
    {
        return singleJumpTiles[0];
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
