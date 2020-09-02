using UnityEditor;
using UnityEngine;

namespace Util.Editor
{
    [CustomEditor(typeof(ScreenTransform))]
    public class ScreenTransformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            ScreenTransform screenTransform = (ScreenTransform) target;
            
            if(GUILayout.Button("Update"))
                screenTransform.ManualUpdate();
            
            if(GUILayout.Button("Force Update Component"))
                screenTransform.ForceUpdate();
        }
    }
}