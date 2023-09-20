using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    // A generic script used to update a slider's value, mostly used for numeric resources like Health. ResourceBar is assigned to various events.
    [SerializeField] private Slider _resourceSlider;

    public void UpdateSliderValue(float value) => _resourceSlider.value = Mathf.Clamp01(value);
}
