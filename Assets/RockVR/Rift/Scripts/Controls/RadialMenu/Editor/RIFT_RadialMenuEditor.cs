using UnityEngine;
using UnityEditor;

namespace RockVR.Rift.Editor
{
    [CustomEditor(typeof(RIFT_RadialMenu))]
    public class RIFT_RadialMenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RIFT_RadialMenu radialMenu = (RIFT_RadialMenu)target;
            if (GUILayout.Button("Regenerate Buttons"))
            {
                radialMenu.RegenerateButtons();
            }
        }
    }
}