using UnityEngine;

namespace Supersonic.Internal
{
    /// <summary>
    /// Stores data for a sound effect to prevent it from playing multiple times per frame.
    /// </summary>
    class QueuedSoundEffect
    {
        #region Fields/Properties

        public string Name { get; set; }
        public Vector3 Position { get; set; }

        #endregion
    }
}