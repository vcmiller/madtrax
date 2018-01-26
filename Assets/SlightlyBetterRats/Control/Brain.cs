using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SBR {
    public class Brain : MonoBehaviour {
        [TypeSelect(typeof(Channels))]
        public string channelsType;

        public Channels channels { get; private set; }

        [EditClassAndFields]
        public List<Controller> controllerPrefabs;
        private Controller[] controllers;
        private Motor[] motors;

        private int _activeIndex = -1;
        public int defaultController = 0;

        public int activeControllerIndex {
            get {
                return _activeIndex;
            }

            set {
                if (value != _activeIndex && value < controllers.Length) {
                    if (_activeIndex >= 0) {
                        activeController.enabled = false;
                    }

                    _activeIndex = value;

                    if (value >= 0) {
                        activeController.enabled = true;
                    }
                }
            }
        }

        public Controller activeController {
            get {
                return _activeIndex >= 0 ? controllers[_activeIndex] : null;
            }
        }

        private void Awake() {
            Type t = Type.GetType(channelsType);

            if (typeof(Channels).IsAssignableFrom(t)) {
                channels = (Channels)t.GetConstructor(new Type[0]).Invoke(new object[0]);
            } else {
                Debug.LogError("Error: invalid channel type!");
            }

            motors = GetComponentsInChildren<Motor>();

            controllers = new Controller[controllerPrefabs.Count];
            for (int i = 0; i < controllerPrefabs.Count; i++) {
                controllers[i] = Instantiate(controllerPrefabs[i]);
                controllers[i].Initialize(gameObject);
            }

            activeControllerIndex = defaultController;
        }

        private void Update() {
            var c = activeController;

            if (c) {
                c.Update();
            }

            foreach (var motor in motors) {
                if (motor.enableInput) {
                    motor.TakeInput();
                    motor.UpdateAfterInput();
                }
            }

            channels.ClearInput();
        }

        private void OnDestroy() {
            foreach (var ctrl in controllers) {
                Destroy(ctrl);
            }
        }

        public void SendMessageToControllers(string functionName, object parameter = null) {
            foreach (var ctrl in controllers) {
                ctrl.SendMessage(functionName, parameter);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            SendMessageToControllers("OnCollisionEnter", collision);
        }

        private void OnCollisionStay(Collision collision) {
            SendMessageToControllers("OnCollisionStay", collision);
        }

        private void OnCollisionExit(Collision collision) {
            SendMessageToControllers("OnCollisionExit", collision);
        }

        private void OnTriggerEnter(Collider other) {
            SendMessageToControllers("OnTriggerEnter", other);
        }

        private void OnTriggerStay(Collider other) {
            SendMessageToControllers("OnTriggerStay", other);
        }

        private void OnTriggerExit(Collider other) {
            SendMessageToControllers("OnTriggerExit", other);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            SendMessageToControllers("OnCollisionEnter2D", collision);
        }

        private void OnCollisionStay2D(Collision2D collision) {
            SendMessageToControllers("OnCollisionStay2D", collision);
        }

        private void OnCollisionExit2D(Collision2D collision) {
            SendMessageToControllers("OnCollisionExit2D", collision);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            SendMessageToControllers("OnTriggerEnter2D", collision);
        }

        private void OnTriggerStay2D(Collider2D collision) {
            SendMessageToControllers("OnTriggerStay2D", collision);
        }

        private void OnTriggerExit2D(Collider2D collision) {
            SendMessageToControllers("OnTriggerExit2D", collision);
        }
    }
}
