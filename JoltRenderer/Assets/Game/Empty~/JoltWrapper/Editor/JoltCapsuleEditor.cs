using System;
using UnityEditor;

namespace JoltWrapper.Editor
{
    [CustomEditor(typeof(JoltCapsule)), CanEditMultipleObjects]
    public class JoltCapsuleEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var capsule = target as JoltCapsule;

            if (capsule == null)
            {
                throw new InvalidOperationException("Target is not a JoltCapsule.");
            }

            var position = capsule.body.position;
            var rotation = capsule.body.rotation;

            JoltHandles.DrawCapsuleShape(position, rotation, capsule);
        }
    }
}