using UnityEngine;

namespace Doppelgainger
{
    [CreateAssetMenu(fileName = "ChatGPTConfig", menuName = "OpenAI/ChatGPT Configuration")]
    public class ChatGPTConfig : ScriptableObject
    {
        public string apiKey = "";
        public string organizationID = null;
        public string modelVersion = "gpt-4o-2024-05-13";
        public string systemPrompt = "Write {{char}}'s next reply in a fictional chat between {{char}} and {{user}}. Write 1 reply only in internet RP style, italicize actions, and avoid quotation marks. Use markdown. Be proactive, creative, and drive the conversation forward. Write at least 1 paragraph, up to 4. Always stay in character and avoid repetition.";
        public string userName = "You";
        public string personalityString = "{{char}}'s personality: ";
    }
}
