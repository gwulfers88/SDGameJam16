using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour
{
	public void BackButtonClicked()
	{
		Application.LoadLevel ("MainMenu");
	}
}
