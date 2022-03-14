using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    public float rotSpeed = 1f;
    public float moveSpeed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float rotation = h * rotSpeed * Time.deltaTime;
        this.transform.Rotate(0, rotation, 0);

        float v = Input.GetAxis("Vertical");
        if (v >= 0) {
            float movement = v * this.moveSpeed * Time.deltaTime;

            Debug.Log(movement);

            this.transform.Translate(0, 0, movement);
        }
    }
}
