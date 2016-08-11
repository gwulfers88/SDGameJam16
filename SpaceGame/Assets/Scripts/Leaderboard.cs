using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Leaderboard : MonoBehaviour
{
    public void BackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
