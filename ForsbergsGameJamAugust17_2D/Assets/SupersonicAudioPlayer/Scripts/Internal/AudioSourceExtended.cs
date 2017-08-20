using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Supersonic.Internal
{
    /// <summary>
    /// Component that destroys a sound effect when it has finished playing.
    /// </summary>
    class AudioSourceExtended : MonoBehaviour
    {
        #region Fields/Properties

        private float _duration;
        private AudioSource _audioSource;
        private Coroutine _destroyCoroutine;
        private Coroutine _disableLoopingCoroutine;
        private Coroutine _dynamicPitchAndVolumeCoroutine;
        private List<SoundEffectAttributes> _soundEffectAttributesList;
        private int _playedLoops;

        public bool IsUiSoundEffect { get; set; }

        #endregion
        #region Events

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        #endregion
        #region Public Methods

        public void Initialize(float duration, bool isUiSoundEffect, List<SoundEffectAttributes> soundEffectAttributesList = null)
        {
            _duration = duration;

            _destroyCoroutine = StartCoroutine(DestroyCoroutine(_duration));

            if (soundEffectAttributesList == null)
            {
                _disableLoopingCoroutine = StartCoroutine(DisableLoopingCoroutine(_duration * 0.5f));
            }            
            else
            {
                _soundEffectAttributesList = soundEffectAttributesList;
                _dynamicPitchAndVolumeCoroutine = StartCoroutine(DynamicPitchAndVolumeCoroutine());

                _disableLoopingCoroutine = StartCoroutine(DisableLoopingCoroutine(_duration - (soundEffectAttributesList[soundEffectAttributesList.Count - 1].Duration * 0.5f)));
            }

            IsUiSoundEffect = isUiSoundEffect;
        }

        public void Pause()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }

            if (_disableLoopingCoroutine != null)
            {
                StopCoroutine(_disableLoopingCoroutine);
                _disableLoopingCoroutine = null;
            }

            if (_dynamicPitchAndVolumeCoroutine != null)
            {
                StopCoroutine(_dynamicPitchAndVolumeCoroutine);
                _dynamicPitchAndVolumeCoroutine = null;
            }

            _audioSource.Pause();
        }

        public void Play()
        {
            var durationLeft = 0f;

            if (_soundEffectAttributesList == null)
            {
                durationLeft = _duration - _audioSource.time / _audioSource.pitch;

                _destroyCoroutine = StartCoroutine(DestroyCoroutine(durationLeft));

                if (_audioSource.loop == true)
                {
                    _disableLoopingCoroutine = StartCoroutine(DisableLoopingCoroutine(durationLeft * 0.5f));
                }
            }
            else
            {                
                _soundEffectAttributesList.RemoveRange(0, _playedLoops);                

                _playedLoops = 0;

                var currentLoop = _soundEffectAttributesList[0];

                var currentLoopTime = _audioSource.time / currentLoop.Pitch;
                currentLoop.TimeLeft = currentLoop.Duration - currentLoopTime;                

                durationLeft = currentLoop.TimeLeft;

                if (_soundEffectAttributesList.Count > 1)
                {
                    for (var i = 1; i < _soundEffectAttributesList.Count; i++)
                    {
                        durationLeft += _soundEffectAttributesList[i].Duration;
                    }
                }

                _destroyCoroutine = StartCoroutine(DestroyCoroutine(durationLeft));

                if (_soundEffectAttributesList.Count > 1)
                {
                    _disableLoopingCoroutine = StartCoroutine(DisableLoopingCoroutine(durationLeft - (_soundEffectAttributesList[_soundEffectAttributesList.Count - 1].Duration * 0.5f)));
                }
                else
                {
                    _audioSource.loop = false;
                }

                _dynamicPitchAndVolumeCoroutine = StartCoroutine(DynamicPitchAndVolumeCoroutine());
            }

            _audioSource.Play();
        }

        private IEnumerator DynamicPitchAndVolumeCoroutine()
        {
            foreach (var audioAttributes in _soundEffectAttributesList)
            {
                _audioSource.pitch = audioAttributes.Pitch;
                _audioSource.volume = audioAttributes.Volume;

                yield return new WaitForSecondsRealtime(audioAttributes.TimeLeft);
                
                _playedLoops++;

                if (_audioSource == null)
                {
                    yield break;
                }
            }

            yield return null;
        }

        private IEnumerator DestroyCoroutine(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            Destroy(gameObject);

            yield return null;
        }

        private IEnumerator DisableLoopingCoroutine(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            _audioSource.loop = false;

            yield return null;
        }

        #endregion
    }
}