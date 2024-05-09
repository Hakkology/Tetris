using UnityEngine;
public class Piece: MonoBehaviour
{
    public Board board;
    public TetrominoData data;
    public Vector3Int position;
    public Vector3Int[] cells;
    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells.Length != data.cells.Length) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        Debug.Log("Initializing cells with length: " + data.cells.Length);

        for (int i = 0; i < data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
}