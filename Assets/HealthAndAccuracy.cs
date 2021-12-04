using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndAccuracy : MonoBehaviour
{
    public TextMeshProUGUI AccuracyText;
    public Image BlackBarFill;
    public Image RedBarFill;


    public void SetHealth(int tmpHealth, int maxHealth)
    {
        RedBarFill.fillAmount = tmpHealth * 1f / maxHealth * 1f;
    }
    public void SetAccuracy(string accuracy)
    {
        AccuracyText.text = accuracy;
    }
}
