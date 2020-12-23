using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileController : MonoBehaviour
{
    public GameObject myExplosion;
    private Rigidbody rigidbody;

    float speed = 7;
    float rotateSpeed = 3.1415f / 5;
    bool launched = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        speed = 7;
        rotateSpeed = 3.1415f / 2;
        launched = true;
        transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (launched)
        {
            Vector3 targetPos = GameObject.FindGameObjectWithTag("MissileTarget").transform.position;

            // calc new rotation
            Vector3 baseDir = new Vector3(0, 1, 0);
            Vector3 targetDir = targetPos - transform.position;
            Debug.Log("TargetPos:" + targetPos + "  TargetDir:" + targetDir);
            Vector3 currentDir = transform.rotation * baseDir;
            Vector3 newDir = Vector3.RotateTowards(currentDir, targetDir, rotateSpeed * Time.deltaTime, 2);

            rigidbody.rotation = Quaternion.FromToRotation(baseDir, newDir);
            //rigidbody.angularVelocity = Quaternion.FromToRotation(currentDir, newDir).eulerAngles;
            rigidbody.angularVelocity = new Vector3(0, 0, 0);

            // move
            //rigidbody.position += transform.rotation * baseDir * speed * Time.deltaTime;
            rigidbody.velocity = rigidbody.rotation * baseDir * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MissileTarget")
        {
            Destroy(gameObject);
            Instantiate(myExplosion, rigidbody.position, Quaternion.identity);
            /*Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Explosion.prefab", typeof(GameObject));
            GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;*/
        }

    }
}
