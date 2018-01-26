using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace SBR {
    [CustomEditor(typeof(ChannelsDefinition))]
    public class ChannelsDefinitionEditor : Editor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Generate Class")) {
                var path = AssetDatabase.GetAssetPath(target);
                if (path.Length > 0) {
                    ChannelsClassGenerator.GenerateClass(target as ChannelsDefinition);
                }
            }

            ChannelsDefinition def = target as ChannelsDefinition;

            DrawDefaultInspector();
        }
    }
}