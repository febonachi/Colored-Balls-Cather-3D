using Utils;
using UnityEditor;
using UnityEngine;
using PathCreation;

[CanEditMultipleObjects]
[CustomEditor(typeof(Rotator))]
public class RotatorEditor : Editor {
    private SerializedProperty endInstruction;
    private SerializedProperty space;
    private SerializedProperty rotateAround;
    private SerializedProperty lookAt;
    private SerializedProperty ignoredAxis;
    private SerializedProperty maxAngle;
    private SerializedProperty forwardSpeed;
    private SerializedProperty backwardSpeed;
    private SerializedProperty startDelay;
    private SerializedProperty stopDelay;
    private SerializedProperty destroyOnStop;
    private SerializedProperty unscaledDeltaTime;

    private void OnEnable() {
        endInstruction = serializedObject.FindProperty(nameof(endInstruction));
        space = serializedObject.FindProperty(nameof(space));
        rotateAround = serializedObject.FindProperty(nameof(rotateAround));
        lookAt = serializedObject.FindProperty(nameof(lookAt));
        ignoredAxis = serializedObject.FindProperty(nameof(ignoredAxis));
        maxAngle = serializedObject.FindProperty(nameof(maxAngle));
        forwardSpeed = serializedObject.FindProperty(nameof(forwardSpeed));
        backwardSpeed = serializedObject.FindProperty(nameof(backwardSpeed));
        startDelay = serializedObject.FindProperty(nameof(startDelay));
        stopDelay = serializedObject.FindProperty(nameof(stopDelay));
        destroyOnStop = serializedObject.FindProperty(nameof(destroyOnStop));
        unscaledDeltaTime = serializedObject.FindProperty(nameof(unscaledDeltaTime));
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(endInstruction, new GUIContent("End instruction"));
        bool stop = endInstruction.enumValueIndex == (int)PathCreation.EndOfPathInstruction.Stop;
        bool reverse = endInstruction.enumValueIndex == (int)PathCreation.EndOfPathInstruction.Reverse;

        EditorGUILayout.PropertyField(space, new GUIContent("Space"));

        if(space.enumValueIndex == (int)Rotator.RotationSpace.Transform){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lookAt, new GUIContent("Look at"));
            EditorGUILayout.PropertyField(rotateAround, new GUIContent("Rotate around"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(ignoredAxis, new GUIContent("Ignore axis"));

        if(reverse || stop) EditorGUILayout.PropertyField(maxAngle, new GUIContent("Max angle"));

        EditorGUILayout.PropertyField(forwardSpeed, new GUIContent("Forward speed"));
        if(reverse){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(backwardSpeed, new GUIContent("Backward speed"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(startDelay, new GUIContent("Start delay"));
        EditorGUILayout.PropertyField(stopDelay, new GUIContent("Stop delay"));
        EditorGUILayout.PropertyField(destroyOnStop, new GUIContent("Destroy on stop"));
        EditorGUILayout.PropertyField(unscaledDeltaTime, new GUIContent("Unscaled delta time"));

        serializedObject.ApplyModifiedProperties();
    }
}
