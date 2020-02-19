using System;
using UnityEngine;
using UnityEditor;

namespace Utils {
    [Serializable] public class Percentile  {
        public int count;
        public float[] percentiles;

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Percentile))]
        public class PercentilePropertyDrawer : PropertyDrawer {
            private const int space = 5;
            private const int clampCountValue = 6;

            private bool customEditPercentile = false;
            private float[] customEditPerventileValues = new float[clampCountValue];

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                EditorGUI.BeginProperty(position, label, property);
                int indentLevel = EditorGUI.indentLevel;

                EditorGUI.LabelField(position, "Percentile:");
                position.x += 64;
                position.width -= 64;

                EditorGUIUtility.labelWidth = 24;

                Rect countPropertyRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth * 2, position.height);

                SerializedProperty countProperty = property.FindPropertyRelative("count");
                EditorGUI.PropertyField(countPropertyRect, countProperty, new GUIContent("##"));
                countProperty.intValue = Mathf.Clamp(countProperty.intValue, 1, clampCountValue);

                SerializedProperty percentilesProperty = property.FindPropertyRelative("percentiles");
                Percentile target = fieldInfo.GetValue(property.serializedObject.targetObject) as Percentile;
                target.percentiles = new float[countProperty.intValue];
                position.x += countPropertyRect.width + space;

                Rect customEditRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                customEditPercentile = EditorGUI.Toggle(customEditRect, customEditPercentile);
                position.x += customEditRect.width;

                float timeStep = 1f / (countProperty.intValue + 1);
                float boxWidth = (position.width - countPropertyRect.width - space) / countProperty.intValue;
                Rect box = new Rect(position.x, position.y, boxWidth, position.height);

                float time = timeStep;
                for(int i = 0; i < countProperty.intValue; i++){
                    if(!customEditPercentile){
                        target.percentiles[i] = time;
                        EditorGUI.FloatField(box, target.percentiles[i]);
                    } else {
                        float value = customEditPerventileValues[i];
                        customEditPerventileValues[i] = EditorGUI.FloatField(box, value);
                        target.percentiles[i] = customEditPerventileValues[i];
                    }
                    box.x += boxWidth + space;
                    time += timeStep;
                }

                EditorGUI.indentLevel = indentLevel;
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}
