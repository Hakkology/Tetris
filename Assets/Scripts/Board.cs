using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    private Tilemap tilemap; // Bu tahtada kullanılacak Tilemap bileşeni
    public Piece activePiece; // Şu anda aktif olan Tetris parçası
    public TetrominoData[] tetrominos; // Mevcut tetrominoların veri dizisi
    public Vector3Int spawnPosition; // Yeni parçaların başlatılacağı konum

    // Başlangıçta çalışan metod
    public void Awake() {
        this.tilemap = GetComponentInChildren<Tilemap>(); // Tilemap bileşenini bul ve ata
        this.activePiece = GetComponentInChildren<Piece>(); // Aktif parça bileşenini bul ve ata
        // Tüm tetrominoları başlat
        for (int i = 0; i < tetrominos.Length; i++) {
            tetrominos[i].Initialize();
        }
    }

    // Oyun başladığında çalışan metod
    public void Start() {
        SpawnPiece(); // Yeni bir parça üret
    }

    // Yeni bir parça üretmek için kullanılan metod
    private void SpawnPiece() {
        int random = Random.Range(0, tetrominos.Length); // Rastgele bir tetromino seç
        TetrominoData data = this.tetrominos[random]; // Rastgele seçilen tetromino verisini al

        this.activePiece.Initialize(this, this.spawnPosition, data); // Aktif parçayı başlat
        Set(this.activePiece); // Parçayı tahtada yerleştir
    }

    // Bir parçanın tüm karelerini tahtada yerleştirmek için kullanılan metod
    public void Set(Piece piece) {
        // Parçanın her bir hücresini tahtaya yerleştir
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position; // Gerçek dünya pozisyonunu hesapla
            this.tilemap.SetTile(tilePosition, piece.data.tile); // Tilemap üzerinde belirtilen pozisyona tile'ı yerleştir
        }
    }
}
