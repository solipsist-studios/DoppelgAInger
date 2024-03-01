using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

[Serializable]
public class VoiceSettings
{
    public float stability;
    public float similarity_boost;
}

[Serializable]
public class TTSData
{
    public string text;
    public string model_id;
    public VoiceSettings voice_settings;
}

public class ElevenLabsTTS : MonoBehaviour
{
    public ElevenLabsConfig config;
    public AudioSource audioSource;
    public void Speak(string message)
    {
        StartCoroutine(GenerateAndStreamAudio(message));
    }

    public IEnumerator GenerateAndStreamAudio(string text)
    {
        string modelId = "eleven_multilingual_v2";
        string url = string.Format(config.ttsUrl, config.voiceId);

        TTSData ttsData = new TTSData
        {
            text = text.Trim(),
            model_id = modelId,
            voice_settings = new VoiceSettings
            {
                stability = 0.5f,
                similarity_boost = 0.8f
            }
        };

        string jsonData = JsonUtility.ToJson(ttsData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);


        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))//new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.method = "POST";
            //request.downloadHandler = new DownloadHandlerAudioClip(new Uri(url), AudioType.MPEG);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", config.apiKey);

            //string filename = string.Concat(Time.time.ToString(), ".wav");
            //var filepath = Path.Combine(Application.persistentDataPath + "/LipSync", filename); //This is probably the line causing problems. When I was building for desktop I used Application.dataPath btw.
            //Debug.Log(".WAV will be saved here: " + filepath);

            //DownloadHandlerFile dhf = new DownloadHandlerFile(filepath);
            //request.downloadHandler = dhf;

            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                yield return true;
            }

            if (request.result != UnityWebRequest.Result.Success || request.downloadedBytes == 0 || !string.IsNullOrEmpty(request.error))
            {
                Debug.LogError("Error: " + request.error);
                yield break;
            }

            //if (request.downloadedBytes == 0)
            //{
            //    Debug.LogError("Error: No downloaded data. Aborting");
            //    yield break;
            //}

            //byte[] buffer = new byte[request.downloadedBytes];
            //FileStream fs = new FileStream(filepath, FileMode.Open);
            //fs.Read(buffer, 0, buffer.Length);

            //AudioClip audioClip = AudioClip.Create("voice", (int)request.downloadedBytes, 1, 44100, false);

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);

            if (audioClip != null && audioClip.length != 0)
            {
                //Save(Time.time.ToString(), audioClip);

                audioSource.clip = audioClip;
                PlayAudio(audioClip);
                // Wait for the audio clip to finish playing
                yield return new WaitForSeconds(audioClip.length);
            }
            else
            {
                // the audio is null so download the audio again
                Debug.LogError("Error: No audio downloaded. Aborting.");
                //yield return StartCoroutine(GenerateAndStreamAudio(text));
                yield break;
            }


            // Wait for the audio clip to finish playing
            yield return new WaitForSeconds(audioClip.length);

        }

    }

    public static bool Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        var filepath = Path.Combine(Application.persistentDataPath + "/LipSync", filename); //This is probably the line causing problems. When I was building for desktop I used Application.dataPath btw.

        Debug.Log(".WAV will be saved here: " + filepath);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (var fileStream = SaveWav.CreateEmpty(filepath))
        {
            SaveWav.ConvertAndWrite(fileStream, clip);

            SaveWav.WriteHeader(fileStream, clip);
        }

        return true;
    }

    private void PlayAudio(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
