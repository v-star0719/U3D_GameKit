using UnityEngine;
using System.Collections;

public class AnimationDelayPlay : MonoBehaviour
{
	public float delayTime;
	
	// Update is called once per frame
	void Update ()
	{
		if(delayTime <= 0)
		{
			Animation a = GetComponent<Animation>();
			a.Play();
			enabled = false;
		}
		delayTime -= Time.deltaTime;
	}
}
