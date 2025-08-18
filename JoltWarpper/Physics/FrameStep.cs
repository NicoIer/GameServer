using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityToolkit;

namespace GameCore.Physics
{
    public struct FrameInput
    {
    }

    public class FrameInputs
    {
        private Dictionary<uint, FrameInput> inputs { get; set; }
        public int Count => inputs.Count;

        public FrameInputs()
        {
            inputs = new Dictionary<uint, FrameInput>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            inputs.Clear();
        }

        public FrameInput this[uint id]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inputs[id];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => inputs[id] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(uint id) => inputs.ContainsKey(id);
    }

    public class FrameStep
    {
        public int currentFrame { get; private set; }

        public readonly Queue<FrameInputs> histroy;
        public readonly CircularBuffer<FrameInputs> future;
        public FrameInputs current { get; private set; }

        public readonly int bufferSize;

        public FrameStep(int bufferSize)
        {
            this.bufferSize = bufferSize;
            currentFrame = 0;
            current = new FrameInputs();
            histroy = new Queue<FrameInputs>(bufferSize);
            future = new CircularBuffer<FrameInputs>(bufferSize);
        }

        public FrameInputs Step()
        {
            histroy.Enqueue(current);
            while (histroy.Count > bufferSize)
            {
                histroy.Dequeue();
            }

            var result = current;
            if (future.Size > 0)
            {
                current = future.Front();
                future.PopFront();
            }
            else
            {
                current = new FrameInputs();
            }

            ++currentFrame;
            return result;
        }

        public bool Accept(uint id, int frame, FrameInput input)
        {
            // 在输入当前帧
            if (frame == currentFrame)
            {
                // 拒绝一帧的反复输入
                if (current.Contains(id)) return false;
                current[id] = input;
                return true;
            }

            // 在输入历史帧
            if (frame < currentFrame)
            {
                return false;
            }


            // 在输入未来帧
            if (frame > currentFrame)
            {
                int index = frame - currentFrame;
                if (index > bufferSize) return false; // 这个输入实在是太未来了
                while (future.Size < index)
                {
                    future.PushBack(new FrameInputs());
                }

                var inputs = future[index - 1];
                if (inputs.Contains(id)) return false;
                inputs[id] = input;
                return true;
            }

            return false;
        }
    }
}