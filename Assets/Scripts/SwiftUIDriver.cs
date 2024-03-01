using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Samples.PolySpatial.SwiftUI.Scripts
{
    // This is a driver MonoBehaviour that connects to SwiftUISamplePlugin.swift via
    // C# DllImport. See SwiftUISamplePlugin.swift for more information.
    public class SwiftUIDriver : MonoBehaviour
    {
        bool m_SwiftUIWindowOpen = false;

        void OnEnable()
        {
            SetNativeCallback(CallbackFromNative);
            OpenSwiftUIWindow("HelloWorld");
        }

        void OnDisable()
        {
            SetNativeCallback(null);
            CloseSwiftUIWindow("HelloWorld");
        }

        void WasPressed(string buttonText, MeshRenderer meshrenderer)
        {
            if (m_SwiftUIWindowOpen)
            {
                CloseSwiftUIWindow("HelloWorld");
                m_SwiftUIWindowOpen = false;
            }
            else
            {
                OpenSwiftUIWindow("HelloWorld");
                m_SwiftUIWindowOpen = true;
            }
        }

        delegate void CallbackDelegate(string command);

        // This attribute is required for methods that are going to be called from native code
        // via a function pointer.
        [MonoPInvokeCallback(typeof(CallbackDelegate))]
        static void CallbackFromNative(string command)
        {
            Debug.Log("Callback from native: " + command);

            // This could be stored in a static field or a singleton.
            // If you need to deal with multiple windows and need to distinguish between them,
            // you could add an ID to this callback and use that to distinguish windows.
            var self = Object.FindFirstObjectByType<SwiftUIDriver>();

            if (command == "closed") {
                self.m_SwiftUIWindowOpen = false;
                return;
            }
        }

        #if UNITY_VISIONOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void SetNativeCallback(CallbackDelegate callback);

        [DllImport("__Internal")]
        static extern void OpenSwiftUIWindow(string name);

        [DllImport("__Internal")]
        static extern void CloseSwiftUIWindow(string name);
        #else
        static void SetNativeCallback(CallbackDelegate callback) {}
        static void OpenSwiftUIWindow(string name) {}
        static void CloseSwiftUIWindow(string name) {}
        #endif

    }
}
