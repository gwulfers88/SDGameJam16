using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public void OnMultiplayerClicked()
	{
		Application.LoadLevel ("CharacterSelection");
	}

	public void OnOptionsClicked()
	{
		Application.LoadLevel ("Options");
	}

    public void LeaderboardClicked()
    {
        Application.LoadLevel("Leaderboard");
    }
    public void CreditsClicked()
    {
        Application.LoadLevel("Credits");
    }
}
