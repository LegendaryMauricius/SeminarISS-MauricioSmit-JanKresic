using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileController : MonoBehaviour
{
    public GameObject myExplosion;

    private Rigidbody rigidbody;
    private ParticleSystem particleSystem;
    private AudioSource sound;

    float speed = 0;
    float acceleration = 25f;
    float gravity = 9.81f;
    float drag = 2.5f;
    float maxSpeed = 40;
    float rotateSpeed = 3.1415f * 2.0f;
    bool launched = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();
        sound = gameObject.GetComponent<AudioSource>();

        rotateSpeed = 3.1415f * 1.1f;
        launched = false;
        /*transform.position = new Vector3(Random.Range(-15, 15), 2, Random.Range(-15, 15));
        transform.position.Set(
            transform.position.x, 
            Terrain.activeTerrain.SampleHeight(transform.position) + 2,
            transform.position.z);*/
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
            Vector3 down = new Vector3(0, -1, 0);

            // calc new rotation
            Vector3 baseDir = new Vector3(0, 1, 0);
            Vector3 targetDir = targetPos - transform.position;
            //Debug.Log("TargetPos:" + targetPos + "  TargetDir:" + targetDir);
            Vector3 currentDir = transform.rotation * baseDir;
            Vector3 newDir = Vector3.RotateTowards(
                currentDir, targetDir,
                rotateSpeed * rigidbody.velocity.magnitude / maxSpeed * Time.deltaTime, 2
            );

            rigidbody.rotation = Quaternion.FromToRotation(baseDir, newDir);
            //rigidbody.angularVelocity = Quaternion.FromToRotation(currentDir, newDir).eulerAngles;
            rigidbody.angularVelocity = new Vector3(0, 0, 0);

            speed += acceleration * Time.deltaTime;
            if (speed > maxSpeed) speed = maxSpeed;

            // move
            rigidbody.freezeRotation = false;
            //rigidbody.position += transform.rotation * baseDir * speed * Time.deltaTime;
            //rigidbody.velocity = rigidbody.rotation * baseDir * speed;
            //rigidbody.velocity += down * gravity * Time.deltaTime;
            rigidbody.velocity -= rigidbody.velocity * drag * Time.deltaTime;
            rigidbody.velocity += rigidbody.rotation * baseDir * acceleration * Time.deltaTime;
            if (rigidbody.velocity.magnitude > maxSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
        else
        {
            Vector3 launcherOffset = new Vector3(0, 1.4f, 0);

            GameObject launcher = GameObject.FindGameObjectWithTag("Launcher");
            rigidbody.freezeRotation = true; ;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
            rigidbody.position = launcher.transform.position + launcher.transform.rotation * launcherOffset;
            rigidbody.rotation = new Quaternion(launcher.transform.rotation.x, launcher.transform.rotation.y, launcher.transform.rotation.z, launcher.transform.rotation.w);
            /*transform.position = launcher.transform.position + launcher.transform.rotation * launcherOffset;
            transform.rotation = new Quaternion(launcher.transform.rotation.x, launcher.transform.rotation.y, launcher.transform.rotation.z, launcher.transform.rotation.w);*/
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "MissileTarget" || other.gameObject.tag == "Ground")
        {
            Instantiate(myExplosion, rigidbody.position, Quaternion.identity);
            /*sound.clip = myExplodeSFX;
            sound.Play();*/

            //Destroy(gameObject);
            launched = false;
            gameObject.SetActive(false);
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

    public void reset()
    {
        launched = false;
        gameObject.SetActive(true);
        particleSystem.Stop();
        sound.Stop();
    }
}
