using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Events;

// This is a driver MonoBehaviour that connects to SwiftUISamplePlugin.swift via
// C# DllImport. See SwiftUISamplePlugin.swift for more information.
public class SwiftUIManager : MonoBehaviour
{
    private static SwiftUIManager instance = null;

    private bool swiftUIWindowOpen = false;

    private SwiftUIManager()
    {
        if (instance != null)
        {
            Debug.LogError("Duplicate instance of SwiftUIManager detected!");
        }

        instance = this;
    }

    public static SwiftUIManager Instance { get { return instance; } }

    public UnityEvent<string> OnMessageReceived;

    void OnEnable()
    {
        SetSwiftMenuOptionCallback(SwiftMenuCallback);
        SetSwiftMessageCallback(SwiftMessageCallback);
        OpenSwiftWindow("MainMenu");
        OpenSwiftWindow("ChatWindow");
    }

    void OnDisable()
    {
        SetSwiftMenuOptionCallback(null);
        SetSwiftMessageCallback(null);
        CloseSwiftWindow("MainMenu");
        CloseSwiftWindow("ChatWindow");
    }

    //void WasPressed(string buttonText, MeshRenderer meshrenderer)
    //{
    //    if (m_SwiftUIWindowOpen)
    //    {
    //        CloseDebugWindow("MainMenu");
    //        m_SwiftUIWindowOpen = false;
    //    }
    //    else
    //    {
    //        OpenDebugWindow("MainMenu");
    //        m_SwiftUIWindowOpen = true;
    //    }
    //}

    public void AppendMessage(string message)
    {
        ReceiveSwiftMessage(message);
    }

    delegate void StringCallbackDelegate(string str);

    // This attribute is required for methods that are going to be called from native code
    // via a function pointer.
    [MonoPInvokeCallback(typeof(StringCallbackDelegate))]
    static void SwiftMenuCallback(string command)
    {
        Debug.Log("Callback from swift: " + command);

        if (command == "closed") {
            Instance.swiftUIWindowOpen = false;
            return;
        }

        if (command == "dance")
        {
            StateManager.Instance.Dance();
        }
        else if (command == "next avatar")
        {
            StateManager.Instance.NextAvatar();
        }
        else if (command == "test audio")
        {
            if (StateManager.Instance != null && StateManager.Instance.audioSource != null)
            {
                Debug.Log("Playing test audio");
                StateManager.Instance.audioSource.Play();
            }
        }
    }

    [MonoPInvokeCallback(typeof(StringCallbackDelegate))]
    static void SwiftMessageCallback(string message)
    {
        Debug.Log(message);

        Instance.OnMessageReceived?.Invoke(message);
    }

#if UNITY_VISIONOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void SetSwiftMenuOptionCallback(StringCallbackDelegate callback);

    [DllImport("__Internal")]
    static extern void SetSwiftMessageCallback(StringCallbackDelegate callback);

    [DllImport("__Internal")]
    static extern void OpenSwiftWindow(string name);

    [DllImport("__Internal")]
    static extern void CloseSwiftWindow(string name);

    [DllImport("__Internal")]
    static extern void ReceiveSwiftMessage(string message);
#else
    static void SetSwiftMenuOptionCallback(StringCallbackDelegate callback) {}
    static void SetSwiftMessageCallback(StringCallbackDelegate callback) { }
    static void OpenSwiftWindow(string name) {}
    static void CloseSwiftWindow(string name) {}
    static void ReceiveSwiftMessage(string message) { }
#endif

}