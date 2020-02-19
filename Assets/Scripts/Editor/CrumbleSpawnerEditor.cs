using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CrumbleSpawner))]
public class CrumbleSpawnerEditor : Editor {
    private CrumbleSpawner crumbleSpawner;

    private SerializedProperty poolID;
    private SerializedProperty prefabs;
    private SerializedProperty overlapMask;
    private SerializedProperty randomizeRotation;
    private SerializedProperty color;
    private SerializedProperty dimension;
    private SerializedProperty scale;
    private SerializedProperty spacing;
    private SerializedProperty explosionRadius;
    private SerializedProperty explosionForce;
    private SerializedProperty explosionUpForce;
    private SerializedProperty explosionForceMode;

    void OnEnable() {
        crumbleSpawner = target as CrumbleSpawner;

        poolID = serializedObject.FindProperty(nameof(poolID));
        prefabs = serializedObject.FindProperty(nameof(prefabs));
        overlapMask = serializedObject.FindProperty(nameof(overlapMask));
        randomizeRotation = serializedObject.FindProperty(nameof(randomizeRotation));
        color = serializedObject.FindProperty(nameof(color));
        dimension = serializedObject.FindProperty(nameof(dimension));
        scale = serializedObject.FindProperty(nameof(scale));
        spacing = serializedObject.FindProperty(nameof(spacing));
        explosionRadius = serializedObject.FindProperty(nameof(explosionRadius));
        explosionForce = serializedObject.FindProperty(nameof(explosionForce));
        explosionUpForce = serializedObject.FindProperty(nameof(explosionUpForce));
        explosionForceMode = serializedObject.FindProperty(nameof(explosionForceMode));
    }

    private void OnSceneGUI() {
        EditorGUI.BeginChangeCheck();
        Vector3 scale = Handles.ScaleHandle(crumbleSpawner.dimension, crumbleSpawner.transform.position, Quaternion.identity, 1f);
        if (EditorGUI.EndChangeCheck()){
            Undo.RecordObject(target, "Scaled crumble object dimension");
            crumbleSpawner.dimension = new Vector3Int((int)scale.x, (int)scale.y, (int)scale.z);
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(poolID, new GUIContent("poolID"));
        if(poolID.stringValue == "") EditorGUILayout.PropertyField(prefabs, new GUIContent("prefabs"), true);

        EditorGUILayout.PropertyField(overlapMask, new GUIContent("Overlap Mask"));
        EditorGUILayout.PropertyField(randomizeRotation, new GUIContent("Randomize rotation"));

        EditorGUILayout.PropertyField(color, new GUIContent("Color"));
        EditorGUILayout.PropertyField(dimension, new GUIContent("Dimension"));
        EditorGUILayout.PropertyField(scale, new GUIContent("Scale"));
        EditorGUILayout.Slider(spacing, 0f, 1f, new GUIContent("Spacing"));

        float maxScale = Mathf.Max(Mathf.Max(scale.vector3Value.x * dimension.vector3IntValue.x, 
                                             scale.vector3Value.y * dimension.vector3IntValue.y), 
                                             scale.vector3Value.z * dimension.vector3IntValue.z);
        EditorGUILayout.Slider(explosionRadius, 0f, maxScale * 10f, new GUIContent("ExplosionRadius"));
        EditorGUILayout.PropertyField(explosionForce, new GUIContent("ExplosionForce"));
        EditorGUILayout.Slider(explosionUpForce, 0f, 100f, new GUIContent("ExplosionUpForce"));
        EditorGUILayout.PropertyField(explosionForceMode, new GUIContent("ExplosionForceMode"));
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Initialize")) Array.ForEach(Selection.gameObjects, co => co.GetComponent<CrumbleSpawner>()?.initialize());
        if (GUILayout.Button("Clear")) Array.ForEach(Selection.gameObjects, co => co.GetComponent<CrumbleSpawner>()?.clear());
        if (GUILayout.Button("Explode")) Array.ForEach(Selection.gameObjects, co => co.GetComponent<CrumbleSpawner>()?.explode());
    }
}

