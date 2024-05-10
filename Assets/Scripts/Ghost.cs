using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap;
    public Vector3Int[] cells;
    public Vector3Int position;

    void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Late update olmasının sebebi önce piece de bulunan bilgilerin işlenmesi gerekiyor.
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Copy()
    {
        // Bu metot, izlenen (tracking) parçanın hücre pozisyonlarını kopyalar.
        // Ana parçanın (Piece) mevcut hücre konfigürasyonunu kopyalar ve ghost parçasının hücrelerine aktarır.
        // Böylece, ghost parçası ana parçanın şu anki durumunun bir yansıması haline gelir.
        for (int i = 0; i < this.cells.Length; i++) {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }
    private void Drop()
    {
        // Drop metodu, ghost parçasını mümkün olan en düşük y konumuna düşürür.
        // Ana parça (Piece) izlenir ve ghost, tahtada daha aşağıya inemeyecek şekilde aşağı doğru "düşer".
        Vector3Int position = this.trackingPiece.position;  // Başlangıç pozisyonunu izlenen parçadan alır.

        int current = position.y;  // Şu anki yükseklik.
        int bottom = -this.board.boardHeight / 2 - 1;  // Tahtanın en alt sınırı.

        this.board.Clear(this.trackingPiece);  // Ana parçanın tahtadaki hücreleri geçici olarak temizlenir.

        // Yükseklikten tahtanın en altına kadar olan her y pozisyonu için kontrol yapar.
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            // Eğer bu yeni y pozisyonu geçerliyse, ghost parçasının yeni pozisyonunu günceller.
            if (this.board.isPositionValid(this.trackingPiece, position))
            {
                this.position = position;
            }
            else
            {
                break;  // Eğer pozisyon geçerli değilse, döngüyü kırar ve en son geçerli pozisyonda kalır.
            }
        }

        this.board.Set(this.trackingPiece);  // Ana parçanın tahtadaki hücreleri tekrar yerleştirilir.
    }
    public void Set() {
        // Parçanın her bir hücresini tahtaya yerleştir
        for (int i = 0; i < this.cells.Length; i++) {
            Vector3Int tilePosition = this.cells[i] + this.position; // Gerçek dünya pozisyonunu hesapla
            // Burada tek yapılması gereken o tileın daha önce belirlenmiş tile lar olduğunu board a bildirmek.
            this.tilemap.SetTile(tilePosition, this.tile); // Tilemap üzerinde belirtilen pozisyona tile'ı yerleştir
        }
    }

    public void Clear() {
        // Parçanın her bir hücresini tahtadan sil.
        for (int i = 0; i < this.cells.Length; i++) {
            Vector3Int tilePosition = this.cells[i] + this.position; // Gerçek dünya pozisyonunu hesapla
            // Burada tek yapılması gereken o tileın daha önce belirlenmiş tile lar olduğunu board a bildirmek.
            this.tilemap.SetTile(tilePosition, null); // Tilemap üzerinde belirtilen pozisyonu temizle.
        }
    }
}
