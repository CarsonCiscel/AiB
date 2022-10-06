using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathBar : MonoBehaviour
{


    public Slider slider; 
    public Gradient gradient;
    public Image fill;

    public void SetMaxBreath(float breath)
    {
        slider.maxValue = breath;
        slider.value = breath;

      fill.color = gradient.Evaluate(1f);
    }

    public void SetBreath(float breath)
    {
        slider.value = breath;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}