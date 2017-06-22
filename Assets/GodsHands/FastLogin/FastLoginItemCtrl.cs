using UnityEngine;
using System.Collections;

public class FastLoginItemCtrl : MonoBehaviour
{
#if UNITY_EDITOR
	public UILabel labelName;
	public int index;
	public FastLoginPanel panel;

	public void SetData(int index, string accountName)
	{
		this.index = index;
		labelName.text = accountName;
	}


	public void OnClick()
	{
		panel.OnAccountItemClicked(this);
	}

#endif
}
