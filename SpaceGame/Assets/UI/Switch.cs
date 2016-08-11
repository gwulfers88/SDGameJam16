using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Switch : MonoBehaviour
{

	public void Click()
    {
         SceneManager.LoadScene("MainMenu");
    }
}
