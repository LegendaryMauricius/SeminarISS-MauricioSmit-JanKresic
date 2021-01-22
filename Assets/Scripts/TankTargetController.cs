using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTargetController : MonoBehaviour
{
    public Vector3 m_Target = new Vector3(95, 28, 145);
    public Vector3 StartingPos;
    public Quaternion StartingRot;
    public float m_Speed = 1.5f;
    public float m_RotSpeed = 30.0f;
    public float m_RotThreshold = 3.0f;

    bool moving = false;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartingPos = transform.position;
        StartingRot = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
                //Debug.Log("Moving: "+moving);
        if (moving)
        {
            if ((m_Target - rigidbody.transform.position).magnitude > 0.1f) 
            {
                Vector3 baseDir = new Vector3(0, 0, 1);
                Vector3 eulerRot = new Vector3(
                    rigidbody.rotation.eulerAngles.x < 180? rigidbody.rotation.eulerAngles.x : rigidbody.rotation.eulerAngles.x - 360,
                    rigidbody.rotation.eulerAngles.y < 180? rigidbody.rotation.eulerAngles.y : rigidbody.rotation.eulerAngles.y - 360,
                    rigidbody.rotation.eulerAngles.z < 180? rigidbody.rotation.eulerAngles.z : rigidbody.rotation.eulerAngles.z - 360
                );

                    Debug.Log("Euler: "+eulerRot);
                if (Mathf.Abs(eulerRot.z) < 45 && Mathf.Abs(eulerRot.x) < 75)
                {
                    float yawDif =  Quaternion.FromToRotation(
                        rigidbody.rotation * baseDir,
                        (m_Target - rigidbody.position).normalized
                    ).eulerAngles.y;

                    Debug.Log("Rot: "+yawDif);
                    if (Mathf.Abs(yawDif) < m_RotThreshold)
                        yawDif = 0;
                    
                    rigidbody.angularVelocity = new Vector3(
                        rigidbody.angularVelocity.x,
                        Mathf.Min(m_RotSpeed, Mathf.Max(-m_RotSpeed, yawDif)) * Mathf.Deg2Rad,
                        rigidbody.angularVelocity.z
                    );
                    Vector3 targetVel = rigidbody.rotation * baseDir * m_Speed;
                    rigidbody.velocity = new Vector3(targetVel.x, rigidbody.velocity.y, targetVel.z);
                }
            }
        }
        else
        {
            //rigidbody.velocity = new Vector3(0, 0, 0);
            //rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }
    }

    public void move()
    {
        moving = true;
    }

    public void stop()
    {
        moving = false;
    }

    public void reset()
    {
        moving = false;
        transform.position = StartingPos;
        transform.rotation = StartingRot;
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}
