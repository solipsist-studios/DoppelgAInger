using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;

using OpenAI;

namespace Doppelgainger
{
    [Serializable]
    public class ChatEvent : UnityEvent<string> { }

    public class ChatGPT : MonoBehaviour
    {
        private OpenAIApi openai;

        private List<ChatMessage> messages = new List<ChatMessage>();

        [SerializeField] private ChatGPTConfig config;
        [SerializeField] private TextAsset charCard;

        public ChatEvent OnChatMessageReceived;

        private void Start()
        {
            // TODO: Support v1 char cards
            this.openai = new OpenAIApi(this.config.apiKey, this.config.organizationID);

            // Parse character card
            if (charCard != null)
            {
                var jsonChar = JsonUtility.FromJson<CharacterCard>(charCard.text);

                // Debug
                CharacterCard fooChar = new CharacterCard()
                {
                    spec = "chara_card_v2",
                    spec_version = "2.0",
                    data = new CharacterData()
                    {
                        name = "Foo",
                        description = "Test personality",
                        personality = "none",
                        first_mes = "Hello world"
                    }
                };
                string testJson = JsonUtility.ToJson(fooChar);

                // Add system prompt if specified
                if (!String.IsNullOrEmpty(this.config.systemPrompt))
                {
                    var systemMessage = new ChatMessage()
                    {
                        Role = "system",
                        Content = this.config.systemPrompt
                            .Replace("{{user}}", this.config.userName)
                            .Replace("{{char}}", jsonChar.data.name)
                    };

                    messages.Add(systemMessage);
                }

                // Add description
                if (!String.IsNullOrEmpty(jsonChar.data.description))
                {
                    var descriptionMessage = new ChatMessage()
                    {
                        Role = "system",
                        Content = jsonChar.data.description
                            .Replace('\r', '\0')
                            .Replace("{{user}}", this.config.userName)
                            .Replace("{{char}}", jsonChar.data.name)
                    };

                    messages.Add(descriptionMessage);
                }

                // Add personality
                if (!String.IsNullOrEmpty(jsonChar.data.personality))
                {
                    var personalityMessage = new ChatMessage()
                    {
                        Role = "system",
                        Content = this.config.personalityString
                            .Replace("{{char}}", jsonChar.data.name)
                            + jsonChar.data.personality
                    };

                    messages.Add(personalityMessage);
                }

                // Add scenario
                if (!String.IsNullOrEmpty(jsonChar.data.scenario))
                {
                    var scenarioMessage = new ChatMessage()
                    {
                        Role = "system",
                        Content = "Scenario: " +
                            jsonChar.data.scenario
                                .Replace("{{user}}", this.config.userName)
                                .Replace("{{char}}", jsonChar.data.name)
                    };

                    messages.Add(scenarioMessage);
                }

                // Add example messages
                


    //            { role: 'system', content: '[Example Chat]' },
    //{
    //            role: 'system',
    //  name: 'example_user',
    //  content: 'Where are you from?'
    //},
    //{
    //            role: 'system',
    //  name: 'example_assistant',
    //  content: "I've been around, but currently I'm residing in Montreal Canada. 🙂 \n" +
    //    ' You?'
    //},

                // Add character's first message
                if (!String.IsNullOrEmpty(jsonChar.data.first_mes))
                {
                    var firstMessage = new ChatMessage()
                    {
                        Role = "assistant",
                        Content = jsonChar.data.first_mes
                            .Replace("{{user}}", this.config.userName)
                            .Replace("{{char}}", jsonChar.data.name)
                    };

                    messages.Add(firstMessage);

                    // Do the UI things for handling messages
                    OnChatMessageReceived?.Invoke(firstMessage.Content);
                }
            }
        }

        public async void SendReply(string messageText)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = messageText
            };

            messages.Add(newMessage);
                       
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = this.config.modelVersion,
                Messages = messages,
                Temperature = 0.2f 
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);

                // Append message in SwiftUI
                OnChatMessageReceived?.Invoke(message.Content);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
    }
}
