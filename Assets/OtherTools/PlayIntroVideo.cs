using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayIntroVideo : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		VideoPlayer.PlayIntroVideo(OnIntroVideoPlayEnd);
	}

	void OnIntroVideoPlayEnd()
	{
		//Application.LoadLevel("Game");
		SceneManager.LoadScene("Game");
	}
}
