using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board; // Bu parçanın ait olduğu oyun tahtası
    public TetrominoData data; // Tetromino'nun veri yapısı
    public Vector3Int position; // Parçanın tahtadaki konumu
    public Vector3Int[] cells; // Parçanın kafes içindeki hücre pozisyonları
    public int rotationIndex; // mevcut dönüm endeksi.

    // Parçayı başlatmak için kullanılan metod
    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board; // Tahta referansını ayarla
        this.position = position; // Başlangıç pozisyonunu ayarla
        this.data = data; // Tetromino verisini ayarla
        this.rotationIndex = 0; // ilk indexi 0 olarak ayarla.

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

        // Son olarak yeni konumu board a gönder.
        this.board.Set(this);
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
        }

        return valid;
    }

    void Fall()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }

    // private void Rotate(int direction)
    // {
    //     // Geçici bir değişken oluştur ve rotationIndex ile direction'u topla.
    //     int newRotationIndex = this.rotationIndex + direction;

    //     // Yeni indexi wraple ve ardından rotationIndex'e ata.
    //     this.rotationIndex = (newRotationIndex + 4) % 4;

    //     // Bir parçanın tüm hücrelerine bunu uygulayacağız.
    //     // Ana datayı değiştirmek istemiyoruz, kopyası olarak oluşturduğumuz cells olan hücreyi değiştireceğiz. 
    //     // Bu sayede her spawn olduğunda data kalmaya devam edecek.
    //     for (int i = 0; i < this.cells.Length; i++)
    //     {
    //         // Burada vector3Int kullanamam çünkü yarım birim değiştireceğimiz parçalar var. SRSe bakalım.
    //         Vector3 cell = this.cells[i];
    //         int x,y;

    //         switch (this.data.tetromino)
    //         {
    //             case Tetromino.I:
    //             case Tetromino.O:

    //                 // Önce yarısını alıp sonra 90 derece çevirmeliyiz.
    //                 cell.x -= .5f;
    //                 cell.y -= .5f;
    //                 x =  Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y* Data.RotationMatrix[1] * direction));
    //                 y =  Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y* Data.RotationMatrix[3] * direction));
    //                 break;
    //             default:

    //                 // Tamamen 90 derece çevirmeliyiz.
    //                 x =  Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y* Data.RotationMatrix[1] * direction));
    //                 y =  Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y* Data.RotationMatrix[3] * direction));
    //                 break;
    //         }

    //         this.cells[i] = new Vector3Int(x, y, 0);
    //     }
    // }
    
    private void Rotate(int direction)
    {
        // Eğer döndürme yönü 0 ise (yani hiçbir değişiklik yapma).
        if (direction == 0)
            return;

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
}