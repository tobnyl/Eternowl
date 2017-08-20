using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UE = UnityEngine;
using Supersonic.Internal;

namespace Supersonic
{
    /// <summary>
    /// Audio player for playing sound effects and tracks.
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        #region Fields/Properties

        [Tooltip("The audio mixer's master group.")]
        public AudioMixerGroup MasterGroup;

        [Tooltip("The audio mixer's track master group.")]
        public AudioMixerGroup TracksMasterGroup;

        [Tooltip("The audio mixer's sound effect master group.")]
        public AudioMixerGroup SoundEffectsMasterGroup;

        [Tooltip("Prefab containing an \"Audio Source\" and an \"Audio Source Extended\" which is instantiated when playing a sound effect.")]
        public AudioSource AudioSourcePrefab;

        [Tooltip("Allows the same sound effect to be played multiple times per frame.")]
        public bool AllowDuplicatesPerFrame;
        
        private bool _muteSoundEffects;
        private static AudioPlayer _instance;
        private List<QueuedSoundEffect> _clipList;
        private float _audioSourceCollidingDistance;
        private AudioSource _trackAudioSource;
        private Stack<OldTrack> _oldTrackList;
        private Coroutine _continueOrRestartOldTrackWhenDoneCoroutine;
        private Coroutine _trackWithIntroCoroutine;
        private Track _currentTrack;
        private bool _isPauseTrack;
        private bool _isPauseSilently;

        /// <summary>
        /// Static instance for accessing the audio players's methods.
        /// </summary>
        public static AudioPlayer Instance
        {
            get { return _instance; }
        }

        #endregion
        #region Events

        private void Awake()
        {
            _clipList = new List<QueuedSoundEffect>();
            _trackAudioSource = gameObject.AddComponent<AudioSource>();
            _audioSourceCollidingDistance = AudioSourcePrefab.minDistance * 2f;
            _oldTrackList = new Stack<OldTrack>();

            if (_instance == null)
            {
                _instance = GetComponent<AudioPlayer>();
            }

            DontDestroyOnLoad(gameObject);
        }

