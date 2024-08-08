using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;

    public void changeVolume(float volume)
    {
        volumeText.text = (volume*100).ToString("0.");
    }

    public void changeSfx(float volume)
    {
        sfxText.text = (volume * 100).ToString("0.");
    }
}
