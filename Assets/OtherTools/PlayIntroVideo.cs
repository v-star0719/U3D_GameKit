using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Collections;

namespace GameKit
{
public class PlayIntroVideo : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		CustomVideoPlayer.PlayIntroVideo(OnIntroVideoPlayEnd);
	}

	void OnIntroVideoPlayEnd()
	{
		Application.LoadLevel("Game");
		//SceneManager.LoadScene("Game");
	}
}
}