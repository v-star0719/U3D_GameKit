using System;
using UnityEngine;
using System.Collections;
namespace GameKit
{

public class CustomVideoPlayer : MonoBehaviour
{
	const string introVedioPath = "Videos/IntroVideo.mp4"; 
	const int MIN_PLAY_FRAME_COUNT = 5;//由于播放时暂停了游戏，恢复时帧数+1。保险起见这里用5

	static bool isPlaying = false;
	static int  playStartFrame = 0;
	static Action playEndCallback;

	//播放时游戏会暂停，播放完成后恢复，update中发出播放完成的消息
	void Update()
	{
		if(isPlaying)
		{
			if(Time.frameCount - playStartFrame >= MIN_PLAY_FRAME_COUNT)
			{
				//播放完成
				PlayerPrefs.SetInt("isIntroVideoPlayed", 1);
				PlayerPrefs.Save();
				isPlaying = false;
				//Debug.Log("Play finished, time = " + (Time.realtimeSinceStartup - playStartTime));
				if(playEndCallback != null) playEndCallback();
			}
		}
	}

	///播放开场动画
	public static void PlayIntroVideo(Action _playEndCallback)
	{
		if(isPlaying)
		{
			Debug.LogError("a video is playing");
			return;
		}

		playEndCallback = _playEndCallback;
		if(PlayerPrefs.GetInt("isIntroVideoPlayed", 0) != 1)
		{
			#if !UNITY_STANDALONE
			//没有播放过，播放
			Handheld.PlayFullScreenMovie(introVedioPath, Color.black, FullScreenMovieControlMode.Hidden);
			#endif
			isPlaying = true;
			playStartFrame = Time.frameCount;
			//Debug.Log("start play video");
		}
		else
		{
			//曾经播放过了，不播放
			if(playEndCallback != null) playEndCallback();
			//Debug.Log("the inro video has played");
		}
	}
}
}