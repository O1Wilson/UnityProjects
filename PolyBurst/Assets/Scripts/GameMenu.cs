using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        SceneManager.LoadScene("Level");
    }
    public void PlaySettings ()
    {
        SceneManager.LoadScene("Settings");
    }

    public void PlayHighScores ()
    {
        SceneManager.LoadScene("HighScores");
    }

}
