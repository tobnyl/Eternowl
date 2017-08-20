using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Supersonic.Editor
{
    /// <summary>
    /// Custom property drawer which implements a more user friendly interface for sound effects.
    /// </summary>
    [CustomPropertyDrawer(typeof(SoundEffect))]
    class SoundEffectPropertyDrawer : PropertyDrawer
    {
        #region Fields/Properties

        private bool _show = true;
        private int _propertyHeight = 18;
        private Rect _position;
        private SoundEffect _soundEffect;

        #endregion
        #region Events

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            _position = new Rect(pos.x, pos.y, pos.width, 16);
            _soundEffect = fieldInfo.GetValue(prop.serializedObject.targetObject) as SoundEffect;

            var clip = prop.FindPropertyRelative("Clip");
            var group = prop.FindPropertyRelative("Group");
            var reverse = prop.FindPropertyRelative("Reverse");
            var mute = prop.FindPropertyRelative("Mute");
            var volume = prop.FindPropertyRelative("Volume");
            var pitch = prop.FindPropertyRelative("Pitch");

            var randomVolume = prop.FindPropertyRelative("RandomVolume");
            var minVolume = prop.FindPropertyRelative("MinVolume");
            var maxVolume = prop.FindPropertyRelative("MaxVolume");
            var randomPitch = prop.FindPropertyRelative("RandomPitch");
            var minPitch = prop.FindPropertyRelative("MinPitch");
            var maxPitch = prop.FindPropertyRelative("MaxPitch");
            var loops = prop.FindPropertyRelative("Loops");
            var sameVolumeForEachLoop = prop.FindPropertyRelative("SameVolumeForEachLoop");
            var samePitchForEachLoop = prop.FindPropertyRelative("SamePitchForEachLoop");

            _show = EditorGUI.Foldout(_position, _show, InsertWhitespace(prop.name));
            IncrementPositionY();

            if (_show)
            {
                EditorGUI.indentLevel++;

                DrawPropertyField(clip);
                DrawPropertyField(group);
                DrawPropertyField(reverse);
                DrawPropertyField(mute);

                RandomSliders(randomVolume, minVolume, maxVolume, volume);
                RandomSliders(randomPitch, minPitch, maxPitch, pitch);

                DrawLoops(loops, sameVolumeForEachLoop, samePitchForEachLoop, randomVolume, randomPitch);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var randomVolume = property.FindPropertyRelative("RandomVolume");
            var randomPitch = property.FindPropertyRelative("RandomPitch");

            var extraHeight = (randomVolume.boolValue ? _propertyHeight : 0);
            extraHeight += (randomPitch.boolValue ? _propertyHeight : 0);

            // Height for SameVolumeForEachLoop and SamePitchForEachLoop
            extraHeight += (randomVolume.boolValue ? _propertyHeight : 0);
            extraHeight += (randomPitch.boolValue ? _propertyHeight : 0);

            return (_show ? 180 + extraHeight : _propertyHeight);
        }

        #endregion
        #region Private Methods

        private void DrawLoops(SerializedProperty loops, SerializedProperty sameVolumeForEachLoop, SerializedProperty samePitchForEachLoop, SerializedProperty randomVolume, SerializedProperty randomPitch)
        {
            DrawPropertyField(loops);

            EditorGUI.indentLevel++;

            if (randomVolume.boolValue)
            {
                DrawCheckbox(sameVolumeForEachLoop);
            }

            if (randomPitch.boolValue)
            {
                DrawCheckbox(samePitchForEachLoop);
            }

            EditorGUI.indentLevel--;           
        }

        private void RandomSliders(SerializedProperty isRandom, SerializedProperty min, SerializedProperty max, SerializedProperty standard)
        {
            DrawCheckbox(isRandom);

            if (isRandom.boolValue)
            {
                EditorGUI.indentLevel++;
                DrawSlider(min);
                IncrementPositionY();
                DrawSlider(max);
                EditorGUI.indentLevel--;
            }
            else
            {
                DrawSlider(standard);
            }

            IncrementPositionY();
        }

        private void DrawPropertyField(SerializedProperty property)
        {
            EditorGUI.PropertyField(_position, property);
            IncrementPositionY();
        }

        private void DrawCheckbox(SerializedProperty prop, string label = "")
        {
            TooltipAttribute[] toolTipAttributes = _soundEffect.GetType().GetField(prop.name).GetCustomAttributes(typeof(TooltipAttribute), true) as TooltipAttribute[];
            var toolTipAttribute = toolTipAttributes.FirstOrDefault();
            
            prop.boolValue = EditorGUI.Toggle(_position, new GUIContent((string.IsNullOrEmpty(label) ? InsertWhitespace(prop.name) : label), (toolTipAttribute != null ? toolTipAttribute.tooltip : "")), prop.boolValue);

            IncrementPositionY();
        }

        private void DrawSlider(SerializedProperty prop, string label = "")
        {
            RangeAttribute[] rangeAttributes = _soundEffect.GetType().GetField(prop.name).GetCustomAttributes(typeof(RangeAttribute), true) as RangeAttribute[];
            var rangeAttribute = rangeAttributes.FirstOrDefault();
            
            EditorGUI.Slider(_position, prop, rangeAttribute.min, rangeAttribute.max);
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