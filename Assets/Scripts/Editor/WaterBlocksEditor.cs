using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Utils.Water))]
public class Water : Editor {
	private Utils.Water water;

    void OnEnable() => water = target as Utils.Water;
		
	public override void OnInspectorGUI() {
        DrawDefaultInspector();

		if (GUILayout.Button("Generate Blocks")) water.generateBlocks();
        if (GUILayout.Button("Clear Blocks")) water.clearBlocks();
    }
}
