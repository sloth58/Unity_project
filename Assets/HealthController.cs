using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HealthController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Slider HealthBar;

    [SerializeField]
    private TMPro.TMP_Text CoinText;

    [SerializeField]
    private GameObject player;


    [SerializeField]
    private GameObject GameOverPanel;

    int CurrentScore;

    public void OnCollected()
    {
        CurrentScore++;
        CoinText.text = "Score: " + CurrentScore.ToString();
    }

    public void OnHit()
    {
        HealthBar.value -= 1;

        if(HealthBar.value == 0)
        {
            GameOverPanel.SetActive(true);
            Destroy(player);
        }

    }

    public void OnDeath()
    {
        HealthBar.value = 0;
        GameOverPanel.SetActive(true);
        Destroy(player);
    }
}
