using UnityEditor;
using UnityEngine;

namespace Util.Editor
{
    [CustomEditor(typeof(UIScalerBoxCollider2D))]
    public class UIScalerBoxCollider2DEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Update"))
                ((UIScalerBoxCollider2D)target).ScaleBoxCollider();
        }
    }
}