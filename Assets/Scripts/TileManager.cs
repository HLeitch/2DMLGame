using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public List<List<Tile>> SingleJumpTileLists = new List<List<Tile>>();
    public List<List<Tile>> DoubleJumpTileLists = new List<List<Tile>>();
    public List<List<Tile>> DashJumpTileLists = new List<List<Tile>>() ;

    Dictionary<List<List<Tile>>, JumpingState> TilelistToJumpState = new Dictionary<List<List<Tile>>, JumpingState>();


    public Tile flatTile;
    public List<Tile> stepDownTiles;
    public List<Tile> stepUpTiles;

    [Header("Place Tiles Here. Sort by Jump type and number of Jumping 'events'")]
    [SerializeField]
    private List<Tile> SingleJump_1Event = new List<Tile>();
    [SerializeField]
    private List<Tile> SingleJump_2Event = new List<Tile>();
    [SerializeField]
    private List<Tile> SingleJump_3Event = new List<Tile>();
    [SerializeField]
    private List<Tile> SingleJump_4Event = new List<Tile>();

    [SerializeField]
    private List<Tile> DoubleJump_1Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DoubleJump_2Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DoubleJump_3Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DoubleJump_4Event = new List<Tile>();

    [SerializeField]
    private List<Tile> DashJump_1Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DashJump_2Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DashJump_3Event = new List<Tile>();
    [SerializeField]
    private List<Tile> DashJump_4Event = new List<Tile>();



    // Start is called before the first frame update
    void Start()
    {
        ListAssignment();

    }

    /// <summary>
    /// Helper Function. Assigns tile lists to the relevent master list and assigns the relevent jump state to the master list.
    /// </summary>
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

        TilelistToJumpState.Add(SingleJumpTileLists, JumpingState.SingleJump);
        TilelistToJumpState.Add(DoubleJumpTileLists, JumpingState.DoubleJump);
        TilelistToJumpState.Add(DashJumpTileLists, JumpingState.Dash);
    }


    TileData GetTile(List<List<Tile>> tileListList, int tileSeed, int xPos)
    {
        int countOfTileList = 0;
        foreach (List<Tile> list in tileListList)
        {
            countOfTileList += list.Count;

        }


        if (countOfTileList == 0)
        {
            return new TileData(GetFlatTile(), JumpingState.Grounded, 0, xPos);      
        }

        int tileToGet = tileSeed % countOfTileList;

        //This +1 is the jump events of the tile returned.
        int jumpEvents = 1;

        //this marks where in the parent list the current list is. E.g. list 0 starts at 0, list 1 starts at 0 + list0.count
        int indexReached = 0;
        foreach (List<Tile> list in tileListList)
        {
            
            if ((tileToGet < indexReached + list.Count) && tileToGet >= indexReached)
            {
                
                int indexOfTile = (tileToGet - indexReached);

                //Prevents a list with no elements from being pulled from
                if (list.Count > 0)
                {
                    //Finds desired jumping state of tile. This is passed into tile data for analysis of performance.
                    JumpingState _jumpingState = JumpingState.Grounded;
                    TilelistToJumpState.TryGetValue(tileListList, out _jumpingState);


                    return new TileData(list[indexOfTile], _jumpingState, jumpEvents, xPos);
                }
            }
            jumpEvents++;
            indexReached += list.Count;
        }

        //Failure State
        Debug.LogError($"Tile Retrieval Failed. Flat Tile Returned at X Tile {xPos}!!");
        return (new TileData(GetFlatTile(), JumpingState.Grounded, 0, xPos));
    }


    Tile GetTile(List<Tile> tileList,int tileSeed)
    {
        int tileToGet = tileSeed % tileList.Count;

        Tile tileToPlace = (tileList[tileToGet]);
        return tileToPlace;

    }


    public TileData GetStepUpTile(int tileSeed, int xPos)
    {
        int i = 0;
        if (stepUpTiles.Count > 0)
        {
            i = (tileSeed % stepUpTiles.Count);
        }
        //produces a flat tile if no step up tiles are present
        else
        {
            return GetFlatTileData(xPos);
        }

        Tile tilePlaced = stepUpTiles[i];

        List<List<Tile>>[] availableTileLists = new List<List<Tile>>[] { SingleJumpTileLists, DoubleJumpTileLists, DashJumpTileLists };

        foreach (List<List<Tile>> tileListList in availableTileLists)
        {
            //number of jumping events in the tile placed
            int jumpingEvents = 1;
            foreach (List<Tile> tileGroup in tileListList)
            {
                if (tileGroup.Contains(tilePlaced))
                {
                    JumpingState _jumpingState = JumpingState.Grounded;
                    TilelistToJumpState.TryGetValue(tileListList, out _jumpingState);

                    return new TileData(tilePlaced, _jumpingState, jumpingEvents, xPos);
                }
                jumpingEvents++;
            }
        }

        //On failure
        Debug.LogError("Step up tile Retrieval failed. Tiles may not be placed properly and ideal jump numbers will not be reliable.");
        return new TileData(tilePlaced, JumpingState.Grounded, 0, xPos);
    }

    public TileData GetStepDownTile(int tileSeed, int xPos)
    {
        int i = (tileSeed % stepDownTiles.Count);
        return new TileData(stepDownTiles[i],JumpingState.Grounded,0,xPos);
    }

   public TileData GetDashJumpTile(int tileSeed, int xPos)
    {
        return GetTile(DashJumpTileLists, tileSeed,xPos);
    }
   public TileData GetFlatTileData(int xPos)
    {
        return new TileData(flatTile, JumpingState.Grounded, 0, xPos);
    }
    public Tile GetFlatTile()
    {
        return flatTile;
    }
   public TileData GetSingleJumpTile(int tileSeed, int xPos)
    {
        return GetTile(SingleJumpTileLists, tileSeed, xPos);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
