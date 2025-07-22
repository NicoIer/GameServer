using System;
using UnityEditor;
using UnityEngine.Assertions;

namespace JoltWrapper.Editor
{
    [CustomEditor(typeof(JoltBox)), CanEditMultipleObjects]
    public class JoltBoxEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var shape = target as JoltBox;
            Assert.IsNotNull(shape);
            var pos = shape.position;
            var rot = shape.rotation;
            JoltShapeHandles.DrawBoxShape(pos, rot, shape);
        }
    }
}