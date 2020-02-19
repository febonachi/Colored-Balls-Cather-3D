using System.Linq;
using UnityEngine;

public class ColorScheme : MonoBehaviour {
    #region editor
    public Color[] fogColors = default;
    public Color[] bulletColors = default;
    public Color[] bombBulletColors = default;
    public Color[] environmentBlocksColors = default;
    #endregion

    public static ColorScheme instance;

    #region public properties
    public Color randomColor => new Color(Random.value, Random.value, Random.value, 1f);
    public Color randomFogColor => fogColors[Random.Range(0, fogColors.Length)];
    public Color randomBulletColor => bulletColors[Random.Range(0, bulletColors.Length)];
    public Color randomBombBulletColor => bombBulletColors[Random.Range(0, bombBulletColors.Length)];
    public Color randomEnvironmentBlockColor => environmentBlocksColors[Random.Range(0, environmentBlocksColors.Length)];
    public Sprite randomBlobSprite => blobSprites[Random.Range(0, blobSprites.Length)];
    #endregion

    private Sprite[] blobSprites;

    #region private
    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        blobSprites = Resources.LoadAll("Sprites/Blobs", typeof(Sprite)).Cast<Sprite>().ToArray();
    }
    #endregion
}
