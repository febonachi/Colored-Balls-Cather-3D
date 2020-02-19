using System;
using UnityEditor;
using UnityEngine;

namespace Utils {
    [Serializable] public struct IgnoredAxis {
        [SerializeField] private bool xAxis;
        [SerializeField] private bool yAxis;
        [SerializeField] private bool zAxis;

        #region public properties
        public bool x => xAxis;
        public bool y => yAxis;
        public bool z => zAxis;
        public Vector3 axis => new Vector3((!x).toInt(), (!y).toInt(), (!z).toInt());
        #endregion

        #region public static properties
        public static IgnoredAxis up => new IgnoredAxis(false, true, false);
        public static IgnoredAxis right => new IgnoredAxis(true, false, false);
        public static IgnoredAxis forward => new IgnoredAxis(false, false, true);
        #endregion

        #region public
        public IgnoredAxis(bool x, bool y, bool z) {
            xAxis = x;
            yAxis = y;
            zAxis = z;
        }

        public static bool operator !=(IgnoredAxis first, IgnoredAxis second) => !(first == second);
        public static bool operator ==(IgnoredAxis first, IgnoredAxis second) => first.axis == second.axis;
        #endregion

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(IgnoredAxis))]
    public class IgnoredAxisDrawer : PropertyDrawer{

        private const int space = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            EditorGUI.BeginProperty(position, label, property);
            int indentLevel = EditorGUI.indentLevel;
            
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUIUtility.labelWidth = 12 + (indentLevel * 12);
            position.width = EditorGUIUtility.labelWidth * 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("xAxis"), new GUIContent("X"));
            position.x += position.width + space;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("yAxis"), new GUIContent("Y"));
            position.x += position.width + space;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("zAxis"), new GUIContent("Z"));

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }
    }
#endif
    }
}