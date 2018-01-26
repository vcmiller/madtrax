using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace SBR {
    [CustomEditor(typeof(Brain))]
    public class BrainEditor : Editor {
        EditClassAndFieldsDrawer drawer;

        Texture2D arrowUp, arrowDown, delete, create;

        private void OnEnable() {
            arrowUp = Resources.Load<Texture2D>("ArrowUp");
            arrowDown = Resources.Load<Texture2D>("ArrowDown");
            delete = Resources.Load<Texture2D>("Delete");
            create = Resources.Load<Texture2D>("New");
        }

        public override void OnInspectorGUI() {
            Brain brain = target as Brain;

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "controllerPrefabs", "script");
            
            EditorGUILayout.LabelField("Controllers", EditorStyles.boldLabel);
            if (brain.controllerPrefabs == null) {
                brain.controllerPrefabs = new List<Controller>();
            }

            if (drawer == null) {
                drawer = new EditClassAndFieldsDrawer();
            }

            SerializedProperty prop = serializedObject.FindProperty("controllerPrefabs");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            float w = GUILayoutUtility.GetLastRect().width;
            Vector2 buttonSize = new Vector2(24, 24);
            bool dirty = false;

            for (int i = 0; i < brain.controllerPrefabs.Count; i++) {
                prop.Next(true);

                float h = drawer.GetEditorHeight(brain.controllerPrefabs[i], new GUIContent());

                if (h < 50) {
                    h = 50;
                }

                GUILayout.Space(h + 20);

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = w;

                EditorGUI.DrawRect(new Rect(rect.x, rect.y + 2, rect.width, rect.height - 4), new Color32(221, 221, 221, 255));


                if (i > 0 && GUI.Button(new Rect(rect.x + 6, rect.y + 6, buttonSize.x, buttonSize.y), new GUIContent(arrowUp))) {
                    var swap = brain.controllerPrefabs[i - 1];
                    brain.controllerPrefabs[i - 1] = brain.controllerPrefabs[i];
                    brain.controllerPrefabs[i] = swap;
                    dirty = true;
                }

                if (i < brain.controllerPrefabs.Count - 1 && GUI.Button(new Rect(rect.x + 6, rect.y + 8 + buttonSize.y, buttonSize.x, buttonSize.y), new GUIContent(arrowDown))) {
                    var swap = brain.controllerPrefabs[i + 1];
                    brain.controllerPrefabs[i + 1] = brain.controllerPrefabs[i];
                    brain.controllerPrefabs[i] = swap;
                    dirty = true;
                }

                if (GUI.Button(new Rect(rect.x + rect.width - 6 - buttonSize.x, rect.y + 6, buttonSize.x, buttonSize.y), new GUIContent(delete))) {
                    brain.controllerPrefabs.RemoveAt(i);
                    dirty = true;
                    continue;
                }

                rect.y += 10;
                rect.height -= 20;
                rect.x += buttonSize.x;
                rect.width -= buttonSize.y + 20;

                brain.controllerPrefabs[i] = drawer.Show(rect, prop, brain.controllerPrefabs[i], new GUIContent(), typeof(Controller), ref dirty) as Controller;
            }

            GUILayout.Space(buttonSize.y + 12);

            var rect2 = GUILayoutUtility.GetLastRect();
            rect2.width = w;

            EditorGUI.DrawRect(new Rect(rect2.x, rect2.y + 2, rect2.width, rect2.height - 4), new Color32(221, 221, 221, 255));

            if (GUI.Button(new Rect(rect2.x + 6, rect2.y + 6, buttonSize.x, buttonSize.y), new GUIContent(create))) {
                brain.controllerPrefabs.Add(null);
                dirty = true;
            }

            serializedObject.ApplyModifiedProperties();

            if (dirty) {
                EditorUtility.SetDirty(brain);
            }

            EditorGUILayout.LabelField("Detected Motors", EditorStyles.boldLabel);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;
            foreach (var motor in brain.GetComponentsInChildren<Motor>()) {
                EditorGUILayout.LabelField(motor.GetType().Name + " at " + motor.name);
            }

            EditorGUI.indentLevel = indent;
        }
    }
}
