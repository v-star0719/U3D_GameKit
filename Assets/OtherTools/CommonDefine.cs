using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void VoidDelegate();

public class CommonDefine
{
	/** 装备洗练的晋级上限系数*/
	public const float Equip_Refine_LevelUp_Param = 0.2f;

    /// <summary>
    /// 判断float == 0 的比较系数
    /// </summary>
    public const float PRECISION = 0.000001f;

    public const int PLAYERCTRL_MASTER_TOUCH = 1;
    public const int PLAYERCTRL_MASTER_JOYSTICK = 2;

    /** 铜钱奖励*/
    public const int GOLD_REWARD_ICONID = 9000502;
    /** 元宝奖励*/
    public const int DIAMOND_REWARD_ICONID = 9000501;
    /** Exp奖励*/
    public const int EXP_REWARD_ICONID = 9000501;
    /** 体力 */
	public const int PHYSICAL_ICONID = 9000506;
	/** 通天副本积分*/
	public const int ID_WARHIGHESTSCORE_ICON = 9009169;

    public const int IMAGE_ID_SECRET = 9020000;
	public const int IMAGE_ID_NOBEAST = 8200100;

	public const int IMAGE_ID_JIESHAO = 9000046;//t_npc_zixia
	public const int IMAGE_ID_ZHONGDIAN = 9000046;//t_npc_zhongdian
	public const int IMAGE_ID_YUTUJING = 9000046;//t_npc_yutujing
	public const int IMAGE_ID_TAIBAI = 9000046;//t_npc_taibai
	public const int IMAGE_ID_SUNWUKONG = 9000046;//t_npc_sunwukong
	public const int IMAGE_ID_QIXIANNV = 9000046;//t_npc_qixiannv


    public const int FEEDBACK_SUCCESS = 1000;

	/** 铜钱名字的多语言key*/
    public const string GOLD_NAME = "coinName1";
	/** 元宝名字的多语言key*/
    public const string DIAMOND_NAME = "coinName2";
	/** 经验名字的多语言key*/
    public const string EXP_NAME = "coinName4";

	/** 铜钱描述的多语言key*/
    public const string GOLD_DES = "coinType1";
	/** 元宝描述的多语言key*/
    public const string DIAMOND_DES = "coinType2";
	/** 经验描述的多语言key*/
    public const string EXP_DES = "coinType4";

	public const int EFFECT_HIT = 1;      					// 受伤光效
	public const int EFFECT_HEAL = 2;      					// 治疗光效
	public const int EFFECT_FUHUO = 6;						// 释放技能聚光效果
	public const int EFFECT_PVP_SHENSHOU_BUFF = 8;			// PVP 神兽buff光效
	public const int EFFECT_WARNING = 10;	     			// 怪物施法，警告圈
	public const int EFFECT_PATH_LEFT = 11;     			// 路径脚印，左
	public const int EFFECT_PATH_RIGHT = 12;   				// 路径脚印，右
	public const int EFFECT_HERO_BIRTH = 34;      			// 英雄出生光效
	public const int EFFECT_HERO_DEATH = 36;      			// 英雄死亡光效
	public const int EFFECT_BATTLE_START = 92;				// 战斗开始
	public const int EFFECT_BOSS_APPEAR = 50000;			// BOSS 出现
	public const int EFFECT_PVE_BAOQI = 50001;				// 释放技能聚光效果
	public const int EFFECT_GRAVEFIRE = 50002;    			// 鬼火光效
	public const int EFFECT_PVP_BAOQI = 50003;              // 释放技能聚光效果
    public const int EFFECT_PVP_DISAPEAR = 51000;              // PVP闪现消失
    public const int EFFECT_PVP_APEAR = 51001;              // PVP闪现出现的
    public const int EFFECT_PVP_SECONDTEAM_APEAR = 40568;        //PVP第二队上场特效

    public const int EFFECT_BOSS_SMOKE = 60003;				// BOSS黑烟
	public const int EFFECT_PORTAL_ALWAYS = 70051;
	public const int EFFECT_PORTAL_AWHILE = 70052;
	public const int EFFECT_PORTAL_FLASH = 70053;
	public const int EFFECT_ZHONGDIAN_HURT = 70021;
	public const int EFFECT_ZHONGDIAN_SIWANG = 70022;
	public const int EFFECT_ZHONGDIAN_LIGHTNING = 70023;

	public const int EFFECT_TOWER_NO_HERO = 70061;      		// 英雄出生光效
	public const int EFFECT_TOWER_NO_HERO_SELECT = 70062;		// 塔无英雄
	public const int EFFECT_TOWER_FLAG = 70063;					// 塔位集结点
	public const int EFFECT_MELEE_HERO_SELECT = 70067;			// 近战英雄塔位被选中

    public static int[] RankingNum = { 0, 9003003, 9003004, 9003005 };
	public static int[] RankingNumIcon = { 0, 9003009, 9003010, 9003011 };

    public const int CALCULATE_DENOMINATOR = 10000;

	public const long CSHARP_1970_TIME = 621355968000000000;	//C#中1970年的时间，用于处理java时间戳 
    public const long RefreshTimeOneDay = 18000000;    //每日刷新的时间毫秒数 凌晨5点 = 5*60*60*1000

    public const int RetrieveModelId = 50012;

	public static Color insufficientColor = new Color(1f, 0.12f, 0f);//量不足的颜色
	public static Color sufficientColor = new Color(0.37f, 1f, 0.1f);//量足的颜色
	public const string insufficientColorString = "[ff1f00]";
	public const string sufficientColorString = "[5eff19]";

    // 获取实心边框
    public static int GetSolidQualityFrameByID(int index)
    {
        switch (index)
        {
            case 1:
                return 9001111; //白
            case 2:
                return 9001112; //绿
            case 3:
                return 9001113; //蓝
            case 4:
                return 9001114; //紫
            case 5:
                return 9001115; //橙
        }
        return 0;
    }

    // 获取空心边框
    public static int GetHollowQualityPicByID(int index)
    {
        switch (index)
        {
            case 1:
                return 9001101; //白
            case 2:
                return 9001102; //绿
            case 3:
                return 9001103; //蓝
            case 4:
                return 9001104; //紫
            case 5:
                return 9001105; //橙
        }
        return 0;
    }
    
    
    public delegate void voidfunc();
}

