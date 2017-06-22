using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trainer : MonoBehaviour
{
	public bool setAttack = false;
	public int attackValue = 3000;
	public bool setSkill1CD0 = false;
	public bool infiniteMana = false;

	void Update()
	{
		if(setAttack){
			SetAttack(attackValue);
			setAttack = false;
		}
		if(setSkill1CD0){
			SetSkillCDTime(0, 1);
			setSkill1CD0 = false;
		}
		if(infiniteMana){
			SetMana(10000, 10000);
			infiniteMana = false;
		}
	}

	void SetAttack(int value)
	{
		
	}

	//这个比较霸道，通过更改模板数据实现
	//t为时间，单位毫秒
	void SetSkillCDTime(int t, int skillID)
	{
		
	}

	void SetMana(int cur, int max)
	{
		
	}
}
