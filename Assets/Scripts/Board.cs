using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    private Tilemap tilemap; // Bu tahtada kullanılacak Tilemap bileşeni
    public Piece activePiece; // Şu anda aktif olan Tetris parçası
    public TetrominoData[] tetrominos; // Mevcut tetrominoların veri dizisi
    public Vector3Int spawnPosition; // Yeni parçaların başlatılacağı konum, değiştirerek yukarı alabiliriz. (-1, 8) 10 en yukarısı, 8 e çekmeliyiz.
    public int boardWidth = 10;
    public int boardHeight = 20;

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
        // Rastgele bir tetromino seçerek onu spawn edeceğiz.
        int random = Random.Range(0, tetrominos.Length); // Rastgele bir tetromino seç
        TetrominoData data = this.tetrominos[random]; // Rastgele seçilen tetromino verisini al

        // Seçilen parçayı aktif hale getirmemiz gerekiyor.
        // spawn position tetris için genelde en üstte ve bir adım sağda olur. 
        // Spawn position üzerinden tüm hücreleri hareket ettiriyoruz.
        // Sonra da buna göre tileı değiştiryoruz.
        this.activePiece.Initialize(this, this.spawnPosition, data); // Aktif parçayı başlat
        Set(this.activePiece); // Parçayı tahtada yerleştir
    }

    // Bir parçanın tüm karelerini tahtada yerleştirmek için kullanılan metod
    public void Set(Piece piece) {
        // Parçanın her bir hücresini tahtaya yerleştir
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position; // Gerçek dünya pozisyonunu hesapla
            // Burada tek yapılması gereken o tileın daha önce belirlenmiş tile lar olduğunu board a bildirmek.
            this.tilemap.SetTile(tilePosition, piece.data.tile); // Tilemap üzerinde belirtilen pozisyona tile'ı yerleştir
        }
    }

    public void Clear(Piece piece) {
        // Parçanın her bir hücresini tahtadan sil.
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position; // Gerçek dünya pozisyonunu hesapla
            // Burada tek yapılması gereken o tileın daha önce belirlenmiş tile lar olduğunu board a bildirmek.
            this.tilemap.SetTile(tilePosition, null); // Tilemap üzerinde belirtilen pozisyonu temizle.
        }
    }

    // Parçanın hareket ettiği yer bizim için doğru mu ?
    public bool isPositionValid(Piece piece, Vector3Int position)
    {
        // Bunun için tüm hücreleri kontrol etmem gerekiyor.
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // Burada başka tile var mı ?
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
            // Borderların dışında kalıyor mu ? Ama önce borderları tanımlamak durumundayız.
            if (!IsInBorders(tilePosition))
            {
                return false; // Sınırların dışındaysa, geçersiz konum.
            }
        }
        return true;
    }

        // Sınır kontrolü için kullanılan fonksiyon
    private bool IsInBorders(Vector3Int position)
    {
        // X koordinatının sınırlar içinde olup olmadığını kontrol ediyoruz.
        // Burada boardWidth / 2 kullanarak merkezi baz alıyoruz.
        bool withinXBounds = position.x >= -boardWidth / 2 && position.x < boardWidth / 2;

        // Y koordinatının sınırlar içinde olup olmadığını kontrol ediyoruz.
        // Burada en üst sınırı doğrudan boardHeight olarak alıyoruz.
        bool withinYBounds = position.y >= -boardHeight / 2 && position.y < boardHeight / 2;

        return withinXBounds && withinYBounds;
    }
}
