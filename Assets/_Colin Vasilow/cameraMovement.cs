using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class cameraMovement : MonoBehaviour
{
    private Vector2 moveInputs;
    public CharacterController controller;
    public Slider angl;
    public float speed = 6f;

    void Update()
    {
        Vector3 direction = new Vector3 (moveInputs.x, 0f, moveInputs.y).normalized;

        if (direction.magnitude > 0.0f) {
            controller.Move(direction * speed * Time.deltaTime );
        }
    }

    void OnMove(InputValue value){
        moveInputs = value.Get<Vector2>();
    }

    public void UpdateAngle(float angle){

    }
}
