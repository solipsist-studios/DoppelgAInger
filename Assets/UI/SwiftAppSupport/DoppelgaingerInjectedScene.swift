// Any swift file whose name ends in "InjectedScene" is expected to contain
// a computed static "scene" property like the one below. It will be injected to the top
// level App's Scene. The name of the class/struct must match the name of the file.

import Foundation
import SwiftUI
import UnityFramework

struct DoppelgaingerInjectedScene {
    @SceneBuilder
    static var scene: some Scene {
        WindowGroup(id: "MainMenu") {
            // The sample defines a custom view, but you can also put your entire window's
            // structure here as you can with SwiftUI.
            MainMenuContentView()
                .environmentObject(MessageHistory.shared)
        }.defaultSize(width: 400.0, height: 400.0)
        
        WindowGroup(id: "ChatWindow") {
            ChatView()
                .environmentObject(MessageHistory.shared)
        }.defaultSize(width: 400.0, height: 400.0)

        // You can create multiple WindowGroups here for different wnidows;
        // they need a distinct id. If you include multiple items,
        // the scene property must be decorated with "@SceneBuilder" as above.
        WindowGroup(id: "SimpleText") {
            Text("Hello World")
        }
    }
}
