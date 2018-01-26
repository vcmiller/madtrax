﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.UI;

namespace SBR {
    public abstract class PlayerController : Controller {

        private Dictionary<string, ButtonHandler> buttonDown;
        private Dictionary<string, ButtonHandler> buttonUp;
        private Dictionary<string, ButtonHandler> buttonHeld;
        private Dictionary<string, AxisHandler> axes;

        public string inputSuffix;

        private delegate void ButtonHandler();
        private delegate void AxisHandler(float value);

        private ViewTarget curViewTarget;
        public ViewTarget viewTarget {
            get {
                return curViewTarget;
            }

            set {
                if (curViewTarget) {
                    curViewTarget.enabled = false;
                }
                curViewTarget = value;
                if (curViewTarget) {
                    curViewTarget.enabled = enabled;
                }
            }
        }

        public override void Initialize(GameObject obj) {
            base.Initialize(obj);

            axes = new Dictionary<string, AxisHandler>();
            buttonDown = new Dictionary<string, ButtonHandler>();
            buttonUp = new Dictionary<string, ButtonHandler>();
            buttonHeld = new Dictionary<string, ButtonHandler>();

            viewTarget = GetComponentInChildren<ViewTarget>();

            foreach (MethodInfo m in GetType().GetMethods()) {
                if (m.Name.StartsWith("Axis_")) {
                    var param = m.GetParameters();

                    if (param.Length == 1 && param[0].ParameterType == typeof(float)) {
                        string axis = m.Name.Substring(5);
                        if (axes.ContainsKey(axis)) {
                            Debug.LogWarning("Waring: Duplicate event handler found for axis " + axis + ".");
                        } else {
                            axes.Add(axis, (AxisHandler)Delegate.CreateDelegate(typeof(AxisHandler), this, m));
                        }
                    } else {
                        Debug.LogWarning("Warning: Axis event handler " + m.Name + " should take one argument of type float.");
                    }
                } else if (m.Name.StartsWith("Button_")) {
                    if (m.GetParameters().Length == 0) {
                        string btn = m.Name.Substring(7);
                        if (buttonHeld.ContainsKey(btn)) {
                            Debug.LogWarning("Waring: Duplicate event handler found for button " + btn + ".");
                        } else {
                            buttonHeld.Add(btn, (ButtonHandler)Delegate.CreateDelegate(typeof(ButtonHandler), this, m));
                        }
                    } else {
                        Debug.LogWarning("Warning: Button event handler " + m.Name + " should take no arguments.");
                    }
                } else if (m.Name.StartsWith("ButtonUp_")) {
                    if (m.GetParameters().Length == 0) {
                        string btn = m.Name.Substring(9);
                        if (buttonUp.ContainsKey(btn)) {
                            Debug.LogWarning("Waring: Duplicate event handler found for button up " + btn + ".");
                        } else {
                            buttonUp.Add(btn, (ButtonHandler)Delegate.CreateDelegate(typeof(ButtonHandler), this, m));
                        }
                    } else {
                        Debug.LogWarning("Warning: ButtonUp event handler " + m.Name + " should take no arguments.");
                    }
                } else if (m.Name.StartsWith("ButtonDown_")) {
                    if (m.GetParameters().Length == 0) {
                        string btn = m.Name.Substring(11);
                        if (buttonDown.ContainsKey(btn)) {
                            Debug.LogWarning("Waring: Duplicate event handler found for button down " + btn + ".");
                        } else {
                            buttonDown.Add(btn, (ButtonHandler)Delegate.CreateDelegate(typeof(ButtonHandler), this, m));
                        }
                    } else {
                        Debug.LogWarning("Warning: ButtonDown event handler " + m.Name + " should take no arguments.");
                    }
                }
            }

        }

        public override void Update() {
            if (enabled) {
                foreach (var m in axes) {
                    m.Value(Input.GetAxis(m.Key + inputSuffix));
                }

                foreach (var m in buttonDown) {
                    if (Input.GetButtonDown(m.Key + inputSuffix)) {
                        m.Value();
                    }
                }

                foreach (var m in buttonHeld) {
                    if (Input.GetButton(m.Key + inputSuffix)) {
                        m.Value();
                    }
                }

                foreach (var m in buttonUp) {
                    if (Input.GetButtonUp(m.Key + inputSuffix)) {
                        m.Value();
                    }
                }
            }
        }

        protected override void OnControllerDisabled() {
            if (curViewTarget) {
                curViewTarget.enabled = false;
            }
        }

        protected override void OnControllerEnabled() {
            if (curViewTarget) {
                curViewTarget.enabled = true;
            }
        }
    }
}