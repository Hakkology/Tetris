using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board; // Bu parçanın ait olduğu oyun tahtası
    public TetrominoData data; // Tetromino'nun veri yapısı
    public Vector3Int position; // Parçanın tahtadaki konumu
    public Vector3Int[] cells; // Parçanın kafes içindeki hücre pozisyonları

    // Parçayı başlatmak için kullanılan metod
    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board; // Tahta referansını ayarla
        this.position = position; // Başlangıç pozisyonunu ayarla
        this.data = data; // Tetromino verisini ayarla

        // Hücre dizisini başlat veya boyutunu ayarla
        if (this.cells == null || this.cells.Length != data.cells.Length) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        // Veri modelinden hücre konumlarını kopyala
        for (int i = 0; i < data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
}