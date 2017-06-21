// Created by Lyman

using UnityEngine;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Message")]
public class UIButtonCustomMessage : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
	}

	public GameObject target;
	public string functionName;
	public GameObject parameter;
	public Trigger trigger = Trigger.OnClick;

	void OnHover (bool isOver)
	{
		if (((isOver && trigger == Trigger.OnMouseOver) ||
			(!isOver && trigger == Trigger.OnMouseOut))) Send();
	}

	void OnPress (bool isPressed)
	{
		if (((isPressed && trigger == Trigger.OnPress) ||
			(!isPressed && trigger == Trigger.OnRelease))) Send();
	}

	void OnClick ()
	{
		if (trigger == Trigger.OnClick) Send();
	}

	void Send ()
	{
		if (!enabled || !gameObject.activeInHierarchy || string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;

		target.SendMessage(functionName, parameter, SendMessageOptions.DontRequireReceiver);
	}
}