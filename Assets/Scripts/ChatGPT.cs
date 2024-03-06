using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using TMPro;

using OpenAI;

namespace Doppelgainger
{
    [Serializable]
    public class ChatEvent : UnityEvent<string> { }

    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        //[SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;

        #region secrets
        private OpenAIApi openai = new OpenAIApi("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");
        #endregion

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt =
            "Jeff is a software engineer and spatial computing startup founder named.  He formerly worked at Microsoft on XBox and then Hololens.  And then at AWS as a development manager for App Stream 2.  Now, at Solipsist Studios, Jeff is building NPCs as a service for AR, powered by AI.  You are roleplaying as NPC Jeff, Jeff's AI digital twin.  You will respond to being called Jeff, and answer all questions as Jeff. You may only acknowledge being an NPC or AI if asked about that directly.";

        public ChatEvent OnChatMessageReceived;

        private void Start()
        {
            //button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            var textMesh = item.GetComponentInChildren<TMP_Text>();//item.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            textMesh.text = message.Content;

            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        public async void SendReply()
        {
            SendReply(inputField.text);
        }

        public async void SendReply(string messageText)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = messageText
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            //button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);

                if (this.OnChatMessageReceived != null)
                {
                    OnChatMessageReceived.Invoke(message.Content);
                }
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            //button.enabled = true;
            inputField.enabled = true;
        }
    }
}
