using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreator : MonoBehaviour
{

    public Tilemap level;


    public List<Tile> singleJumpTiles;
    public List<Tile> stepUpTiles;
    public List<Tile> stepDownTiles;
    public List<Tile> dashJumpTiles;
    public Tile flatTile;
    private int[] _seed;
    private int heightOfNextTile;

    //Please enter a 7 digit seed
    public int seed = 1234567;

    /// <summary>
    /// Number of tiles in the x direction
    /// </summary>
    public int LengthOfLevel = 20;

    // Start is called before the first frame update
    void Start()
    {
        SeedToArray();
        level = GameObject.FindObjectOfType<Tilemap>();
        level.SetTile(new Vector3Int(0, 0, 0), singleJumpTiles[0]);
        ProduceLevel();
    }

    private void SeedToArray()
    {
        List<int> seedList = new List<int>();
        string digits = seed.ToString();
        foreach (int d in digits)
        {
            seedList.Add(d);
        }
        _seed = seedList.ToArray();
    }

    void ProduceLevel()
    {
        int[] seedi = _seed;
        int[] seedj = _seed;

        int tileCounter = 0;
        foreach (int i in seedi)
        {
            foreach (int j in seedj)
            {
                if (tileCounter < LengthOfLevel)
                {
                    //Creates a 1/2 digit int which is used to determine type and specific tile placed.
                    int localTileSeed = (i * j) + (i + j);
                    Tile tilePlaced =
                    switch (localTileSeed % 3)
                    {
                        case 0:
                            GetSingleJumpTile(localTileSeed);
                            break;

                            //Step up or down tile. Step down can only be placed when nextTileHeight is above 0
                        case 1:
                            GetStepUpOrDown(localTileSeed);
                            break;
                        case 2:
                            GetDashJumpTile(localTileSeed);
                            break;


                        default:
                            GetFlatTile(localTileSeed);
                            break;
                    }
                    tileCounter++;
                }
            }
        }


    }

    void _PlaceTile(Tile toPlace, int posx, int posy)
    {
        level.SetTile(new Vector3Int(posx, posy, 0), toPlace);
    }

    // Places a tile from the single Jump Array
    void PlaceSingleJumpTile(int xpos, int tileSeed)
    {
        Tile tile = GetSingleJumpTile(tileSeed);

        _PlaceTile(tile, xpos, heightOfNextTile);
    }
    //Places a tile from the StepUpArray
    void PlaceStepUpOrDownTile(int xpos, int tileSeed)
    {
        if

    }




    Tile GetStepUpOrDown(int tileSeed)
    {
        return GetStepUpTile();
    }

    Tile GetStepUpTile(int tileSeed)
    { 
        
        return stepUpTiles[0];
    }

    Tile GetDashJumpTile(int tileSeed)
    {
        return dashJumpTiles[0];
    }
    Tile GetFlatTile(int tileSeed)
    {
        return flatTile;
    }
    Tile GetSingleJumpTile(int tileSeed)
    {
        return singleJumpTiles[0];
    }

    void PlaceStartAndEndFlag()
    {
        PlaceStartFlag();
        PlaceEndFlag();
    }

    private void PlaceEndFlag()
    {
        float x_pos = level.localBounds.max.x;
        Vector3 flagPosition = level.GetCellCenterLocal(new Vector3Int(LengthOfLevel, 0, 0));
        flagPosition.x = x_pos;
    }

    private void PlaceStartFlag()
    {
        float x_pos = level.localBounds.min.x;
        Vector3 flagPosition = level.GetCellCenterLocal(new Vector3Int(0, 0, 0));
        flagPosition.x = x_pos;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
