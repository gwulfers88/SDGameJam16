using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour 
{
	public void BackButtonClicked()
	{
		Application.LoadLevel ("MainMenu");
	}
}
