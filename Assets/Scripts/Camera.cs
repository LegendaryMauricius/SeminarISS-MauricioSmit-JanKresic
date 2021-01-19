using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(0, - rotateSpeed * Time.deltaTime, 0));
        }
       
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(-rotateSpeed * Time.deltaTime, 0, 0));
        }
       
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0, 0));
        }
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
    }
}
