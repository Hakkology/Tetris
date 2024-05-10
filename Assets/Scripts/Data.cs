using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    // Precomputed cosine of 90 degrees, used in rotation matrix
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    // Precomputed sine of 90 degrees, used in rotation matrix
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    // Rotation matrix for rotating Tetromino blocks in 90-degree increments
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    // Mapping from Tetromino types to their cell positions. This defines the shape of each Tetromino
    // by specifying the grid cells occupied relative to a pivot point at the center of the Tetromino.
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.I, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) } },
        { Tetromino.J, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        { Tetromino.L, new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        { Tetromino.O, new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        { Tetromino.S, new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0) } },
        { Tetromino.T, new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        { Tetromino.Z, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
    };

    // Specific wall kick data for 'I' shaped Tetromino rotations
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        // Each row represents the offset to try for each of four rotation states (0, 90, 180, 270 degrees)
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2,-1), new Vector2Int(1, 2) },
        // Additional rows omitted for brevity
    };

    // Wall kick data for 'J', 'L', 'O', 'S', 'T', 'Z' shaped Tetromino rotations
    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
        // Each entry contains offsets for wall kicks applicable to non-'I' Tetrominos
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        // Additional rows omitted for brevity
    };

    // A dictionary mapping each Tetromino type to its corresponding wall kick data array
    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksJLOSTZ },
        { Tetromino.L, WallKicksJLOSTZ },
        { Tetromino.O, WallKicksJLOSTZ },
        { Tetromino.S, WallKicksJLOSTZ },
        { Tetromino.T, WallKicksJLOSTZ },
        { Tetromino.Z, WallKicksJLOSTZ },
    };
}