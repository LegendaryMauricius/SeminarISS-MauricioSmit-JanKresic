using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileController : MonoBehaviour
{
    public GameObject myExplosion;

    private Rigidbody rigidbody;
    private ParticleSystem particleSystem;
    private AudioSource sound;

    float speed = 7;
    float rotateSpeed = 3.1415f / 5;
    bool launched = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();
        sound = gameObject.GetComponent<AudioSource>();

        speed = 7;
        rotateSpeed = 3.1415f / 2;
        launched = false;
        transform.position = new Vector3(Random.Range(-15, 15), 2, Random.Range(-15, 15));
        transform.position.Set(
            transform.position.x, 
            Terrain.activeTerrain.SampleHeight(transform.position) + 2,
            transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            launch();
        }
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
        if (other.gameObject.tag == "MissileTarget" || other.gameObject.tag == "Ground")
        {
            Instantiate(myExplosion, rigidbody.position, Quaternion.identity);
            /*sound.clip = myExplodeSFX;
            sound.Play();*/
            
            Destroy(gameObject);
            /*Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Explosion.prefab", typeof(GameObject));
            GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;*/
        }

    }

    public void launch()
    {
        launched = true;
        particleSystem.Play();
        sound.Play();
    }
}
