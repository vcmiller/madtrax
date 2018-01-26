using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace SBR {
    public class StateMachineEditorWindow : EditorWindow {
        private Operation op;
        private bool dirty = false;
        private float sidePanelWidth;
        private bool resize = false;
        private readonly Color bgColor = new Color32(93, 93, 93, 255);
        private readonly Color lineColor = new Color32(70, 70, 70, 255);
        private readonly Color darkLineColor = new Color32(40, 40, 40, 255);
        private readonly Color panelColor = new Color32(194, 194, 194, 255);

        [NonSerialized]
        private string[] types;

        [NonSerialized]
        public static StateMachineDefinition def;

        [NonSerialized]
        private StateMachineDefinition.Transition lastSelectedTr;
        [NonSerialized]
        private StateMachineDefinition.State lastSelectedState;

        [NonSerialized]
        private StateMachineDefinition.State editingState;
        [NonSerialized]
        private Pair<StateMachineDefinition.State, StateMachineDefinition.Transition> editingTransition;

        Vector2 scrollPos = Vector2.zero;

        public StateMachineEditorWindow() {
            wantsMouseMove = true;
            editingState = null;
            editingTransition = new Pair<StateMachineDefinition.State, StateMachineDefinition.Transition>(null, null);
        }

        [MenuItem("Window/State Machine Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(StateMachineEditorWindow));
        }

        void UpdateSelection() {
            Repaint();
        }

        void OnGUI() {
            ResizeScrollView();
            Rect viewportRect = new Rect(0, 0, position.width - sidePanelWidth, position.height);
            EditorGUI.DrawRect(viewportRect, bgColor);
            bool repaint = false;

            foreach (var obj in Selection.objects) {
                if (obj is StateMachineDefinition) {
                    def = (StateMachineDefinition)obj;
                    break;
                }
            }

            if (def != null) {
                bool showSide = true;

                Event cur = Event.current;

                if (op != null) {
                    if (def != op.definition) {
                        op.Cancel();
                        op = null;
                    } else {
                        op.Update();

                        if (op.done) {
                            op = null;
                            repaint = true;
                            EditorUtility.SetDirty(def);
                            dirty = true;
                        }
                    }
                } else {
                    var selected = def.SelectState(cur.mousePosition + scrollPos);
                    var selectedTr = def.SelectTransition(cur.mousePosition + scrollPos);

                    if (selected == null) {
                        if (selectedTr.t2 != lastSelectedTr) {
                            repaint = true;
                            lastSelectedTr = selectedTr.t2;
                        }

                        if (lastSelectedState != null) {
                            lastSelectedState = null;
                            repaint = true;
                        }
                    } else {
                        if (selected != lastSelectedState) {
                            repaint = true;
                            lastSelectedState = selected;
                        }

                        if (lastSelectedTr != null) {
                            lastSelectedTr = null;
                            repaint = true;
                        }
                    }

                    if (viewportRect.Contains(cur.mousePosition)) {
                        if (cur.type == EventType.MouseDown) {
                            if (cur.button == 0) {
                                if (selected != null) {
                                    editingState = selected;
                                    editingTransition.t1 = null;
                                    editingTransition.t2 = null;
                                    repaint = true;
                                    showSide = false;


                                    if (cur.clickCount == 1) {
                                        op = new MoveStateOperation(def, selected);
                                    } else if (cur.clickCount == 2) {
                                        op = new RenameStateOperation(def, selected);
                                    }
                                } else if (selectedTr.t1 != null) {
                                    editingTransition = selectedTr;
                                    editingState = null;
                                    repaint = true;
                                    showSide = false;
                                } else {
                                    editingState = null;
                                    editingTransition.t1 = null;
                                    editingTransition.t2 = null;
                                    repaint = true;
                                    showSide = false;
                                }
                            } else if (cur.button == 1) {
                                if (selected == null && lastSelectedTr == null) {
                                    GenericMenu menu = new GenericMenu();

                                    menu.AddItem(new GUIContent("Create State"), false, () => {
                                        var s = def.AddState();
                                        s.position = cur.mousePosition;
                                        op = new RenameStateOperation(def, s);
                                        EditorUtility.SetDirty(def);
                                        dirty = true;
                                    });

                                    menu.ShowAsContext();
                                } else if (selected != null) {
                                    GenericMenu menu = new GenericMenu();

                                    menu.AddItem(new GUIContent("Delete State"), false, () => {
                                        def.RemoveState(selected);
                                        EditorUtility.SetDirty(def);
                                        dirty = true;
                                        repaint = true;
                                    });

                                    menu.AddItem(new GUIContent("Add Transition"), false, () => {
                                        op = new MakeTransitionOperation(def, selected);
                                    });

                                    if (selected.name != def.defaultState) {
                                        menu.AddItem(new GUIContent("Make Default State"), false, () => {
                                            def.defaultState = selected.name;
                                            dirty = true;
                                            repaint = true;
                                        });
                                    }

                                    menu.ShowAsContext();
                                } else if (selectedTr.t1 != null) {
                                    GenericMenu menu = new GenericMenu();

                                    menu.AddItem(new GUIContent("Remove Transition"), false, () => {
                                        selectedTr.t1.RemoveTransition(selectedTr.t2);
                                        EditorUtility.SetDirty(def);
                                        dirty = true;
                                        repaint = true;
                                    });

                                    menu.ShowAsContext();
                                }
                            }
                        } else if (cur.type == EventType.MouseDrag && cur.button == 2) {
                            scrollPos -= cur.delta;
                            repaint = true;
                        } else if (cur.type == EventType.KeyDown && cur.keyCode == KeyCode.F) {
                            var state = def.GetState(def.defaultState);
                            if (state == null && def.states.Count > 0) {
                                state = def.states[0];
                            }

                            if (state != null) {
                                scrollPos = state.position - viewportRect.size / 2;
                                repaint = true;
                            }
                        }
                    }
                }

                if (Event.current.type != EventType.Repaint && (repaint || (op != null && op.repaint))) {
                    Repaint();
                }

                Handles.BeginGUI();

                Handles.color = lineColor;
                for (float x = -scrollPos.x % MoveStateOperation.snap; x < viewportRect.width; x += MoveStateOperation.snap) {
                    Handles.DrawLine(new Vector3(x, 0), new Vector3(x, viewportRect.height));
                }

                for (float y = -scrollPos.y % MoveStateOperation.snap; y < viewportRect.height; y += MoveStateOperation.snap) {
                    Handles.DrawLine(new Vector3(0, y), new Vector3(viewportRect.width, y));
                }

                foreach (var from in def.states) {
                    if (from.transitions != null) {
                        foreach (var tr in from.transitions) {
                            if (tr != lastSelectedTr && tr != editingTransition.t2) {
                                Handles.color = Color.black;
                            } else {
                                Handles.color = Color.blue;
                            }

                            if (def.GetState(tr.to) != null) {
                                var line = def.GetTransitionPoints(from, tr);
                                Vector2 src = line.t1 - scrollPos;
                                Vector2 dest = line.t2 - scrollPos;

                                Vector2 v = (dest - src).normalized;
                                Vector2 ortho = new Vector2(v.y, -v.x);

                                Vector2 arrow = ortho - v;
                                Vector2 mid = (src + dest) / 2;

                                Handles.DrawAAPolyLine(3, src, dest);
                                Handles.DrawAAPolyLine(3, mid + v * 5, mid + arrow * 10);
                            }
                        }
                    }
                }

                Handles.EndGUI();

                foreach (var state in def.states) {
                    if (op == null || op.state != state || op.showBaseGUI) {
                        string s = state.name;
                        if (def.defaultState == s) {
                            s += "\n<default state>";
                        }

                        Rect rect = state.rect;
                        rect.position -= scrollPos;

                        if (state != lastSelectedState && state != editingState) {
                            GUI.Button(rect, s);
                        } else {
                            GUI.Button(rect, "");

                            var centeredStyle = new GUIStyle(GUI.skin.label);
                            centeredStyle.alignment = TextAnchor.MiddleCenter;
                            centeredStyle.normal.textColor = Color.blue;
                            centeredStyle.fontStyle = FontStyle.Bold;

                            GUI.Label(rect, s, centeredStyle);
                        }
                    }

                    if (op != null && op.state == state) {
                        op.OnGUI();
                    }
                }

                if (showSide) {
                    EditorGUI.DrawRect(new Rect(position.width - sidePanelWidth, 0, sidePanelWidth, position.height), panelColor);


                    float padding = 20;
                    GUILayout.BeginArea(new Rect(position.width - sidePanelWidth + padding, padding, sidePanelWidth - padding * 2, position.height - padding * 2));
                    EditorGUILayout.BeginVertical();
                    float w = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 1;

                    if (GUILayout.Button(dirty ? "Save *" : "Save")) {
                        AssetDatabase.SaveAssets();
                        dirty = false;
                        StateMachineClassGenerator.GenerateAbstractClass(def);
                    }

                    if (GUILayout.Button("New Impl Class...")) {
                        var path = EditorUtility.SaveFilePanelInProject("Save new class", def.name + "Impl.cs", "cs", "Enter a name for the new impl class");
                        if (path.Length > 0) {
                            StateMachineClassGenerator.GenerateImplClass(def, path);
                        }
                    }

                    if (types == null) {
                        types = typeof(StateMachine).Assembly.GetTypes()
                            .Where(p => !p.IsGenericType && typeof(StateMachine).IsAssignableFrom(p))
                            .Select(t => t.FullName).ToArray();
                    }

                    int index = Array.IndexOf(types, def.baseClass);
                    if (index < 0) {
                        index = Array.IndexOf(types, typeof(StateMachine).FullName);
                    }
                    int prev = index;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Base Class");
                    index = EditorGUILayout.Popup(index, types);

                    if (prev != index) {
                        dirty = true;
                    }

                    EditorGUILayout.EndHorizontal();
                    def.baseClass = types[index];

                    if (editingState != null) {
                        EditorGUILayout.LabelField("State " + editingState.name);
                        EditorGUILayout.LabelField("State Events", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Enter: ");
                        bool enter = EditorGUILayout.Toggle(editingState.hasEnter);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("During: ");
                        bool during = EditorGUILayout.Toggle(editingState.hasDuring);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Exit: ");
                        bool exit = EditorGUILayout.Toggle(editingState.hasExit);
                        EditorGUILayout.EndHorizontal();


                        if (enter != editingState.hasEnter || during != editingState.hasDuring || exit != editingState.hasExit) {
                            editingState.hasEnter = enter;
                            editingState.hasDuring = during;
                            editingState.hasExit = exit;

                            dirty = true;
                            EditorUtility.SetDirty(def);
                        }

                    } else if (editingTransition.t1 != null) {
                        EditorGUILayout.LabelField("Transition From " + editingTransition.t1.name + " To " + editingTransition.t2.to);
                        EditorGUILayout.LabelField("Transition Events", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Notify: ");
                        bool notify = EditorGUILayout.Toggle(editingTransition.t2.hasNotify);
                        EditorGUILayout.EndHorizontal();

                        if (notify != editingTransition.t2.hasNotify) {
                            editingTransition.t2.hasNotify = notify;

                            dirty = true;
                            EditorUtility.SetDirty(def);
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUIUtility.labelWidth = w;

                    GUILayout.EndArea();
                }
            } else if (op != null) {
                op.Cancel();
                op = null;
            }

        }

        private void OnEnable() {
            sidePanelWidth = 200;

            Selection.selectionChanged += UpdateSelection;
            titleContent = new GUIContent("SM Editor", Resources.Load<Texture2D>("StateMachine"));
        }

        private void OnDisable() {
            Selection.selectionChanged -= UpdateSelection;
        }

        private void ResizeScrollView() {
            Rect cursorChangeRect = new Rect(position.width - sidePanelWidth - 5.0f, 0, 10f, position.height);

            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition)) {
                resize = true;
            }
            if (resize) {
                sidePanelWidth = position.width - Event.current.mousePosition.x;
                Repaint();
            }
            if (Event.current.type == EventType.MouseUp)
                resize = false;
        }
    }
}