        private void LateUpdate()
        {
            if (_clipList != null && _clipList.Any())
            {
                _clipList.Clear();
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="track">The track to be played.</param>
        /// <param name="destroySoundEffects">Destroy all sound effects currently playing.</param>
        /// <param name="muteSoundEffects">Mute all upcoming sound effects for the duration of this track.</param>
        public void PlayTrack(Track track, bool destroySoundEffects = false, bool muteSoundEffects = false)
        {
            if (!_isPauseTrack)
            {
                if (track.IsNull())
                {
                    Debug.LogError("PlayTrack: No clip assigned");
                    return;
                }

                if (_trackWithIntroCoroutine != null)
                {
                    Debug.Log("PlayTrack: Stop TrackWithIntroCoroutine");
                    StopCoroutine(_trackWithIntroCoroutine);
                    _trackWithIntroCoroutine = null;
                }

                var oldTrack = PeekOldTrackList();

                if (oldTrack != null && _continueOrRestartOldTrackWhenDoneCoroutine != null)
                {
                    StopCoroutine(_continueOrRestartOldTrackWhenDoneCoroutine);
                    _continueOrRestartOldTrackWhenDoneCoroutine = null;

                    Destroy(oldTrack.AudioSource);
                    _oldTrackList.Pop();
                }

                _trackAudioSource.time = 0;
                _trackAudioSource.clip = track.Clip;
                _trackAudioSource.outputAudioMixerGroup = FirstNotNullAudioMixerGroup(track.Group, TracksMasterGroup, MasterGroup);
                _trackAudioSource.loop = track.Loop;
                _trackAudioSource.mute = track.Mute;
                _trackAudioSource.volume = track.Volume;
                _trackAudioSource.pitch = (track.Reverse ? track.Pitch * -1 : track.Pitch);
                _trackAudioSource.Play();
                _currentTrack = track;

                if (track.Reverse && !track.Loop)
                {
                    StartCoroutine(TrackReverseCoroutine(_trackAudioSource));
                }

                if (track.HasIntro && !track.Reverse)
                {
                    Debug.Log("PlayTrack: Start TrackWithIntroCoroutine");
                    _trackAudioSource.loop = true;
                    _trackWithIntroCoroutine = StartCoroutine(TrackWithIntroCoroutine(track, _trackAudioSource));
                }

                if (destroySoundEffects)
                {
                    DestroySoundEffects();
                }

                _muteSoundEffects = muteSoundEffects;
            }
        }

        /// <summary>
        /// Plays the specified track and pauses the current track (which is resumed when the specified track has ended.
        /// </summary>
        /// <param name="track">The track to be played</param>
        /// <param name="restartCurrentTrackWhenDone">If true it restarts the current track when the specified track has ended. If false the current track is paused.</param>
        /// <param name="destroySoundEffects">Destroy all sound effects currently playing.</param>
        /// <param name="muteSoundEffects">Mute all upcoming sound effects for the duration of this track.</param>
        /// <param name="destroySoundEffectsWhenDone">Destroy all sound effects when the temporary track has finished playing.</param>
        public void PlayTemporaryTrack(Track track, bool restartCurrentTrackWhenDone = false, bool destroySoundEffects = false, bool muteSoundEffects = false, bool destroySoundEffectsWhenDone = false)
        {
            if (!_isPauseTrack)
            {
                var oldTrack = PeekOldTrackList();
                track.Loop = false;

                if (_continueOrRestartOldTrackWhenDoneCoroutine != null)
                {
                    StopCoroutine(_continueOrRestartOldTrackWhenDoneCoroutine);
                    _continueOrRestartOldTrackWhenDoneCoroutine = null;
                }

                if (oldTrack != null && oldTrack.AudioSource != null)
                {
                    oldTrack.RestartCurrentTrackWhenDone = restartCurrentTrackWhenDone;      
                }
                else if (oldTrack == null)
                {
                    PushOldTrack(restartCurrentTrackWhenDone);
                }

                PlayTrack(track, destroySoundEffects, muteSoundEffects);

                _continueOrRestartOldTrackWhenDoneCoroutine = StartCoroutine(ContinueOrRestartOldTrackWhenDoneCoroutine(track.Length, destroySoundEffectsWhenDone));
            }
        }

        /// <summary>
        /// Pauses the current track, use this for a pause menu with NO music.
        /// </summary>
        /// <param name="restartCurrentTrackWhenDone">If true it restarts the current track when the specified track has ended. If false the current track is paused.</param>
        /// <param name="pauseSoundEffects">If true it pauses all sound effects currently playing. If false it destroys all sound effects currently playing.</param>
        public void PlayPauseSilently(bool restartCurrentTrackWhenDone = false, bool pauseSoundEffects = true)
        {
            if (!_isPauseTrack)
            {
                if (_trackWithIntroCoroutine != null)
                {
                    Debug.Log("PlayPauseSilently: Stop TrackWithIntroCoroutine");
                    StopCoroutine(_trackWithIntroCoroutine);
                    _trackWithIntroCoroutine = null;
                }
                
                _trackAudioSource.Pause();
                PushOldTrack(restartCurrentTrackWhenDone);

                if (pauseSoundEffects)
                {
                    PauseSoundEffects();
                }
                else
                {
                    DestroySoundEffects();
                }

                DestroyUiSoundEffects();

                _muteSoundEffects = false;

                _isPauseTrack = true;
                _isPauseSilently = true;
            }
            else
            {
                Debug.LogWarning("PlayPauseSilently can only be called if the game isn't paused");
            }
        }

        /// <summary>
        /// Resumes the current track, can only be called if "PlayPauseSilently" has been called earlier.
        /// </summary>
        /// <param name="destroyUiSoundEffects">Destroys all sound effects that have been initiated while the game was paused.</param>
        public void StopPauseSilently(bool destroyUiSoundEffects = true)
        {
            if (_isPauseTrack && _isPauseSilently)
            {
                StopPause(destroyUiSoundEffects);
                _isPauseSilently = false;
            }
            else
            {
                Debug.LogWarning("StopPauseSilently can only be called if PlayPauseSilently has been called earlier");
            }
        }

        /// <summary>
        /// Pauses the current track, use this for a pause menu with music.
        /// </summary>
        /// <param name="track">The track to be played</param>
        /// <param name="restartCurrentTrackWhenDone">If true it restarts the current track when the specified track has ended. If false the current track is paused.</param>
        /// <param name="pauseSoundEffects">If true it pauses all sound effects currently playing. If false it destroys all sound effects currently playing.</param>
        public void PlayPauseTrack(Track track, bool restartCurrentTrackWhenDone = false, bool pauseSoundEffects = true)
        {
            if (!_isPauseTrack)
            {
                if (_continueOrRestartOldTrackWhenDoneCoroutine != null)
                {
                    StopCoroutine(_continueOrRestartOldTrackWhenDoneCoroutine);
                    _continueOrRestartOldTrackWhenDoneCoroutine = null;
                }

                if (pauseSoundEffects)
                {
                    PauseSoundEffects();
                }

                PushOldTrack(restartCurrentTrackWhenDone);

                PlayTrack(track, (pauseSoundEffects == false), false);

                DestroyUiSoundEffects();

                _muteSoundEffects = false;

                _isPauseTrack = true;
            }
            else
            {
                Debug.LogWarning("PlayPauseTrack can only be called if the game isn't paused");
            }
        }

        /// <summary>
        /// Resumes the current track, can only be called if "PlayPauseTrack" has been called earlier.
        /// </summary>
        /// <param name="destroyUiSoundEffects">Destroys all sound effects that have been initiated while the game was paused.</param>
        public void StopPauseTrack(bool destroyUiSoundEffects = true)
        {
            if (_isPauseTrack && !_isPauseSilently)
            {
                StopPause(destroyUiSoundEffects);
            }
            else
            {
                Debug.LogWarning("StopPauseTrack can only be called if PlayPauseTrack has been called earlier");

            }
        }

        /// <summary>
        /// Plays the specified sound effect as a 2D sound.
        /// </summary>
        /// <param name="soundEffect">The sound effect to be played.</param>
        /// <param name="loops">Overrides the default number of loops if set to greater than 0.</param>
        public void PlaySoundEffect2D(SoundEffect soundEffect, int loops = 0)
        {
            if (soundEffect.IsNull())
            {
                Debug.LogError("PlaySoundEffect2D: No clip assigned");
                return;
            }

            if (!soundEffect.IsNull() && !soundEffect.Mute && !_muteSoundEffects && NotInQueue(soundEffect))
            {
                PlaySoundEffect(soundEffect, Vector3.zero, 0, loops);
            }
        }

        /// <summary>
        /// Plays the specified sound effect as a 3D sound.
        /// </summary>
        /// <param name="soundEffect">The sound effect to be played.</param>
        /// <param name="position">The position where the sound effect should be played.</param>
        /// <param name="loops">Overrides the default number of loops if set to greater than 0.</param>
        public void PlaySoundEffect3D(SoundEffect soundEffect, Vector3 position, int loops = 0)
        {
            if (soundEffect.IsNull())
            {
                Debug.LogError("PlaySoundEffect3D: No clip assigned");
            }

            if (!soundEffect.IsNull() && !soundEffect.Mute && !_muteSoundEffects && NotInQueueAndOutOfDistance(soundEffect, position))
            {
                PlaySoundEffect(soundEffect, position, 1, loops);
            }
        }

        /// <summary>
        /// Destroy all sound effects currently playing.
        /// </summary>
        public void DestroySoundEffects()
        {
            var soundEffectsPlaying = GetComponentsInChildren<AudioSource>().Where(x => x.transform.parent == transform);

            foreach (var soundEffect in soundEffectsPlaying)
            {
                Destroy(soundEffect.gameObject);
            }
        }

        #endregion
        #region Private Methods

        private void ResumePausedSoundEffects()
        {
            var soundEffectsPlaying = GetComponentsInChildren<AudioSourceExtended>().Where(x => !x.IsUiSoundEffect);

            foreach (var soundEffect in soundEffectsPlaying)
            {
                soundEffect.Play();
            }
        }

        private void PauseSoundEffects()
        {
            var soundEffectsPlaying = GetComponentsInChildren<AudioSourceExtended>();

            foreach (var soundEffect in soundEffectsPlaying)
            {
                soundEffect.Pause();
            }
        }

        private void DestroyUiSoundEffects()
        {
            var soundEffectsPlaying = GetComponentsInChildren<AudioSourceExtended>().Where(x => x.IsUiSoundEffect);

            foreach (var soundEffect in soundEffectsPlaying)
            {
                if (soundEffect.IsUiSoundEffect)
                {
                    Destroy(soundEffect.gameObject);
                }
            }
        }

        private void StopPause(bool destroyUiSoundEffects)
        {
            if (destroyUiSoundEffects)
            {
                DestroyUiSoundEffects();
            }

            ResumePausedSoundEffects();

            var oldTrack = PeekOldTrackList();

            if (oldTrack != null && oldTrack.AudioSource != null)
            {
                if (_continueOrRestartOldTrackWhenDoneCoroutine != null)
                {
                    StopCoroutine(_continueOrRestartOldTrackWhenDoneCoroutine);
                }

                PopAndPlayOldTrack();

                _isPauseTrack = false;

                if (_oldTrackList.Count == 1)
                {
                    _continueOrRestartOldTrackWhenDoneCoroutine = StartCoroutine(ContinueOrRestartOldTrackWhenDoneCoroutine(_currentTrack.Length - (_trackAudioSource.time / _currentTrack.Pitch)));
                }
            }
        }

        private OldTrack PeekOldTrackList()
        {
            return _oldTrackList.Count > 0 ? _oldTrackList.Peek() : null;
        }

        private OldTrack InitializeOldTrack(AudioSource audioSource, Track track, bool restartCurrentTrackWhenDone)
        {
            OldTrack oldTrack = new OldTrack();

            oldTrack.AudioSource = gameObject.AddComponent<AudioSource>();
            oldTrack.AudioSource.clip = _trackAudioSource.clip;
            oldTrack.AudioSource.outputAudioMixerGroup = _trackAudioSource.outputAudioMixerGroup;
            oldTrack.AudioSource.loop = _trackAudioSource.loop;
            oldTrack.AudioSource.mute = _trackAudioSource.mute;
            oldTrack.AudioSource.time = _trackAudioSource.time;
            oldTrack.AudioSource.volume = _trackAudioSource.volume;
            oldTrack.AudioSource.pitch = _trackAudioSource.pitch;
            oldTrack.AudioSource.Pause();
            oldTrack.Track = new Track(_currentTrack);
            oldTrack.RestartCurrentTrackWhenDone = restartCurrentTrackWhenDone;            
            oldTrack.MuteSoundEffects = _muteSoundEffects;

            return oldTrack;
        }

        private void PopAndPlayOldTrack()
        {
            var oldTrack = _oldTrackList.Pop();

            _trackAudioSource.clip = oldTrack.AudioSource.clip;
            _trackAudioSource.outputAudioMixerGroup = oldTrack.AudioSource.outputAudioMixerGroup;
            _trackAudioSource.loop = oldTrack.AudioSource.loop;
            _trackAudioSource.mute = oldTrack.AudioSource.mute;

            _trackAudioSource.time = (oldTrack.RestartCurrentTrackWhenDone ? 0 : oldTrack.AudioSource.time);

            _trackAudioSource.volume = oldTrack.AudioSource.volume;
            _trackAudioSource.pitch = oldTrack.AudioSource.pitch;
            _trackAudioSource.Play();
            _currentTrack = new Track(oldTrack.Track);            
            _muteSoundEffects = oldTrack.MuteSoundEffects;

            if (oldTrack.Track.HasIntro && !oldTrack.Track.Reverse)
            {
                Debug.Log("PopAndPlayOldTrack: Start TrackWithIntroCoroutine");
                _trackWithIntroCoroutine = StartCoroutine(TrackWithIntroCoroutine(_currentTrack, _trackAudioSource));
            }

            Destroy(oldTrack.AudioSource);
        }

        private void PushOldTrack(bool restartCurrentTrackWhenDone)
        {
            _oldTrackList.Push(InitializeOldTrack(_trackAudioSource, _currentTrack, restartCurrentTrackWhenDone));
        }

        private bool NotInQueue(SoundEffect soundEffect)
        {
            return _clipList.Where(x => x.Name == soundEffect.Clip.name).Count<QueuedSoundEffect>() == 0;
        }

        private bool NotInQueueAndOutOfDistance(SoundEffect soundEffect, Vector3 position)
        {
            var clips = _clipList.Where(x => x.Name == soundEffect.Clip.name);

            foreach (var clip in clips)
            {
                if (Vector3.Distance(clip.Position, position) < _audioSourceCollidingDistance)
                {
                    return false;
                }
            }

            return true;
        }

        private AudioMixerGroup FirstNotNullAudioMixerGroup(params AudioMixerGroup[] audioMixerGroups)
        {
            foreach (var audioMixerGroup in audioMixerGroups)
            {
                if (audioMixerGroup != null)
                {
                    return audioMixerGroup;
                }
            }

            return null;
        }

        private float GetPitch(SoundEffect soundEffect)
        {
            if (!soundEffect.RandomPitch)
            {
                return (soundEffect.Reverse ? soundEffect.Pitch * -1 : soundEffect.Pitch);
            }

            var pitch = RandomRange(soundEffect.MinPitch, soundEffect.MaxPitch);

            return (soundEffect.Reverse ? pitch * -1 : pitch);
        }

        private float GetVolume(SoundEffect soundEffect)
        {
            if (!soundEffect.RandomVolume)
            {
                return soundEffect.Volume;
            }

            return RandomRange(soundEffect.MinVolume, soundEffect.MaxVolume);
        }

        private float RandomRange(float min, float max)
        {
            if (min != 1.0f || max != 1.0f)
            {
                return UE.Random.Range(min, max);
            }

            return max;
        }

        private void PlaySoundEffect(SoundEffect soundEffect, Vector3 position, float spatialBlend, int loops)
        {
            var audioSource = Instantiate(AudioSourcePrefab, transform.position, transform.rotation) as AudioSource;
            var audioSourceExtended = audioSource.GetComponent<AudioSourceExtended>();
            var totalDuration = 0f;
            var numLoops = (loops > 0 ? loops : soundEffect.Loops); 

            audioSource.transform.parent = this.transform;
            audioSource.transform.position = position;

            audioSource.spatialBlend = spatialBlend;
            audioSource.clip = soundEffect.Clip;
            audioSource.outputAudioMixerGroup = FirstNotNullAudioMixerGroup(soundEffect.Group, SoundEffectsMasterGroup, MasterGroup);            

            audioSource.loop = true;

            if (!soundEffect.IsPitchValid)
            {
                return;
            }

            if (numLoops > 1)
            {
                var soundEffectAttributesList = new List<SoundEffectAttributes>();

                var pitch = GetPitch(soundEffect);
                var volume = GetVolume(soundEffect);

                for (var i = 0; i < numLoops; i++)
                {
                    if (i > 0)
                    {
                        if (!soundEffect.SamePitchForEachLoop)
                        {
                            pitch = GetPitch(soundEffect);
                        }

                        if (!soundEffect.SameVolumeForEachLoop)
                        {
                            volume = GetVolume(soundEffect);
                        }
                    }

                    soundEffectAttributesList.Add(new SoundEffectAttributes(pitch, volume, soundEffect.Clip.length));

                    totalDuration += soundEffectAttributesList[i].Duration;
                }

                audioSourceExtended.Initialize(totalDuration, _isPauseTrack, soundEffectAttributesList);
            }
            else
            {
                audioSource.pitch = GetPitch(soundEffect);
                audioSource.volume = GetVolume(soundEffect);

                totalDuration = soundEffect.Clip.length / Math.Abs(audioSource.pitch);

                audioSourceExtended.Initialize(totalDuration * numLoops, _isPauseTrack);
            }

            audioSource.Play();

            if (!AllowDuplicatesPerFrame)
            {
                _clipList.Add(new QueuedSoundEffect { Name = soundEffect.Clip.name, Position = position });
            }
        }

        #endregion
        #region Coroutines

        private IEnumerator ContinueOrRestartOldTrackWhenDoneCoroutine(float length, bool destroySoundEffectsWhenDone = false)
        {
            yield return new WaitForSeconds(length);

            if (destroySoundEffectsWhenDone)
            {
                DestroySoundEffects();
            }

            PopAndPlayOldTrack();
        }

        private IEnumerator TrackWithIntroCoroutine(Track track, AudioSource audioSource)
        {
            var yieldTime = 0f;

            while (true)
            {
                yieldTime = track.Length - (audioSource.time / track.Pitch);

                yield return new WaitForSeconds(yieldTime);

                audioSource.time = track.IntroEndTime;
            }
        }

        private IEnumerator TrackReverseCoroutine(AudioSource audioSource)
        {
            audioSource.loop = true;

            yield return new WaitForSeconds(0.1f);

            audioSource.loop = false;
        }

        #endregion
    }
}