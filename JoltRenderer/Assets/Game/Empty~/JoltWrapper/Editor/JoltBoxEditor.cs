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
            var pos = shape.body.position;
            var rot = shape.body.rotation;
            JoltHandles.DrawBoxShape(pos, rot, shape);
        }
    }
}