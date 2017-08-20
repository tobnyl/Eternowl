using System;
using UnityEngine;
using Supersonic.Internal;

namespace Supersonic
{
    /// <summary>
    /// Serializable class for handling sound effects.
    /// </summary>
    [Serializable]
    public class SoundEffect : Audio
    {
        #region Fields/Properties

        [Tooltip("Enables random volume between two constants.")]
        public bool RandomVolume;

        [Tooltip("Min volume.")]
        [Range(0, 1)]
        public float MinVolume = 1.0f;

        [Tooltip("Max volume.")]
        [Range(0, 1)]
        public float MaxVolume = 1.0f;

        [Tooltip("Enables random pitch between two constants.")]
        public bool RandomPitch;

        [Tooltip("Min pitch.")]
        [Range(0.125f, 8f)]
        public float MinPitch = 1.0f;

        [Tooltip("Max pitch.")]
        [Range(0.125f, 8f)]
        public float MaxPitch = 1.0f;

        [Tooltip("Determines how many times the sound effect should loop. Can be overridden when calling \"PlaySoundEffect2D\" or \"PlaySoundEffect3D\".")]
        public int Loops = 1;

        [Tooltip("If this is checked and Random Volume is checked the volume will only be randomized one time.")]
        public bool SameVolumeForEachLoop = true;

        [Tooltip("If this is checked and Random Pitch is checked the pitch will only be randomized one time.")]        
        public bool SamePitchForEachLoop = true;

        /// <summary>
        /// Pitch is only valid if it is greater than 0.
        /// </summary>
        public bool IsPitchValid
        {
            get
            {
                var results = true;

                if (MinPitch <= 0)
                {
                    Debug.LogError("MinPitch can't be 0");
                    results = false;
                }

                if (MaxPitch <= 0)
                {
                    Debug.LogError("MaxPitch can't be 0");
                    results = false;
                }

                return results;
            }
        }

        #endregion
    }
}