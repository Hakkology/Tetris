using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board; // Bu parçanın ait olduğu oyun tahtası
    public TetrominoData data; // Tetromino'nun veri yapısı
    public Vector3Int position; // Parçanın tahtadaki konumu
    public Vector3Int[] cells; // Parçanın kafes içindeki hücre pozisyonları
    public int rotationIndex; // mevcut dönüm endeksi.

    public float stepDelay = 1f;
    public float lockDelay = .5f;


    private float stepTime;
    private float lockTime;

    // Parçayı başlatmak için kullanılan metod
    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board; // Tahta referansını ayarla
        this.position = position; // Başlangıç pozisyonunu ayarla
        this.data = data; // Tetromino verisini ayarla
        this.rotationIndex = 0; // ilk indexi 0 olarak ayarla.

        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        // Hücre dizisini başlat veya boyutunu ayarla
        if (this.cells == null || this.cells.Length != data.cells.Length) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        // Veri modelinden hücre konumlarını kopyala
        for (int i = 0; i < data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    void Update()
    {
        // İlk olarak bulunduğu noktayı temizle.
        this.board.Clear(this);
        // A ya basınca sola, D ye basınca sağa gideceğiz. Hareket fonksiyonunu çalıştır.

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            Rotate(+1);
        }

        // Yumuşak iniş için gerekli olan input 
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fall();
        }

        if (Time.time >= this.stepTime)
        {
            Step();
        }

        // Son olarak yeni konumu board a gönder.
        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    bool Move(Vector2Int translation)
    {
        // Hareket etme fonksiyonunun problemi, öncelikle gittiği noktanın doğru olup olmadığını kontrol etmeliyiz.
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        // Burada board ile iletişime geçip gittiğimiz yerin boardun sınırlarının içinde olup olmadığını kontrol etmeliyiz.
        bool valid =  this.board.isPositionValid(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
            this.lockTime = 0;
        }

        return valid;
    }

    void Fall()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Rotate(int direction)
    {
        // Eğer döndürme yönü 0 ise (yani hiçbir değişiklik yapma).
        if (direction == 0)
            return;
        // İlk endeksi kaydet.
        int initialRotationIndex = this.rotationIndex;

        ApplyRotation(direction);

        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = initialRotationIndex;
            ApplyRotation(-direction);
        }

    }

    void ApplyRotation(int direction)
    {
        // Dönüş açısını hesapla
        float angle = direction * 90;

        switch (this.data.tetromino)
        {
            case Tetromino.O:
                // 'O' Tetromino için hiçbir dönüş yapma
                break;

            case Tetromino.I:
                // 'I' Tetromino için özel dönüş işlemi
                RotateITetromino(angle);
                break;

            default:
                // Diğer tüm Tetrominolar için genel dönüş işlemi
                RotateGeneral(angle);
                break;
        }
    }
    void RotateITetromino(float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        for (int i = 0; i < this.cells.Length; i++)
        {
            // Grid düzeltmesi öncesi ve sonrası için düzeltilmiş pozisyonlar
            Vector3 cellPosition = this.cells[i] - new Vector3(0.5f, 0.5f, 0); // Grid düzeltilmesi: Önce 0.5 birim çıkar
            cellPosition = rotation * cellPosition; // Dönüş uygula
            cellPosition += new Vector3(0.5f, 0.5f, 0); // Döndürüldükten sonra 0.5 birim ekle

            // Dönüş sonrası hücre pozisyonunu güncelle
            this.cells[i] = new Vector3Int(Mathf.RoundToInt(cellPosition.x), Mathf.RoundToInt(cellPosition.y), 0);
        }
    }

    void RotateGeneral(float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cellPosition = this.cells[i];
            cellPosition = rotation * cellPosition;
            this.cells[i] = new Vector3Int(Mathf.RoundToInt(cellPosition.x), Mathf.RoundToInt(cellPosition.y), 0);
        }
    }


    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < 5; i++) // Her test dizisi için 5 farklı test var
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation)) {
                return true; // Eğer taşınma başarılı ise, true dön
            }
        }
        return false; // Eğer hiçbir taşınma başarılı olmadıysa, false dön
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int baseIndex = rotationIndex * 2; // Her rotasyon durumu için iki ayrı dizin ayır
        if (rotationDirection > 0) {
            // Saat yönünde dönüş, 'R' dizini
            return baseIndex % 8; // Modulo 8 kullanarak sınırları belirle
        } else {
            // Saat yönünün tersine dönüş, 'L' dizini
            return (baseIndex + 1) % 8; // Modulo 8 kullanarak sınırları belirle
        }
    }
}