//
// This custom View is referenced by SwiftUISampleInjectedScene
// to provide the body of a WindowGroup. It's part of the Unity-VisionOS
// target because it lives inside a "SwiftAppSupport" directory (and Unity
// will move it to that target).
//

import Foundation
import SwiftUI
import UnityFramework

struct DebugMenuContentView: View {
    var body: some View {
        VStack {
            Text("Main Menu")
            Button("Play Sound") {
                CallCSharpCallback("play sound")
            }
            Button("Switch Avatar") {
                CallCSharpCallback("next avatar")
            }
        }
        .onAppear {
            // Call the public function that was defined in SwiftUIBridge
            // inside UnityFramework
            CallCSharpCallback("appeared")
        }
    }
}

#Preview(windowStyle: .automatic) {
    DebugMenuContentView()
}

