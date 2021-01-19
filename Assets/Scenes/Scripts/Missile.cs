using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject myExplosion;

    private Rigidbody rigidbody;
    private AudioSource sound;

    public float speed;
    public float rotateSpeed;

    public float gravity;
    public float mass;

    bool launched = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        sound = gameObject.GetComponent<AudioSource>();

        launched = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            launched = true;
            sound.Play();
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(0, -rotateSpeed * Time.deltaTime, 0));
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0, 0));
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(-rotateSpeed * Time.deltaTime, 0, 0));
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<Transform>().Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
    }

    void FixedUpdate()
    {
        if (launched)
        {
            GetComponent<Transform>().Translate(0 , - mass * gravity * Time.deltaTime * Time.deltaTime / 2, -speed * Time.deltaTime);
            //GetComponent<Transform>().position -= (Vector3.forward + Vector3.left).normalized * speed * Time.deltaTime;
            //GetComponent<Transform>().position += Vector3.down * mass * gravity * Time.deltaTime * Time.deltaTime / 2;
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
}
