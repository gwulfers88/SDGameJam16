using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    AudioSource _ehehe;
    float timer = 3f;
    void Start()
    {
        _ehehe = GetComponent<AudioSource>();
    }
	public void OnMultiplayerClicked()
	{
        _ehehe.Play();
        SceneManager.LoadScene("CharacterSelection");
	}

	public void OnOptionsClicked()
	{
        _ehehe.Play();
        SceneManager.LoadScene("Options");
        
	}

    public void LeaderboardClicked()
    {
        _ehehe.Play();

        SceneManager.LoadScene("Leaderboard");
        
       
    }
    public void CreditsClicked()
    {
        _ehehe.Play();
        SceneManager.LoadScene("Credits");
    }
}

    