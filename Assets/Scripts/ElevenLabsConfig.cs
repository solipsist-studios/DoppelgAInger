using UnityEngine;

[CreateAssetMenu(fileName = "ElevenLabsConfig", menuName = "ElvenLabs/ElvenLabs Configuration")]
public class ElevenLabsConfig : ScriptableObject
{
    public string apiKey = "11649f2ef4907461df743e73ad3143d4";
    public string voiceId = "SOYHLrjzK2X1ezoPC6cr"; // Harry
    public string ttsUrl = "https://api.elevenlabs.io/v1/text-to-speech/{0}/stream";

}
