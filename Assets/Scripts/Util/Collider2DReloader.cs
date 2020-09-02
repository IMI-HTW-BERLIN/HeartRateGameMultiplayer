using UnityEditor;
using UnityEngine;

namespace Util
{
    public class Collider2DReloader : EditorWindow
    {
        [MenuItem("Tools/Reload all Collider2Ds")]
        private static void ReloadAllColliders2D()
        {
            foreach (Object o in FindObjectsOfType(typeof(Collider2D)))
            {
                Behaviour behaviour = (Behaviour) o;
                behaviour.enabled = false;
                behaviour.enabled = true;
            }
        }
    }
}