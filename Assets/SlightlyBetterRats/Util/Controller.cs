using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBR {
    public abstract class Controller : ScriptableObject, IEventSystemHandler {
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        public Channels channels { get; private set; }

        private bool isEnabled;
        public bool enabled {
            get {
                return isEnabled;
            }

            set {
                if (isEnabled != value) {
                    isEnabled = value;

                    if (isEnabled) {
                        OnControllerEnabled();
                    } else {
                        OnControllerDisabled();
                    }
                }
            }
        }

        public T GetComponent<T>() where T : Component {
            return transform.GetComponent<T>();
        }

        public T GetComponentInChildren<T>() where T : Component {
            return transform.GetComponentInChildren<T>();
        }

        public T GetComponentInParent<T>() where T : Component {
            return transform.GetComponentInParent<T>();
        }

        public T[] GetComponents<T>() where T : Component {
            return transform.GetComponents<T>();
        }

        public T[] GetComponentsInChildren<T>() where T : Component {
            return transform.GetComponentsInChildren<T>();
        }

        public T[] GetComponentsInParent<T>() where T : Component {
            return transform.GetComponentsInParent<T>();
        }

        public void SendMessage(string functionName, object argument = null) {
            if (argument != null) {
                var func = GetType().GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance, null, new System.Type[] { argument.GetType() }, null);
                if (func != null) {
                    func.Invoke(this, new object[] { argument });
                }
            } else {
                var func = GetType().GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (func != null) {
                    func.Invoke(this, null);
                }
            }
        }

        protected void print(object obj) {
            Debug.Log(obj, this);
        }

        public virtual void Initialize(GameObject obj) {
            gameObject = obj;
            transform = obj.transform;

            var brain = GetComponent<Brain>();
            if (brain) {
                channels = brain.channels;
            }
        }

        protected virtual void OnControllerEnabled() { }
        protected virtual void OnControllerDisabled() { }

        public virtual void Update() { }
    }
}
