using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void StartTexting()
    {
        animator.SetBool("IsTexting", true);
    }

    public void StopTexting()
    {
        animator.SetBool("IsTexting", false);
    }
}
