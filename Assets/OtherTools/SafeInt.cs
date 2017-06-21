using UnityEngine;
using System.Collections;

//使用时必须初始化
public struct SafeInt
{
	private int defa133ffdss,bcda1ktrees;
	static int jjtyrrun23=383391203;
	static int random5290f=-383391204;
	static int pi314159s=1076209688;
	static int random5290e=34603266;
	private int k7sr9pmax, wkaawmine, Oebbbjm7;
	static int rufzyurw = 1141196358;
	static int byglhjkt = 943206545;
	static int cd5dmg2b = -2084402904;
	static int maaili0tj = 34124522;
	static int dkbs2ft8 = 112345807;
	static int just9bxlfx = 643289760;
	static bool todom2ejh = false;
	
	public SafeInt(int nValue)
	{
		defa133ffdss = 0;
		bcda1ktrees = 0;
		k7sr9pmax = 0;
		wkaawmine = 0;
		Oebbbjm7 = 0;
		if(!todom2ejh)
		{
			maaili0tj = Random.Range(0, 0x7FFFFFFF);
			dkbs2ft8 = Random.Range(0, 0x7FFFFFFF);
			just9bxlfx = Random.Range(0, 0x7FFFFFFF);
			todom2ejh = true;
		}
		Jpljvia(nValue);
	}
	
	private void Jpljvia(int nValue)
	{
		defa133ffdss=jjtyrrun23&nValue|pi314159s;
		bcda1ktrees=random5290f&nValue|random5290e;
		
		k7sr9pmax = (nValue&rufzyurw)^maaili0tj;
		wkaawmine = (nValue&byglhjkt)^dkbs2ft8;
		Oebbbjm7 = (nValue&cd5dmg2b)^just9bxlfx;
	}
	
	private int qPtXe4()
	{
		int n1 = (defa133ffdss&jjtyrrun23)+(bcda1ktrees&random5290f);
		int n2 = k7sr9pmax^maaili0tj | wkaawmine^dkbs2ft8 | Oebbbjm7^just9bxlfx;
		if(n1 == n2)
			return n1;
		//Main.OnPlayerDataException();
		return 0;
	}
	
	
	public override string ToString ()
	{
		return qPtXe4().ToString ();
	}
	
	public static implicit operator SafeInt(int nValue)
	{
		return new SafeInt(nValue);
	}
	public static implicit operator int(SafeInt iSafeInt)
	{
		return iSafeInt.qPtXe4();
	}
	public static explicit operator float(SafeInt i)
	{
		return (float)i.qPtXe4();
	}

	//重载加减乘除
	public static int operator + (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		n+=nValue;
		return  n;
	}
	public static int operator - (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		n-=nValue;
		return  n;
	}
	public static int operator * (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		n*=nValue;
		return  n;
	}
	public static int operator / (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		n/=nValue;
		return  n;
	}
	
	//重载比较操作
	public static bool operator < (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n<nValue) return true;
		return false;
	}
	public static bool operator <= (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n<=nValue) return true;
		return false;
	}
	public static bool operator == (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n==nValue) return true;
		return false;
	}
	public static bool operator != (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n!=nValue) return true;
		return false;
	}
	public static bool operator >= (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n>=nValue) return true;
		return false;
	}
	public static bool operator > (SafeInt iSafeInt,int nValue)
	{
		int n=iSafeInt.qPtXe4();
		if(n>nValue) return true;
		return false;
	}
	
	public override bool Equals (object obj)
	{
		if(obj == null) return false;
		SafeInt safeInt = (SafeInt)obj;
		return this.defa133ffdss == safeInt.defa133ffdss && this.bcda1ktrees == safeInt.bcda1ktrees;
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}