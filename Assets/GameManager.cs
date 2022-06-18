using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    bool is_not_pause = true;

    [SerializeField]
    GameObject pause_panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            pause_panel.SetActive(is_not_pause);

            is_not_pause = !is_not_pause;

            Time.timeScale = System.Convert.ToInt32(is_not_pause);
        }
    }

    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
