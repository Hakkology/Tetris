using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    private Tilemap tilemap;
    public Piece activePiece;
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;

    public void Awake(){

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    public void Start(){
        SpawnPiece();
    }

    private void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];

        this.activePiece.Initialize(this, this.spawnPosition, data);
        Set(this.activePiece);
    }

    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
        
    }
}
