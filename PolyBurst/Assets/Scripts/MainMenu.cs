using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoresText;
    public TextMeshProUGUI ScoreText;

    private void Start()
    {
        highScoresText.text = PlayerPrefs.GetInt("highscore").ToString();
        ScoreText.text = PlayerPrefs.GetInt("score").ToString() ;
    }

    public void Main_Menu ()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
