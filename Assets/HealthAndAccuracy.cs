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
    public Image ActionOne;
    public Image ActionTwo;

    public void SetAction(int leftActions)
    {
        if (leftActions == 2)
        {
            ActionOne.gameObject.SetActive(true);
            ActionTwo.gameObject.SetActive(true);
        } 
        else if (leftActions == 1)
        {
            ActionOne.gameObject.SetActive(true);
            ActionTwo.gameObject.SetActive(false);
        }
        else
        {
            ActionOne.gameObject.SetActive(false);
            ActionTwo.gameObject.SetActive(false);
        }
    }

    public void On(bool on)
    {
        AccuracyText.gameObject.SetActive(on);
        HealthText.gameObject.SetActive(on);
        BlackBarFill.gameObject.SetActive(on);
        RedBarFill.gameObject.SetActive(on);
    }

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
