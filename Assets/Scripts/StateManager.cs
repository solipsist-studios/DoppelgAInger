using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private static StateManager instance = null;
    public static StateManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private int curAvatarIdx = 0;

    public AudioSource audioSource;

    public List<GameObject> Avatars = new List<GameObject>();

    private StateManager()
    {
        if (instance != null)
        {
            Debug.LogError("[StateManager] Duplicate instance detected!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        curAvatarIdx = 0;
        SelectAvatar(curAvatarIdx);
    }

    public void SelectAvatar(int index)
    {
        if (index < 0 || index >= Avatars.Count)
        {
            Debug.LogError("[StateManager] Avatar index out of bounds: " + index);
            return;
        }

        foreach (var avatar in Avatars)
        {
            avatar.SetActive(false);
        }

        curAvatarIdx = index;
        Avatars[curAvatarIdx].SetActive(true);
    }

    public void NextAvatar()
    {
        int index = curAvatarIdx + 1;
        if (index >= Avatars.Count)
        {
            index = 0;
        }

        SelectAvatar(index);
    }

    public void PrevAvatar()
    {
        int index = curAvatarIdx - 1;
        if (index < 0)
        {
            index = Avatars.Count - 1;
        }

        SelectAvatar(index);
    }

    public void Dance()
    {
        if (Avatars[curAvatarIdx].TryGetComponent<Animator>(out Animator avatarAnimator))
        {
            avatarAnimator.SetFloat("DanceValue", Random.value);
            avatarAnimator.SetTrigger("DanceTrigger");
        }
    }
}
