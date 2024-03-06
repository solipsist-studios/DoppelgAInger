using UnityEngine;

namespace Doppelgainger
{
    [CreateAssetMenu(fileName = "ElevenLabsConfig", menuName = "ElvenLabs/ElvenLabs Configuration")]
    public class ElevenLabsConfig : ScriptableObject
    {
        public string apiKey = "";
        public string voiceId = ""; // Harry
        public string ttsUrl = "https://api.elevenlabs.io/v1/text-to-speech/{0}/stream";

    }
}
