using System;
using System.Collections.Generic;
using UnityEngine;
using Network;
using UnityToolkit;

namespace Soccer
{
    public class PhysicsView : MonoBehaviour
    {
        [Sirenix.OdinInspector.ShowInInspector]
        // private SortedList<float, PhysicsData> remoteDataList = new SortedList<float, PhysicsData>();

        // 假设PhysicsData有position、rotation、linearVelocity、angularVelocity
        public Vector3 targetPos;

        public bool lerpToTarget = true;
        public bool needLerp;

        public void EnqueuePosAndRot(in PhysicsData data)
        {
            transform.rotation = data.rotation.T();
            if (!lerpToTarget)
            {
                transform.position = data.position.T();
                return;
            }

            if (Vector3.Distance(data.position.T(), transform.position) > 0.1f)
            {
                targetPos = data.position.T();
                needLerp = true;
            }
            else // 差距很小 没必要做插值
            {
                transform.position = data.position.T();
            }
        }

        private void Update()
        {
            if (!lerpToTarget) return;
            if (!needLerp) return;
            float distance = Vector3.Distance(targetPos, transform.position);
            // double rtt = NetworkTime.Singleton.rttMs; // 转换为秒
            if (distance > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10);
            }
            else
            {
                needLerp = false;
                transform.position = targetPos;
            }
        }

        //
        // private void Update()
        // {
        //     double rttMs = NetworkTime.Singleton.rttMs;
        //     float now = Time.realtimeSinceStartup;
        //     while (true)
        //     {
        //         if (remoteDataList.Count == 0)
        //             break;
        //         var receivedTime = remoteDataList.Keys[0];
        //         var data = remoteDataList[receivedTime];
        //         float distance = Vector3.Distance(data.position.T(), transform.position);
        //         // 差距很小了 直接应用
        //         if (distance < 0.05f)
        //         {
        //             transform.position = data.position.T();
        //             transform.rotation = data.rotation.T();
        //             remoteDataList.RemoveAt(0);
        //             break;
        //         }
        //
        //         // 如果距离太远了 直接应用
        //         if (distance > 10)
        //         {
        //             transform.position = data.position.T();
        //             transform.rotation = data.rotation.T();
        //             remoteDataList.RemoveAt(0);
        //             break;
        //         }
        //
        //
        //         // 计算时间差 秒为单位
        //         float delta = now - receivedTime + (float)(rttMs / 1000.0) / 2.0f; // 距离服务器这个坐标的时间距离
        //         var t = delta;
        //         transform.position =
        //             Vector3.Lerp(transform.position, data.position.T(), t);
        //         transform.rotation =
        //             Quaternion.Slerp(transform.rotation, data.rotation.T(), t);
        //
        //
        //         break;
        //     }
        // }
    }
}