using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Ricochet3D_Prototype1 {
	[CustomEditor(typeof(PathController))]
	public class PathControllerEditor : Editor {
		private PathController pathController;

		private SerializedProperty offset;
		private SerializedProperty direction;

        void OnEnable() {
			pathController = target as PathController;

            offset = serializedObject.FindProperty(nameof(offset));
			direction = serializedObject.FindProperty(nameof(direction));
        }
		
		public override void OnInspectorGUI() {
            serializedObject.Update();

			EditorGUILayout.PropertyField(direction, new GUIContent("Direction"));
            EditorGUILayout.Slider(offset, -1f, 1f, new GUIContent("Horizontal offset"));
			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Initialize")) pathController.initialize();
			if (GUILayout.Button("Clear")) pathController.clear();

            
        }
	}
}