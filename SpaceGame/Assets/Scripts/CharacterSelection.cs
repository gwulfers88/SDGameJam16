using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour 
{
	public void BackButtonClicked()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
