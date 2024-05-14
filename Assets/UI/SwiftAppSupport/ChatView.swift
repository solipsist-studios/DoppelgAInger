// Author: ChatGPT

import SwiftUI
import UnityFramework

class ScrollManager: ObservableObject {
    var scrollProxy: ScrollViewProxy?
    
    func scrollToBottom(messageID: UUID?) {
        if (messageID != nil) {
            withAnimation {
                scrollProxy?.scrollTo(messageID, anchor: .bottom)
            }
        }
    }
}

struct ChatView: View {
    @EnvironmentObject() var messageHistory: MessageHistory
    @State private var refreshID = UUID()
    @ObservedObject var scrollManager = ScrollManager()
    
    @State private var textInput = ""
    
    var body: some View {
        VStack {
            ScrollViewReader { scrollProxy in
                ScrollView {
                    VStack {
                        ForEach(messageHistory.messages, id: \.self) { message in
                            HStack {
                                if message.isCurrentUser {
                                    Spacer()
                                    Text(message.message)
                                        .padding()
                                        .background(Color.blue)
                                        .cornerRadius(15)
                                        .foregroundColor(.white)
                                } else {
                                    Text(message.message)
                                        .padding()
                                        .background(Color.gray.opacity(0.2))
                                        .cornerRadius(15)
                                    Spacer()
                                }
                            }
                            .padding(.horizontal)
                            .id(message)
//                            Text(message.message)
//                                .id(message)
                                
                        }
                    }
                    .onAppear {
                        scrollManager.scrollProxy = scrollProxy
                    }
                }
                .onChange(of: messageHistory.messages) {
                    // Scroll to the bottom whenever the messages array changes
                    print("Message Count: \(messageHistory.messages.count)")
                    print("Message ID: \(messageHistory.messages.last?.id)")
                    
                    DispatchQueue.global().asyncAfter(deadline: .now() + 0.1) {
                        withAnimation {
                            scrollProxy.scrollTo(messageHistory.messages.last, anchor: .bottom)
                        }
                    }
                }
            }
            
            HStack {
                TextField("Message...", text: $textInput)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                
                Button(action: sendMessage) {
                    Text("Send")
                }
            }.padding()
        }
    }

    func sendMessage() {
        messageHistory.append(message: textInput, isUserMessage: true)

        // Call to managed code to post message to ChatGPT
        CallMessageCallback(textInput)
        
        textInput = ""
    }

    func receiveMessage(_ message: String) {
        messageHistory.append(message: message, isUserMessage: false)
    }
}
