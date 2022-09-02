﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit
{
    //给子弹施加的速度偏移，实现各种神奇的弹道。这个偏移量始终垂直目标方向，或者当前速度方向
    [System.Serializable]
    public class VelocityOffsetData
    {
        public VelocityOffsetType offsetType;
        public float delay;
        public float duration = 1;
        public float offsetStart = 1;
        public float offsetEnd = 0;
    }

    public enum VelocityOffsetType
    {
        ToTargetDir,
        ToVelocity,
    }

    public class BulletTrajectory : MonoBehaviour
    {
        public Transform start;
        public Transform target;
        public float angel = 15f;
        public float speed = 12;
        public float angle = 60;
        public Transform vec;
        public Transform vecOffset;

        public VelocityOffsetData[] offsets = new VelocityOffsetData[0];

        private float velocityOffsetValue = 1;
        private bool isArrived = false;

        private int curOffsetIndex = 0;
        private float offsetTimer = 0;
        private VelocityOffsetData curOffsetData;
        private bool isOffsetValueApplied = false;
        private Vector3 curVelocity;

        // Use this for initialization
        void Start()
        {
            StartFly();
        }

        // Update is called once per frame
        void Update()
        {
            if(isArrived)
            {
                StartFly();
                return;
            }

            //确定偏移方向和主方向
            Vector3 offsetDir = Vector3.zero;
            Vector3 mainDir;
            if(curOffsetData != null && curOffsetData.offsetType == VelocityOffsetType.ToVelocity)
            {
                //始终垂直当前速度方向
                mainDir = curVelocity;
            }
            else
            {
                mainDir = target.position - transform.position;
            }

            //垂直于主方向的一个方向。在垂直面的圆上取一个点。先在x,z平面上算，然后旋转到位
            if(Mathf.Abs(velocityOffsetValue) > 0.0001)
            {
                float angle = this.angle * Mathf.Deg2Rad;
                offsetDir.x = Mathf.Cos(angle);
                offsetDir.z = Mathf.Sin(angle);
                var qua = Quaternion.FromToRotation(Vector3.up, mainDir);
                offsetDir = qua * offsetDir;
            }

            //确定主方向的速度大小，主方向是一个分量
            //因为是垂直于原放下的分量，所以直接勾股定理算另一个分量的大小
            float mainValue = speed * speed - velocityOffsetValue * speed * velocityOffsetValue * speed;
            if(mainValue < 0)
            {
                mainValue = 0;
            }
            else
            {
                mainValue = Mathf.Sqrt(mainValue);
            }

            curVelocity = offsetDir * velocityOffsetValue * speed + mainValue * mainDir.normalized;

            Vector3 delta = curVelocity * Time.deltaTime;
            if(delta.sqrMagnitude >= mainDir.sqrMagnitude)
            {
                isArrived = true;
                delta = mainDir;
            }

            if(curOffsetIndex < offsets.Length)
            {
                if(offsetTimer >= curOffsetData.delay)
                {
                    if(!isOffsetValueApplied)
                    {
                        velocityOffsetValue = curOffsetData.offsetStart;
                        isOffsetValueApplied = true;
                    }

                    float t = offsetTimer - curOffsetData.delay;
                    velocityOffsetValue = Mathf.Lerp(curOffsetData.offsetStart, curOffsetData.offsetEnd, t / curOffsetData.duration);
                    if(t >= curOffsetData.duration)
                    {
                        //切换到下一段
                        curOffsetIndex++;
                        if(curOffsetIndex < offsets.Length)
                        {
                            isOffsetValueApplied = false;
                            curOffsetData = offsets[curOffsetIndex];
                        }
                        else
                        {
                            curOffsetData = null;
                            velocityOffsetValue = 0;
                        }
                    }
                }

                offsetTimer += Time.deltaTime;
            }

            transform.LookAt(transform.position + delta);
            transform.position += delta;

            if(vec != null)
            {
                vec.rotation = Quaternion.LookRotation(curVelocity);
            }

            if(vecOffset != null)
            {
                vecOffset.rotation = Quaternion.LookRotation(offsetDir);
            }
        }

        void StartFly()
        {
            //angle = Random.Range(0, 360) * Mathf.Rad2Deg;
            isArrived = false;
            transform.position = start.position;
            transform.LookAt(target.position);

            Vector3 dir = target.position - transform.position;
            dir = dir.normalized;

            velocityOffsetValue = 1;
            curOffsetIndex = 0;
            offsetTimer = 0;
            isOffsetValueApplied = false;

            //执行第一段
            if(offsets.Length > 0)
            {
                curOffsetData = offsets[0];
            }
            else
            {
                curOffsetData = null;
            }
        }
    }
}
