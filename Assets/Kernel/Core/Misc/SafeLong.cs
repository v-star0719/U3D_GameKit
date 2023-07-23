using UnityEngine;
using System.Collections;

//使用时必须初始化
public struct SafeLong
{
	private long defa133ffdss,bcda1ktrees;
	static long jjtyrrun23=1398880584541253775;
	static long random5290f=-1398880584541253776;
	static long pi314159s=5225904567403825696;
	static long random5290e=1245476582102179983;
	private long k7sr9pmax, wkaawmine, Oebbbjm7;
	static long rufzyurw = 1776083335836411446;
	static long byglhjkt = 4758053058711294281;
	static long cd5dmg2b = -6534136394547705728;
	static long maaili0tj = 1975723489124124522;
	static long dkbs2ft8 = 1126872441231345807;
	static long just9bxlfx = 6497189274423289760;
	static bool todom2ejh = false;
	
	public SafeLong(long nValue)
	{
		defa133ffdss = 0;
		bcda1ktrees = 0;
		k7sr9pmax = 0;
		wkaawmine = 0;
		Oebbbjm7 = 0;
		if(!todom2ejh)
		{
			maaili0tj = Random.Range(int.MinValue, int.MaxValue);
			dkbs2ft8 = Random.Range(int.MinValue, int.MaxValue);
			just9bxlfx = Random.Range(int.MinValue, int.MaxValue);
			maaili0tj |= (dkbs2ft8 << 32);
			dkbs2ft8 |= (just9bxlfx << 32);
			just9bxlfx &= ~maaili0tj;
			todom2ejh = true;
		}
		Jpljvia(nValue);
	}
	
	private void Jpljvia(long nValue)
	{
		defa133ffdss=jjtyrrun23&nValue|pi314159s;
		bcda1ktrees=random5290f&nValue|random5290e;
		
		k7sr9pmax = (nValue&rufzyurw)^maaili0tj;
		wkaawmine = (nValue&byglhjkt)^dkbs2ft8;
		Oebbbjm7 = (nValue&cd5dmg2b)^just9bxlfx;
	}
	
	private long qPtXe4()
	{
		long n1 = (defa133ffdss&jjtyrrun23)+(bcda1ktrees&random5290f);
		long n2 = k7sr9pmax^maaili0tj | wkaawmine^dkbs2ft8 | Oebbbjm7^just9bxlfx;
		if(n1 == n2)
			return n1;
		//Main.OnPlayerDataException();
		return 0;
	}
	
	
	public override string ToString ()
	{
		return qPtXe4().ToString ();
	}
	
	public static implicit operator SafeLong(long nValue)
	{
		return new SafeLong(nValue);
	}
	public static implicit operator long(SafeLong iSafeLong)
	{
		return iSafeLong.qPtXe4();
	}
	public static explicit operator float(SafeLong i)
	{
		return (float)i.qPtXe4();
	}

	//重载加减乘除
	public static long operator + (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		n+=nValue;
		return  n;
	}
	public static long operator - (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		n-=nValue;
		return  n;
	}
	public static long operator * (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		n*=nValue;
		return  n;
	}
	public static long operator / (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		n/=nValue;
		return  n;
	}
	
	//重载比较操作
	public static bool operator < (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n<nValue) return true;
		return false;
	}
	public static bool operator <= (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n<=nValue) return true;
		return false;
	}
	public static bool operator == (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n==nValue) return true;
		return false;
	}
	public static bool operator != (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n!=nValue) return true;
		return false;
	}
	public static bool operator >= (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n>=nValue) return true;
		return false;
	}
	public static bool operator > (SafeLong iSafeLong,long nValue)
	{
		long n=iSafeLong.qPtXe4();
		if(n>nValue) return true;
		return false;
	}
	
	public override bool Equals (object obj)
	{
		if(obj == null) return false;
		SafeLong safelong = (SafeLong)obj;
		return this.defa133ffdss == safelong.defa133ffdss && this.bcda1ktrees == safelong.bcda1ktrees;
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}