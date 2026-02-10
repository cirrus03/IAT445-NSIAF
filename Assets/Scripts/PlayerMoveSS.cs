using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveSS : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
            return;
        Vector2 dir = InputManager.GetInstance().GetMoveDirection();
        float input = dir.x;
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);
    }

}
