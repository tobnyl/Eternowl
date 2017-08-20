using System;
using UnityEngine;
using Supersonic.Internal;

namespace Supersonic
{
    /// <summary>
    /// Serializable class for handling tracks.
    /// </summary>
    [Serializable]
    public class Track : Audio
    {
        #region Fields/Properties

        [Tooltip("Enables intro. Intro gets disabled if Reverse is checked.")]
        public bool Intro;

        [Tooltip("The time where the intro ends. When the track has finished playing it will restart from the time specified.")]
        public float IntroEndTime;

        [Tooltip("Enables looping.")]
        public bool Loop;

        /// <summary>
        /// Returns the length of the Audio Clip taking pitch into consideration.
        /// </summary>
        public float Length
        {
            get { return Clip.length / Pitch; }
        }

        /// <summary>
        /// Returns true if the track has intro end time specified.
        /// </summary>
        public bool HasIntro
        {
            get { return Intro && IntroEndTime > 0; }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Track() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="track"></param>
        public Track(Track track)
        {
            Volume = track.Volume;
            Pitch = track.Pitch;
            Intro = track.Intro;
            IntroEndTime = track.IntroEndTime;
            Loop = track.Loop;
            Clip = track.Clip;
            Group = track.Group;
            Reverse = track.Reverse;
            Mute = track.Mute;
        }

        #endregion
    }
}