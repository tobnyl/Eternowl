using System;

namespace Supersonic.Internal
{
    /// <summary>
    /// Stores data for a loop instance.
    /// </summary>
    class SoundEffectAttributes
    {
        #region Fields/Properties

        public float Pitch { get; set; }
        public float Volume { get; set; }
        public float Duration { get; set; }
        public float TimeLeft { get; set; }

        #endregion
        #region Public Methods

        public SoundEffectAttributes(float pitch, float volume, float originalClipLength)
        {
            Pitch = pitch;
            Volume = volume;
            Duration = originalClipLength / Math.Abs(pitch);
            TimeLeft = Duration;
        }

        #endregion
    }
}