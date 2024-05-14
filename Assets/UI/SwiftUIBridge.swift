//
// This is a sample Swift plugin that provides an interface for
// the SwiftUI sample to interact with. It must be linked into
// UnityFramework, which is what the default Swift file plugin
// importer will place it into.
//
// It uses "@_cdecl", a Swift not-officially-supported attribute to
// provide C-style linkage and symbol for a given function.
//
// It also uses a "hack" to create an EnvironmentValues() instance
// in order to fetch the openWindow and dismissWindow action. Normally,
// these would be provided to a view via something like:
//
//    @Environment(\.openWindow) var openWindow
//
// but we don't have a view at this point, and it's expected that these
// actions will be global (and not view-specific) anyway.
//
// There are two additional files that complete this example:
// SwiftUISampleInjectedScene.swift and HelloWorldConventView.swift.
//
// Any file named "...InjectedScene.swift" will be moved to the Unity-VisionOS
// Xcode target (as it must be there in order to be referenced by the App), and
// its static ".scene" member will be added to the App's main Scene. See
// the comments in SwiftUISampleInjectedScene.swift for more information.
//
// Any file that's inside of a "SwiftAppSupport" directory anywhere in its path
// will also be moved to the Unity-VisionOS Xcode target. HelloWorldContentView.swift
// is inside SwiftAppSupport beceause it's needed by the WindowGroup this sample
// adds to provide its content.
//

import Foundation
import SwiftUI

public struct ChatMessage: Identifiable, Hashable {
    public var id = UUID()
    public var message: String
    public var isCurrentUser: Bool

    public static func == (lhs: ChatMessage, rhs: ChatMessage) -> Bool {
        return lhs.id == rhs.id && lhs.message == rhs.message
    }
    
    public func hash(into hasher: inout Hasher) {
        hasher.combine(id)
        hasher.combine(message)
    }
}

public class MessageHistory: ObservableObject {
    public static let shared = MessageHistory()
    
    @Published public var messages: [ChatMessage] = [
        ChatMessage(message: "Hello!", isCurrentUser: false),
        ChatMessage(message: "Hi! How are you?", isCurrentUser: true),
        ChatMessage(message: "I'm great, thanks for asking. How about you?", isCurrentUser: false)
    ]

    public func append(message: String, isUserMessage: Bool) {
        print("########### Number of messages: \(MessageHistory.shared.messages.count)")
        
        DispatchQueue.main.async {
            let newMessage = ChatMessage(message: message, isCurrentUser: isUserMessage)
            self.messages.append(newMessage)
        }
    }
}


// These methods are exported from Swift with an explicit C-style name using @_cdecl,
// to match what DllImport expects. You will need to do appropriate conversion from
// C-style argument types (including UnsafePointers and other friends) into Swift
// as appropriate.

// SetNativeCallback is called from the SwiftUIDriver MonoBehaviour in OnEnable,
// to give Swift code a way to make calls back into C#. You can use one callback or
// many, as appropriate for your application.
//
// Declared in C# as: delegate void CallbackDelegate(string command);
typealias SwiftCallbackDelegateType = @convention(c) (UnsafePointer<CChar>) -> Void

var sMenuOptionDelegate: SwiftCallbackDelegateType? = nil
var sMessageDelegate: SwiftCallbackDelegateType? = nil

// Declared in C# as: static extern void SetSwiftCallback(CallbackDelegate callback);
@_cdecl("SetSwiftMenuOptionCallback")
func setSwiftMenuOptionCallback(_ delegate: SwiftCallbackDelegateType)
{
    print("############ SET MENU OPTION CALLBACK")
    sMenuOptionDelegate = delegate
}

// Declared in C# as: static extern void SetSwiftCallback(CallbackDelegate callback);
@_cdecl("SetSwiftMessageCallback")
func setSwiftMessageCallback(_ delegate: SwiftCallbackDelegateType)
{
    print("############ SET MESSAGE CALLBACK")
    sMessageDelegate = delegate
}

// This is a function for your own use from the enclosing Unity-VisionOS app, to call the delegate
// from your own windows/views (HelloWorldContentView uses this)
public func CallMenuOptionCallback(_ str: String)
{
    if (sMenuOptionDelegate == nil) {
        return
    }

    str.withCString {
        sMenuOptionDelegate!($0)
    }
}

public func CallMessageCallback(_ str: String)
{
    if (sMessageDelegate == nil) {
        return
    }

    str.withCString {
        sMessageDelegate!($0)
    }
}

// Declared in C# as: static extern void OpenSwiftWindow(string name);
@_cdecl("OpenSwiftWindow")
func openSwiftWindow(_ cname: UnsafePointer<CChar>)
{
    let openWindow = EnvironmentValues().openWindow

    let name = String(cString: cname)
    print("############ OPEN WINDOW \(name)")
    openWindow(id: name)
}

// Declared in C# as: static extern void CloseSwiftWindow(string name);
@_cdecl("CloseSwiftWindow")
func closeSwiftWindow(_ cname: UnsafePointer<CChar>)
{
    let dismissWindow = EnvironmentValues().dismissWindow

    let name = String(cString: cname)
    print("############ CLOSE WINDOW \(name)")
    dismissWindow(id: name)
}

// Declared in C# as: static extern void ReceiveMessage(string message);
@_cdecl("ReceiveSwiftMessage")
func receiveMessage(_ cmessage: UnsafePointer<CChar>)
{
    let message = String(cString: cmessage)
    //MessageHistory.shared.append(message: message, isUserMessage: false)
    
//    ForEach(MessageHistory.shared.messages) { message in
//        print(message)
//    }
    
    //let newMessage = ChatMessage(message: message, isCurrentUser: false)
    MessageHistory.shared.append(message: message, isUserMessage: false)
    
    print("########## RECEIVED MESSAGE: \(message)")
}
