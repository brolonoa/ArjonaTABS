using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SoundManager.VolumeKey, 1f));
        UpdateText(slider.value);
        slider.onValueChanged.AddListener(SetVolume);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(SetVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(SoundManager.VolumeKey, value);
        UpdateText(value);
    }

    private void UpdateText(float value)
    {
        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
}
