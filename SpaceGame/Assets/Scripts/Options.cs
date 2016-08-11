using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
	public void BackButtonClicked()
	{
		SceneManager.LoadScene("MainMenu");
	}

    public void OnBrightnessSlider(float value)
    {

    }

    public void FullscreenToggle(bool value)
    {
        Screen.fullScreen = value;
    }
}
