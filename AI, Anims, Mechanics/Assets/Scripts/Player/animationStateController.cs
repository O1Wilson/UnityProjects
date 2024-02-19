using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash; 
    int isRunningHash;
    int isJumpingHash;
    int isTalkingHash;
    int isRightHash;
    int isLeftHash;
    int isBackHash;

    void Start()
    {
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking"); //stores isWalking in a simpler data type | replaced "string"
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isTalkingHash = Animator.StringToHash("isTalking");
        isRightHash = Animator.StringToHash("isRight");
        isLeftHash = Animator.StringToHash("isLeft");
        isBackHash = Animator.StringToHash("isBack");

    }

    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isTalking = animator.GetBool(isTalkingHash);
        bool isRight = animator.GetBool(isRightHash);
        bool isLeft = animator.GetBool(isLeftHash);
        bool isBack = animator.GetBool(isBackHash);
        bool forwardPressed = Input.GetKey(KeyCode.W); //optimizes code by storing input in a variable
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetKey(KeyCode.Space);
        bool talkPressed = Input.GetKey(KeyCode.F);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool backPressed = Input.GetKey(KeyCode.S);

        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if (!isRunning && (forwardPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }
        
        if (isRunning && (!forwardPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }

        if (!isJumping && jumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
        }

        if (isJumping && !jumpPressed)
        {
            animator.SetBool(isJumpingHash, false);
        }

        if (!isTalking && talkPressed)
        {
            animator.SetBool(isTalkingHash, true);
        }

        if (isTalking && !talkPressed)
        {
            animator.SetBool(isTalkingHash, false);
        }

        if (!isRight && rightPressed)
        {
            animator.SetBool(isRightHash, true);
        }

        if (isRight && !rightPressed)
        {
            animator.SetBool(isRightHash, false);
        }

        if (!isLeft && leftPressed)
        {
            animator.SetBool(isLeftHash, true);
        }

        if (isLeft && !leftPressed)
        {
            animator.SetBool(isLeftHash, false);
        }

        if (!isBack && backPressed)
        {
            animator.SetBool(isBackHash, true);
        }

        if (isBack && !backPressed)
        {
            animator.SetBool(isBackHash, false);
        }
    }
}
