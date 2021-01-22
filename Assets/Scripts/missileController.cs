using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileController : MonoBehaviour
{
    public GameObject myExplosion;
    public Vector3 targetPos;

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
            Vector3 baseDir = new Vector3(0, 1, 0);// smjer sa pocetnom rotacijom
            Vector3 targetDir = targetPos - transform.position;// smjer prema meti
            Vector3 currentDir = transform.rotation * baseDir;// trenutni smjer

            // Izracunajmo novi smjer zakrenut za brzinu okretanja
            float rotateSpeedFactor = rigidbody.velocity.magnitude / maxSpeed;
            Vector3 newDir = Vector3.RotateTowards(
                current: currentDir,
                target: targetDir,
                maxRadiansDelta: rotateSpeed * rotateSpeedFactor * Time.deltaTime, 
                maxMagnitudeDelta: 2
            );

            // Prevedimo smjre u rotaciju
            rigidbody.rotation = Quaternion.FromToRotation(baseDir, newDir);

            // Sprijecimo physics engine da mijenja rotaciju
            rigidbody.freezeRotation = false;
            rigidbody.angularVelocity = new Vector3(0, 0, 0);

            // Promjenimo brzinu
            rigidbody.velocity -= rigidbody.velocity * drag * Time.deltaTime;// otpor zraka
            rigidbody.velocity += newDir * acceleration * Time.deltaTime;// ubrzanje
            if (rigidbody.velocity.magnitude > maxSpeed)// Ogranicimo brzinu
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
            other.gameObject.GetComponent<TankTargetController>().stop();
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
