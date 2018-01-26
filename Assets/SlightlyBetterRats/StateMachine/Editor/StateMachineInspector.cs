using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SBR {
    [CustomEditor(typeof(StateMachineDefinition))]
    public class StateMachineInspector : Editor {
        public override void OnInspectorGUI() {
            StateMachineDefinition myTarget = (StateMachineDefinition)target;

            if (GUILayout.Button("Open State Machine Editor")) {
                StateMachineEditorWindow.def = myTarget;
                StateMachineEditorWindow.ShowWindow();
            }

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "baseClass");
            serializedObject.ApplyModifiedProperties();
        }
    }
}