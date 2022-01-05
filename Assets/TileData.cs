using UnityEngine.Tilemaps;

public struct TileData
{
    public TileData(Tile tile, JumpingState jumpType,int numberOfJumps,int xPosition)
    {
        this.tile = tile;
        this.jumpType = jumpType;
        this.numberOfJumps = numberOfJumps;
        this.xPosition = xPosition;
    }

    public Tile tile;
    /// <summary>
    /// Type of jump required to complete
    /// </summary>
    public JumpingState jumpType;
    /// <summary>
    /// jumps expected to complete
    /// </summary>
    public int numberOfJumps;
    /// <summary>
    /// position in tilemap
    /// </summary>
    public int xPosition;
}
