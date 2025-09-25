using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            animator.Play("Idle");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            animator.Play("Runing");

        if (Input.GetKeyDown(KeyCode.Alpha4))
            animator.Play("Jump");

    }
}

