using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Supersonic.Internal
{
    /// <summary>
    /// Base class for Track and SoundEffect.
    /// </summary>
    [Serializable]
    public abstract class Audio
    {
        #region Fields/Properties

        [Tooltip("The Audio Clip to use.")]
        public AudioClip Clip;

        [Tooltip("The Audio Mixer Group to use, if not set it will use the master group.")]
        public AudioMixerGroup Group;

        [Tooltip("Plays the Audio Clip reversed.")]
        public bool Reverse;

        [Tooltip("Mutes the Audio Clip.")]
        public bool Mute;

        [Tooltip("Volume for the Audio Clip.")]
        [Range(0, 1)]
        public float Volume = 1.0f;

        [Tooltip("Pitch for the Audio Clip, a value of 1 equals no pitch.")]
        [Range(0.125f, 8f)]
        public float Pitch = 1.0f;

        /// <summary>
        /// Shortcut for retrieving the name of the audio clip.
        /// </summary>
        public string Name
        {
            get { return (Clip != null ? Clip.name : ""); }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Audio()
        {

        }

        /// <summary>
        /// Returns true if no audio clip has been assigned.
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            return Clip == null;
        }

        #endregion
    }
}