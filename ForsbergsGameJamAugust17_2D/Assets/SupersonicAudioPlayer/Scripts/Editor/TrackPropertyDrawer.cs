using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Supersonic.Editor
{
    /// <summary>
    /// Custom property drawer which implements a more user friendly interface for tracks.
    /// </summary>
    [CustomPropertyDrawer(typeof(Track))]
    class TrackPropertyDrawer : PropertyDrawer
    {
        #region Fields/Properties

        private bool _show = true;
        private int _propertyHeight = 18;
        private Rect _position;
        private Track _track;

        #endregion
        #region Events

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            _position = new Rect(pos.x, pos.y, pos.width, 16);
            _track = fieldInfo.GetValue(prop.serializedObject.targetObject) as Track;

            var clip = prop.FindPropertyRelative("Clip");
            var group = prop.FindPropertyRelative("Group");
            var reverse = prop.FindPropertyRelative("Reverse");
            var mute = prop.FindPropertyRelative("Mute");
            var volume = prop.FindPropertyRelative("Volume");
            var pitch = prop.FindPropertyRelative("Pitch");

            var intro = prop.FindPropertyRelative("Intro");
            var introEndTime = prop.FindPropertyRelative("IntroEndTime");
            var loop = prop.FindPropertyRelative("Loop");

            _show = EditorGUI.Foldout(_position, _show, InsertWhitespace(prop.name));
            IncrementPositionY();

            if (_show)
            {
                EditorGUI.indentLevel++;

                DrawPropertyField(clip);
                DrawPropertyField(group);
                DrawPropertyField(reverse);
                DrawPropertyField(mute);
                DrawPropertyField(volume);
                DrawPropertyField(pitch);

                DrawIntro(intro, introEndTime, loop, reverse);

                DrawPropertyField(loop);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var reverse = property.FindPropertyRelative("Reverse");
            var intro = property.FindPropertyRelative("Intro");

            var extraHeight = (!reverse.boolValue && intro.boolValue ? _propertyHeight : 0);

            return (_show ? 162 + extraHeight : _propertyHeight);
        }

        #endregion
        #region Private Methods

        private void DrawIntro(SerializedProperty intro, SerializedProperty introEndTime, SerializedProperty loop, SerializedProperty reverse)
        {
            if (!reverse.boolValue)
            {
                DrawCheckbox(intro);
            }

            if (!reverse.boolValue && intro.boolValue)
            {
                EditorGUI.indentLevel++;
                DrawPropertyField(introEndTime);

                if (EditorApplication.isPlaying)
                {
                    loop.boolValue = true;
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawPropertyField(SerializedProperty property)
        {
            EditorGUI.PropertyField(_position, property);
            IncrementPositionY();
        }

        private void DrawCheckbox(SerializedProperty prop, string label = "")
        {
            TooltipAttribute[] toolTipAttributes = _track.GetType().GetField(prop.name).GetCustomAttributes(typeof(TooltipAttribute), true) as TooltipAttribute[];
            var toolTipAttribute = toolTipAttributes.FirstOrDefault();
            
            prop.boolValue = EditorGUI.Toggle(_position, new GUIContent((string.IsNullOrEmpty(label) ? InsertWhitespace(prop.name) : label), (toolTipAttribute != null ? toolTipAttribute.tooltip : "")), prop.boolValue);

            IncrementPositionY();
        }

        private void IncrementPositionY()
        {
            _position.y += _propertyHeight;
        }

        private string InsertWhitespace(string name)
        {
            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }

        #endregion

    }
}