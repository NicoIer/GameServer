using System;
using UnityEditor;

namespace JoltWrapper.Editor
{
    [CustomEditor(typeof(JoltSphere)), CanEditMultipleObjects]
    public class JoltSphereEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var shape = target as JoltSphere;
            var pos = shape.body.position;
            var rot = shape.body.rotation;
            JoltHandles.DrawSphereShape(pos, rot, shape);
        }
    }
}