// Original gist: https://gist.github.com/VictorHHT/24e6c3d8e68eb842a024482ca9ea98b0

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class CMinMaxSlider : PropertyAttribute {
        internal Vector2 m_MinmaxLimit;
        internal Vector2 m_MinmaxValue;

        public CMinMaxSlider(float minLimit, float maxLimit) {
            m_MinmaxLimit.x = minLimit;
            m_MinmaxLimit.y = maxLimit;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CMinMaxSlider))]
    internal sealed class MinMaxSliderDrawer : PropertyDrawer {
        float minValue;
        float maxValue;
        float minLimit;
        float maxLimit;

        bool maxReachFlag;
        bool minReachFlag;
        float minValueAtReached;
        float maxValueAtReached;
        float mouseXPosAtRached;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Event e = Event.current;
            if (property.propertyType != SerializedPropertyType.Vector2) {
                throw new ArgumentException($"{nameof(CMinMaxSlider)} underlying value type must be Vector2");
            }

            var minmaxAttribute = (CMinMaxSlider)base.attribute;
            minLimit = minmaxAttribute.m_MinmaxLimit.x;
            maxLimit = minmaxAttribute.m_MinmaxLimit.y;

            if (minLimit >= maxLimit) {
                throw new ArgumentOutOfRangeException($"{nameof(CMinMaxSlider)} min limit can not greater or equal to max limit");
            }

            // Make sure the min and max value set by the underlying field doesn't less than min limit,
            // and maxValue doesn't exceeds max limit
            minValue = Mathf.Max(property.vector2Value.x, minLimit);
            maxValue = Mathf.Min(Mathf.Max(property.vector2Value.y, minLimit), maxLimit);

            float padding = 2;
            EditorGUI.PrefixLabel(position, new GUIContent(property.name));

            // Each int field occupies 1/5 of the remaining rect area
            float intFieldWidth = ((position.xMax - position.xMin) - EditorGUIUtility.labelWidth - (padding * 3)) / 5;
            // We immediately Clamp its width to make sure it doesn't get too small
            intFieldWidth = Mathf.Max(intFieldWidth, 30);

            EditorGUI.BeginChangeCheck();
            Rect minFieldRect = new Rect(padding + position.x + EditorGUIUtility.labelWidth, position.y, intFieldWidth,
                position.height);
            // We want a delayed field because we don't want to change another bound value immediately when we type a number
            minValue = EditorGUI.DelayedFloatField(minFieldRect, minValue);


            // The three paddings are min rect padding, padding of itself, max rect padding
            float sliderWidth = position.width - (intFieldWidth * 2) - EditorGUIUtility.labelWidth - (padding * 3);
            Rect sliderRect = new Rect(minFieldRect.position.x + minFieldRect.width + padding, position.y, sliderWidth,
                position.height);

            // If user starts to drag the slider, we clear the current keyboard focus
            if (e.type == EventType.MouseDown && e.button == 0 && sliderRect.Contains(e.mousePosition)) {
                GUIUtility.keyboardControl = 0;
            }

            if (e.type == EventType.MouseDrag && e.button == 0) {
                if (maxValue >= maxLimit) {
                    // To make sure the mouse x position at reached only set once
                    if (maxReachFlag == false) {
                        maxReachFlag = true;
                        // When max is reached, we set reach min to false
                        minReachFlag = false;
                        mouseXPosAtRached = e.mousePosition.x;
                        minValueAtReached = minValue;
                    }

                    if (e.mousePosition.x >= mouseXPosAtRached) {
                        // Shrink to the right
                        float totalRange = maxLimit - minLimit;
                        // Take slider width into account
                        float minValueIncreaseAmount =
                            (e.mousePosition.x - mouseXPosAtRached) / sliderWidth * totalRange;
                        minValue = minValueAtReached + minValueIncreaseAmount;
                    }
                    else {
                        // To prevent number error
                        minValue = minValueAtReached;
                    }
                }
                else if (minValue <= minLimit) {
                    // To make sure the mouse x position at reached only set once
                    if (minReachFlag == false) {
                        minReachFlag = true;
                        // Vice versa
                        maxReachFlag = false;
                        mouseXPosAtRached = e.mousePosition.x;
                        maxValueAtReached = maxValue;
                    }

                    if (e.mousePosition.x < mouseXPosAtRached) {
                        // Shrink to the left
                        float totalRange = maxLimit - minLimit;
                        float maxValueDecreaseAmount =
                            (mouseXPosAtRached - e.mousePosition.x) / sliderWidth * totalRange;
                        maxValue = maxValueAtReached - maxValueDecreaseAmount;
                    }
                    else {
                        // To prevent number error
                        maxValue = maxValueAtReached;
                    }
                }
            }

            if (e.type == EventType.MouseUp && e.button == 0) {
                maxReachFlag = false;
                minReachFlag = false;
                mouseXPosAtRached = 0;
                minValueAtReached = 0;
                maxValueAtReached = 0;
            }

            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, minLimit, maxLimit);

            Rect maxFieldRect = new Rect(sliderRect.position.x + sliderRect.width + padding, position.y, intFieldWidth,
                position.height);
            maxValue = EditorGUI.DelayedFloatField(maxFieldRect, maxValue);

            if (EditorGUI.EndChangeCheck()) {
                //Clamp min value between min limit and max value
                minValue = Mathf.Clamp(minValue, minLimit, maxValue);
                //Clamp max value between min value and max limit
                maxValue = Mathf.Clamp(maxValue, minValue, maxLimit);

                minmaxAttribute.m_MinmaxValue.x = (float)Math.Round(minValue, 1);
                minmaxAttribute.m_MinmaxValue.y = (float)Math.Round(maxValue, 1);

                //Assign underlying variable value
                property.vector2Value = minmaxAttribute.m_MinmaxValue;
            }
        }
    }
#endif
}