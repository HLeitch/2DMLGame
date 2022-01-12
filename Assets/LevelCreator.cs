using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class LevelCreator : MonoBehaviour
{

    public Tilemap level;
    public List<TileData> tileDatas = new List<TileData>();
    public TileManager tileManager;

    public LevelManager levelManager;
/*    public List<Tile> singleJumpTiles;
    public List<Tile> stepUpTiles;
    public List<Tile> stepDownTiles;
    public List<Tile> dashJumpTiles;
    public Tile flatTile;*/
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
       // GenerateLevel();
    }

    public void GenerateLevel()
    {
        _seed = SeedToArray();

        level = GameObject.FindObjectOfType<Tilemap>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();

        level.ClearAllTiles();

        startFlag = levelManager.startFlag;
        endFlag = levelManager.endFlag;


        ProduceLevel();
        PlaceStartAndEndFlag();
        string numberOfJumps = "";
        foreach (TileData t in tileDatas)
        {
            numberOfJumps += $"{t.numberOfJumps}, ";
        }
        Debug.Log($"Number Of Jumps In each tile: {numberOfJumps}");
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
                        int typeSeed = i + j;
                        _tilesPlaced.Add(currentTileSeed);
                        switch (typeSeed % 3)
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
    private void PlaceSingleJumpTile(int xPos, int tileSeed)
    {
        TileData tileData = tileManager.GetSingleJumpTile(tileSeed, xPos);
        tileDatas.Add(tileData);

        Tile tileToPlace = tileData.tile;
        _PlaceTile(tileToPlace, xPos, heightOfNextTile);
    }
    //Places a tile from the StepUpArray
    void PlaceStepUpTile(int xPos, int tileSeed)
    {
        TileData tileData = tileManager.GetStepUpTile(tileSeed, xPos);
        tileDatas.Add(tileData);

        Tile tileToPlace = tileData.tile;
        _PlaceTile(tileToPlace, xPos, heightOfNextTile);
        int ypos = heightOfNextTile++;
    }

    //Places a tile from the StepDownArray
    void PlaceStepDownTile(int xPos, int tileSeed)
    {
        TileData tileData = tileManager.GetStepDownTile(tileSeed, xPos);
        tileDatas.Add(tileData);
        int ypos = --heightOfNextTile;
        Tile tileToPlace = tileData.tile;
        _PlaceTile(tileToPlace, xPos, heightOfNextTile);
    }

    //places a dash jump tile from DashJumpTileArray
    void PlaceDashJumpTile(int xPos, int tileSeed)
    {
        TileData tileData = tileManager.GetDashJumpTile(tileSeed, xPos);
        tileDatas.Add(tileData);

        Tile tileToPlace = tileData.tile;
        _PlaceTile(tileToPlace, xPos, heightOfNextTile);
    }
    //Places the flat tile
    void PlaceFlatTile(int xpos)
    {
        TileData td = tileManager.GetFlatTileData(xpos);
        tileDatas.Add(td);
        Tile flat = td.tile;
        _PlaceTile(flat, xpos, heightOfNextTile);
    }
/*    Tile GetStepUpTile(int tileSeed)
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
*/
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
        endFlag.transform.localPosition = flagPosition;
    }

    private void PlaceStartFlag()
    {
        float x_pos = level.localBounds.min.x;
        Vector3 flagPosition = level.GetCellCenterLocal(new Vector3Int(0, 0, 0));
        //flagPosition.x = x_pos;

        startFlag.transform.localPosition = flagPosition;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
