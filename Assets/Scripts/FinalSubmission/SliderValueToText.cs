using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float defaultValue;

    void Start()
    {
        if (defaultValue < slider.minValue)
        {
            defaultValue = slider.minValue;
            slider.value = defaultValue;
        }

        else if (defaultValue > slider.maxValue)
        {
            defaultValue = slider.maxValue;
            slider.value = defaultValue;
        }
        else
            slider.value = defaultValue;

        if (slider.wholeNumbers)
        {
            slider.onValueChanged.AddListener((v) => 
            { 
                valueText.text = v.ToString("0");
            });

            valueText.text = slider.value.ToString("0");

            return;
        }

        slider.onValueChanged.AddListener((v) =>
        {
            valueText.text = v.ToString("0.00");
        });

        valueText.text = slider.value.ToString("0.00");

    }
}
