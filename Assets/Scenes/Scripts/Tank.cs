using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public Vector3 m_Target = new Vector3(95, 28, 145);

    public float m_Speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((m_Target - GetComponent<Transform>().position).magnitude > 0.1f) 
        {
            GetComponent<Transform>().position += (m_Target - GetComponent<Transform>().position).normalized * m_Speed * Time.deltaTime;
        }
    }
}
