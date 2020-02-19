using UnityEngine;
using UnityEditor;

using Assets.Scripts.Water;

[CustomEditor(typeof(WaterPropertyBlockSetter))]
public class PathControllerEditor : Editor {
	private WaterPropertyBlockSetter waterSetter;

    void OnEnable() => waterSetter = target as WaterPropertyBlockSetter;
		
	public override void OnInspectorGUI() {
        DrawDefaultInspector();

		if (GUILayout.Button("Find Blocks")) waterSetter.findBlocks();

            
    }
}
