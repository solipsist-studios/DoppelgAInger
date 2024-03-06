using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

// This is a driver MonoBehaviour that connects to SwiftUISamplePlugin.swift via
// C# DllImport. See SwiftUISamplePlugin.swift for more information.
public class SwiftUIManager : MonoBehaviour
{
    public AudioSource audioSource;

    bool m_SwiftUIWindowOpen = false;

    void OnEnable()
    {
        SetSwiftCallback(CallbackFromSwift);
        OpenDebugWindow("DebugMenu");
    }

    void OnDisable()
    {
        SetSwiftCallback(null);
        CloseDebugWindow("DebugMenu");
    }

    void WasPressed(string buttonText, MeshRenderer meshrenderer)
    {
        if (m_SwiftUIWindowOpen)
        {
            CloseDebugWindow("DebugMenu");
            m_SwiftUIWindowOpen = false;
        }
        else
        {
            OpenDebugWindow("DebugMenu");
            m_SwiftUIWindowOpen = true;
        }
    }

    delegate void CallbackDelegate(string command);

    // This attribute is required for methods that are going to be called from native code
    // via a function pointer.
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    static void CallbackFromSwift(string command)
    {
        Debug.Log("Callback from swift: " + command);

        // This could be stored in a static field or a singleton.
        // If you need to deal with multiple windows and need to distinguish between them,
        // you could add an ID to this callback and use that to distinguish windows.
        var self = Object.FindFirstObjectByType<SwiftUIManager>();

        if (command == "closed") {
            self.m_SwiftUIWindowOpen = false;
            return;
        }

        if (command == "play sound")
        {
            //self.Spawn(Color.red);
        }
        else if (command == "next avatar")
        {
            //self.Spawn(Color.green);
        }
    }

    #if UNITY_VISIONOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void SetSwiftCallback(CallbackDelegate callback);

    [DllImport("__Internal")]
    static extern void OpenDebugWindow(string name);

    [DllImport("__Internal")]
    static extern void CloseDebugWindow(string name);
    #else
    static void SetSwiftCallback(CallbackDelegate callback) {}
    static void OpenDebugWindow(string name) {}
    static void CloseDebugWindow(string name) {}
    #endif

}