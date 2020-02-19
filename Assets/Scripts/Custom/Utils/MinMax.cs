using System;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Utils{
    [Serializable] public struct MinMaxValue{
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;

        #region public properties
        public float min {
            get {
                validateValue(); 
                return minValue;
            }
        }
        
        public float max {
            get {
                validateValue();
                return maxValue;
            }
        }
        public float random => Random.Range(min, max);
        #endregion

        #region public
        public MinMaxValue(float minVal, float maxVal){
            minValue = minVal;
            maxValue = maxVal;
        }

        public static implicit operator MinMaxValue(Tuple<float, float> t) => new MinMaxValue(t.Item1, t.Item2);

        public void validateValue(){
            if(maxValue < minValue) Utility.swap(ref minValue, ref maxValue);
        }
        #endregion

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(MinMaxValue))]
        public class MinMaxPropertyDrawer : PropertyDrawer{

            private const int space = 5;
            
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
                EditorGUI.BeginProperty(position, label, property);
                int indentLevel = EditorGUI.indentLevel;

                position = EditorGUI.PrefixLabel(position, label);

                EditorGUIUtility.labelWidth = 32;
                position.width = (position.width - space) / 2;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("minValue"), new GUIContent("min:"));
                position.x += position.width + space;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("maxValue"), new GUIContent("max:"));

                EditorGUI.indentLevel = indentLevel;
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}