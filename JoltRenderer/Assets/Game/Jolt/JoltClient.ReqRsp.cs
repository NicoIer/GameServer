using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCore.Jolt;
using MemoryPack;
using Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game.Jolt
{
    public partial class JoltClient
    {
        private void AwakeReqRsp()
        {
            _client.messageHandler.Add<RspHead>(OnRspHead);
        }

        private void OnRspHead(in RspHead message)
        {
            _response[message.index] = message;
        }

        private readonly Dictionary<ushort, ReqHead> _requesting = new Dictionary<ushort, ReqHead>();

        private readonly Dictionary<ushort, RspHead> _response = new Dictionary<ushort, RspHead>();

        private readonly List<ushort> _idPool = new List<ushort>(16);
        private ushort _currentId = 0;

        private async UniTask<RspHead> Request(ReqHead reqHead, float timeout = 1f)
        {
            _client.Send(reqHead);
            _requesting[reqHead.index] = reqHead;
            float startedTime = Time.realtimeSinceStartup;
            await UniTask.WaitUntil(() =>
                _response.ContainsKey(reqHead.index) || Time.realtimeSinceStartup - startedTime > timeout);
            bool outed = !_response.ContainsKey(reqHead.index);
            if (outed)
            {
                return new RspHead(reqHead.index, reqHead.reqHash, 0, ErrorCode.Timeout, null, default);
            }

            bool success = _response.Remove(reqHead.index, out var head);
            _idPool.Add(reqHead.index);
            Assert.IsTrue(success);
            return head;
        }


        private async Task<(TRsp, bool)> Request<TReq, TRsp>(TReq req, float timeout = 1f)
            where TReq : INetworkReq where TRsp : INetworkRsp
        {
            if (_idPool.Count == 0)
            {
                _idPool.Add(_currentId++);
            }

            var reqIndex = _idPool[^1];
            //https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs O(1) Remove
            _idPool.RemoveAt(_idPool.Count - 1);

            ushort reqHash = TypeId<TReq>.stableId16;
            var reqHead = new ReqHead
            {
                reqHash = reqHash,
                index = reqIndex,
                payload = MemoryPackSerializer.Serialize(req),
            };

            var rspHead = await Request(reqHead, timeout);
            if (rspHead.error != ErrorCode.Success)
            {
                ToolkitLog.Warning($"收到了一个发生错误的响应:{rspHead.error},{rspHead.errorMessage}");
                return (default, false);
            }

            if (!_requesting.TryGetValue(rspHead.index, out var value))
            {
                ToolkitLog.Warning($"收到了一个未请求的响应:{rspHead}");
                return (default, false);
            }

            return (MemoryPackSerializer.Deserialize<TRsp>(rspHead.payload), true);
        }


        [Sirenix.OdinInspector.Button]
        public async void ReqBodyInfo(uint bodyId)
        {
            var req = new ReqBodyInfo(bodyId);
            var (rsp, success) = await Request<ReqBodyInfo, RspBodyInfo>(req, 1f);
            if (success)
            {
                ToolkitLog.Info($"收到BodyInfo响应:{rsp}");
            }
        }
    }
}