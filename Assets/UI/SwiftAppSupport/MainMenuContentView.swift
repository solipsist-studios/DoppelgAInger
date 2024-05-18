//
// This custom View is referenced by SwiftUISampleInjectedScene
// to provide the body of a WindowGroup. It's part of the Unity-VisionOS
// target because it lives inside a "SwiftAppSupport" directory (and Unity
// will move it to that target).
//

import Foundation
import SwiftUI
import UnityFramework

struct MainMenuContentView: View {
    @Environment(\.openWindow) private var openWindow
    
    var body: some View {
        VStack {
            Text("Main Menu")
            Button("Dance") {
                CallMenuOptionCallback("dance")
            }
            Button("Switch Avatar") {
                CallMenuOptionCallback("next avatar")
            }
            Button("Chat View") {
                openWindow(id: "ChatWindow")
            }
            Button("Play Audio") {
                CallMenuOptionCallback("test audio")
            }
        }
        .onAppear {
            // Call the public function that was defined in SwiftUIBridge
            // inside UnityFramework
            CallMenuOptionCallback("appeared")
        }
    }
}

#Preview(windowStyle: .automatic) {
    MainMenuContentView()
}
