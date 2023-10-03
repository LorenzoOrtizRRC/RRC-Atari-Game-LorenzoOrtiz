using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    // A generic script used to update a slider's value, mostly used for numeric resources like Health. ResourceBar is assigned to various events.
    [SerializeField] private Slider _resourceSlider;
    [SerializeField] private Slider _decaySlider;
    [SerializeField] private bool _showDecay = false;
    [SerializeField] private float _decayStartDelay = 0.5f; // delay in seconds before starting to decay. THIS WILL NOT RESET EVERY TIME DECAY STARTS, AND PERSISTS UNTIL DECAYING FINISHES.
    [SerializeField] private float _decayDuration= 0.2f;    // total duration for decay to finish in seconds

    private Coroutine _currentDecayCoroutine;
    private bool _decayCoroutineIsRunning = false;
    private float _decayStartDelayTimer = 0f;

    private float _endDecayValue = 0f;

    public void UpdateSliderValue(float value)
    {
        _resourceSlider.value = Mathf.Clamp(value, 0f, 1f);

        // check if gameobject is active as failsafe to stop starting coroutines when disabled
        if (_showDecay && gameObject.activeInHierarchy)
        {
            if (value < _decaySlider.value)
            {
                // if (_decayCoroutineIsRunning) StopCoroutine(_currentDecayCoroutine);
                if (!_decayCoroutineIsRunning) _currentDecayCoroutine = StartCoroutine(StartDecay(value));
            }
            else _decaySlider.value = value;
        }
    }

    public IEnumerator StartDecay(float endDecayValue)
    {
        _decayCoroutineIsRunning = true;
        _decayStartDelayTimer = _decayStartDelay;
        while (_decayStartDelayTimer > 0f)
        {
            _decayStartDelayTimer -= Time.deltaTime;
            yield return null;
        }
        float initialDecayValue = _decaySlider.value;
        float lerpTimer = 0f;
        _endDecayValue = _resourceSlider.value;
        while (_decaySlider.value > _endDecayValue)
        {
            _endDecayValue = _resourceSlider.value;
            _decaySlider.value = Mathf.Lerp(initialDecayValue, _endDecayValue, lerpTimer / _decayDuration);
            lerpTimer += Time.deltaTime;
            yield return null;
        }
        yield return null;
        _decayCoroutineIsRunning = false;
    }
}
