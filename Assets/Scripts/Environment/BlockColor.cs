using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BlockColor : MonoBehaviour {
    #region editor
    [SerializeField] private MeshRenderer meshRenderer = default;
    [SerializeField] private bool updateColorOnStart = true;
    [SerializeField] private bool randomizeColor = false;
    [SerializeField] private Color defaultColor = Color.white;
    #endregion

    #region public properties
    public MeshRenderer mesh => meshRenderer;
    #endregion

    private ColorScheme colorScheme;

    #region private
    private void Awake() {
        meshRenderer = meshRenderer ?? GetComponent<MeshRenderer>();
        if (updateColorOnStart) meshRenderer.material.color = defaultColor;
    }

    private void Start() {
        colorScheme = ColorScheme.instance;

        if (randomizeColor) meshRenderer.material.color = colorScheme.randomEnvironmentBlockColor;
    }
    #endregion
}
