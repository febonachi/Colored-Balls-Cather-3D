using Utils;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Follower))]
public class FollowerEditor : Editor{
    private Placer placer;
    private Follower follower;

    private SerializedProperty endInstruction;
    private SerializedProperty forwardSpeed;
    private SerializedProperty forwardSpeedCurve;
    private SerializedProperty backwardSpeed;
    private SerializedProperty backwardSpeedCurve;
    private SerializedProperty startDelay;
    private SerializedProperty stopDelay;
    private SerializedProperty lookAt;
    private SerializedProperty lookAtAxis;
    private SerializedProperty rotateTowards;
    private SerializedProperty destroyOnStop;
    private SerializedProperty unscaledDeltaTime;

    private void OnEnable(){
        follower = target as Follower;
        placer = follower.GetComponent<Placer>();
        
        endInstruction = serializedObject.FindProperty(nameof(endInstruction));
        forwardSpeed = serializedObject.FindProperty(nameof(forwardSpeed));
        forwardSpeedCurve = serializedObject.FindProperty(nameof(forwardSpeedCurve));
        backwardSpeed = serializedObject.FindProperty(nameof(backwardSpeed));
        backwardSpeedCurve = serializedObject.FindProperty(nameof(backwardSpeedCurve));
        startDelay = serializedObject.FindProperty(nameof(startDelay));
        stopDelay = serializedObject.FindProperty(nameof(stopDelay));
        rotateTowards = serializedObject.FindProperty(nameof(rotateTowards));
        lookAt = serializedObject.FindProperty(nameof(lookAt));
        lookAtAxis = serializedObject.FindProperty(nameof(lookAtAxis));
        destroyOnStop = serializedObject.FindProperty(nameof(destroyOnStop));
        unscaledDeltaTime = serializedObject.FindProperty(nameof(unscaledDeltaTime));
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        if (placer.pathCreator == null) return;

        EditorGUILayout.PropertyField(endInstruction, new GUIContent("End instruction"));
        EditorGUILayout.PropertyField(forwardSpeed, new GUIContent("Forward speed"));
        EditorGUILayout.PropertyField(forwardSpeedCurve, new GUIContent("Forward curve"));
        
        if(endInstruction.enumValueIndex == (int)PathCreation.EndOfPathInstruction.Reverse){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(backwardSpeed, new GUIContent("Backward speed"));
            EditorGUILayout.PropertyField(backwardSpeedCurve, new GUIContent("Backward curve"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(startDelay, new GUIContent("Start delay"));
        EditorGUILayout.PropertyField(stopDelay, new GUIContent("Stop delay"));

        EditorGUILayout.PropertyField(rotateTowards, new GUIContent("Rotate towards"));
        if(rotateTowards.enumValueIndex != (int)Follower.FollowerRotation.None){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lookAtAxis, new GUIContent("Look axis"));
            if(rotateTowards.enumValueIndex == (int)Follower.FollowerRotation.Transform) {
                EditorGUILayout.PropertyField(lookAt, new GUIContent("Look at target"));
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(destroyOnStop, new GUIContent("Destroy on stop"));
        EditorGUILayout.PropertyField(unscaledDeltaTime, new GUIContent("Unscaled delta time"));

        serializedObject.ApplyModifiedProperties();
    }
}
