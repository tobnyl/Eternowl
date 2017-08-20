using UnityEngine;

namespace Supersonic.Internal
{
    /// <summary>
    /// Stores data for a track that shall be resumed later.
    /// </summary>
    class OldTrack
    {
        #region Fields/Properties

        public AudioSource AudioSource;
        public Track Track;
        public bool RestartCurrentTrackWhenDone;
        public bool MuteSoundEffects;

        #endregion
        #region Public Methods

        public OldTrack()
        {
            AudioSource = null;
            Track = null;
        }

        #endregion
    }
}