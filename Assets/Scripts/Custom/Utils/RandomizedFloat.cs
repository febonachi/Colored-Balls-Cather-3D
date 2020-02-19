using System;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Utils{
    [Serializable] public struct RandomizedFloat{
        [SerializeField] private bool randomize;
        [SerializeField] private float customValue;
        [SerializeField] private float randomValue;
        [SerializeField] private MinMaxValue minMaxValue;

        #region public properties
        public float value {
            get {
                if(!randomValueInitialized) {
                    randomValueInitialized = true;
                    randomValue = minMaxValue.random;
                }
                return randomize ? randomValue : customValue;
            }
        }
        public float randomizedValue => randomize ? minMaxValue.random : customValue;
        #endregion

        private bool randomValueInitialized;

        public RandomizedFloat(bool randomize, MinMaxValue minMaxValue, float customValue = 0f){
            this.randomize = randomize;
            this.minMaxValue = minMaxValue;
            this.customValue = customValue;
            this.randomValue = customValue;
            randomValueInitialized = false;
        }

        public static implicit operator RandomizedFloat(float value) {
            RandomizedFloat rf = new RandomizedFloat();
            rf.randomize = false;
            rf.customValue = value;
            rf.randomValue = value;
            return rf;
        }

        public static implicit operator RandomizedFloat(MinMaxValue minMaxValue) => new RandomizedFloat(true, minMaxValue);
        
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(RandomizedFloat))]
        public class RandomizedFloatPropertyDrawer : PropertyDrawer{
            private const int space = 5;
            
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){                
                EditorGUI.BeginProperty(position, label, property);
                int indentLevel = EditorGUI.indentLevel;

                position = EditorGUI.PrefixLabel(position, label);
                
                EditorGUI.indentLevel = 0;
                Rect toggleRect = new Rect(position.x, position.y, 24, position.height);
                EditorGUIUtility.labelWidth = toggleRect.width / 2;
                SerializedProperty randomize = property.FindPropertyRelative("randomize");
                EditorGUI.PropertyField(toggleRect, randomize, new GUIContent("R"));
                position.x += toggleRect.width + EditorGUIUtility.labelWidth + space;
                position.width -= toggleRect.width + toggleRect.width + EditorGUIUtility.labelWidth + space;
                if(randomize.boolValue) {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("minMaxValue"), GUIContent.none);
                    if(Application.isPlaying){
                        position.x += position.width + space;
                        position.width = toggleRect.width;
                        EditorGUI.LabelField(position, $"{property.FindPropertyRelative("randomValue").floatValue:F1}");
                    }
                } else EditorGUI.PropertyField(position, property.FindPropertyRelative("customValue"), new GUIContent("F"));

                EditorGUI.indentLevel = indentLevel;
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}