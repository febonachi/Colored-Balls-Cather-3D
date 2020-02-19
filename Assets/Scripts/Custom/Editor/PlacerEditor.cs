using UnityEditor;
using PathCreation;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Placer))]
public class PlacerEditor : Editor{
    private Placer placer;
    
    private SerializedProperty pathCreator;
    private SerializedProperty placeByClosestPoint;
    private SerializedProperty time;
    private SerializedProperty ignoredAxis;
    private SerializedProperty lookAt;
    private SerializedProperty lookAtAxis;
    private SerializedProperty rotateTowards;

    // editor
    private bool placeByTime = false;

    private void OnEnable(){
        placer = target as Placer;
        
        pathCreator = serializedObject.FindProperty(nameof(pathCreator));
        placeByClosestPoint = serializedObject.FindProperty(nameof(placeByClosestPoint));
        time = serializedObject.FindProperty(nameof(time));
        ignoredAxis = serializedObject.FindProperty(nameof(ignoredAxis));
        rotateTowards = serializedObject.FindProperty(nameof(rotateTowards));
        lookAt = serializedObject.FindProperty(nameof(lookAt));
        lookAtAxis = serializedObject.FindProperty(nameof(lookAtAxis));
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        bool haveParent = placer.transform.parent != null;

        EditorGUILayout.PropertyField(pathCreator, new GUIContent("Path"));
        if (pathCreator.objectReferenceValue == null){
            if (haveParent){ // try to find PathCreator automatically
                PathCreator pathCreatorObject = placer.GetComponentInParent<PathCreator>();
                pathCreator.objectReferenceValue = pathCreatorObject;
            }
        }

        if (pathCreator.objectReferenceValue == null) return;

        EditorGUILayout.PropertyField(ignoredAxis, new GUIContent("Ignore axis"));

        placeByTime = EditorGUILayout.Toggle("Place by time", placeByTime);
        if(placeByTime){
            EditorGUI.indentLevel++;
            EditorGUILayout.Slider(time, 0f, 1f, new GUIContent("Time"));
            EditorGUI.indentLevel--;
        }

        placeByClosestPoint.boolValue = !placeByTime;

        EditorGUILayout.PropertyField(rotateTowards, new GUIContent("Rotate towards"));
        if (rotateTowards.enumValueIndex != (int)Follower.FollowerRotation.None) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lookAtAxis, new GUIContent("Look axis"));
            if (rotateTowards.enumValueIndex == (int)Follower.FollowerRotation.Transform) {
                EditorGUILayout.PropertyField(lookAt, new GUIContent("Look at target"));
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Place")) {
            foreach (GameObject item in Selection.gameObjects) item.GetComponent<Placer>()?.placeToPath();
        }

        if (GUILayout.Button("Rotate")) {
            foreach (GameObject item in Selection.gameObjects) item.GetComponent<Placer>()?.rotateTransform();
        }
    }
}
