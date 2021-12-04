using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndAccuracy : MonoBehaviour
{
    public TextMeshProUGUI AccuracyText;
    public TextMeshProUGUI HealthText;
    public Image BlackBarFill;
    public Image RedBarFill;


    public void SetHealth(int tmpHealth, int maxHealth)
    {
        RedBarFill.fillAmount = tmpHealth * 1f / maxHealth * 1f;
        HealthText.text = $"{maxHealth}/{tmpHealth}";
    }
    public void SetAccuracy(string accuracy)
    {
        AccuracyText.text = accuracy;
    }
}
