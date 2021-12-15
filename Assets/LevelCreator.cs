using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreator : MonoBehaviour
{

    public Tilemap level;

    public LevelManager levelManager;
    public List<Tile> singleJumpTiles;
    public List<Tile> stepUpTiles;
    public List<Tile> stepDownTiles;
    public List<Tile> dashJumpTiles;
    public Tile flatTile;
    private int[] _seed;
    private int heightOfNextTile;

    public GameObject startFlag;
    public GameObject endFlag;


    //Please enter a 7 digit seed
    public int seed = 1234567;

    /// <summary>
    /// Number of tiles in the x direction
    /// </summary>
    public int LengthOfLevel = 20;

    // Start is called before the first frame update
    void Start()
    {
        _seed = SeedToArray();
        
        level = GameObject.FindObjectOfType<Tilemap>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();

        startFlag = levelManager.startFlag;
        endFlag = levelManager.endFlag;


        ProduceLevel();
        PlaceStartAndEndFlag();
    }

    //Convert Seed to an array
    private int[] SeedToArray()
    {
        //convert seed to string
        List<int> seedList = new List<int>();
        string digits = seed.ToString();

        Debug.Log(digits);
        //each character is added to the seedList
        foreach (char d in digits)
        {
            int n = int.Parse(d.ToString());
            seedList.Add(n);
            Debug.Log(d);
        }
        //seedlist is converted to an array for output.
        return seedList.ToArray();
    }

    void ProduceLevel()
    {
        int[] seedi = _seed;
        int[] seedj = _seed;

        List<int> _tilesPlaced = new List<int>();
        int tileCounter = 0;
        foreach (int i in seedi)
        {
            foreach (int j in seedj)
            {

                //start and end of levels are flat tiles
                if (tileCounter < LengthOfLevel)
                {
                    if (tileCounter == 0 || tileCounter == LengthOfLevel - 1)
                    {
                        PlaceFlatTile(tileCounter);
                    }
                    else
                    {

                        //Creates a 1/2 digit int which is used to determine type and specific tile placed.
                        int currentTileSeed = (i * j) + (i + j);
                        _tilesPlaced.Add(currentTileSeed);
                        switch (currentTileSeed % 3)
                        {
                            case 0:
                                PlaceSingleJumpTile(tileCounter, currentTileSeed);
                                break;

                            //Step up or down tile. Step down can only be placed when nextTileHeight is above 0
                            case 1:
                                float stepDownChance = (0.33333333333333f) * heightOfNextTile;
                                float roll = ((float)seedi[6]) / 10f;
                                if (roll < stepDownChance)
                                {
                                    PlaceStepDownTile(tileCounter, currentTileSeed);
                                }
                                else
                                {
                                    PlaceStepUpTile(tileCounter, currentTileSeed);
                                }


                                break;
                            case 2:
                                PlaceDashJumpTile(tileCounter, currentTileSeed);
                                break;


                            default:
                                PlaceFlatTile(tileCounter);
                                break;
                        }
                    }
                    tileCounter++;
                }
            }
        }
        Debug.Log("TILES = " + string.Join(",", _tilesPlaced));

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
    void PlaceStepUpTile(int xpos, int tileSeed)
    {
        Tile tile = GetStepUpTile(tileSeed);
        //return the current ypos and then increment up
        int ypos = heightOfNextTile++;
        _PlaceTile(tile, xpos, ypos);
    }

    //Places a tile from the StepDownArray
    void PlaceStepDownTile(int xpos, int tileSeed)
    {
        Tile tile = GetStepDownTile(tileSeed);
        //decrement ypos and use this as the height for the tile
        int ypos = --heightOfNextTile;
        _PlaceTile(tile, xpos, ypos);
    }

    //places a dash jump tile from DashJumpTileArray
    void PlaceDashJumpTile(int xpos, int tileSeed)
    {
        Tile tile = GetDashJumpTile(tileSeed);
        _PlaceTile(tile, xpos, heightOfNextTile);
    }
    //Places the flat tile
    void PlaceFlatTile(int xpos)
    {
        _PlaceTile(GetFlatTile(), xpos, heightOfNextTile);
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
        endFlag.transform.position = flagPosition;
    }

    private void PlaceStartFlag()
    {
        float x_pos = level.localBounds.min.x;
        Vector3 flagPosition = level.GetCellCenterLocal(new Vector3Int(0, 0, 0));
        flagPosition.x = x_pos;

        startFlag.transform.position = flagPosition;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
