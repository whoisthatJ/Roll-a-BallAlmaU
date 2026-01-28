using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 1f;
    int coinsCollected;
    int coinsOnPlatform;
    public GameObject door;
    public Transform coinsParent;
    public bool invulnarable;
    public Material defaultMaterial;
    public Material fadeMaterial;
    MeshRenderer mr; 
    public BossBehaviour bossBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        //coinsOnPlatform = GameObject.FindGameObjectsWithTag("Coin").Length;
        coinsOnPlatform = coinsParent.childCount;
        
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.J))
        {
            bossBehaviour.StartBossFight();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //______dddddddddddddddddddd____ddddd___
        //00000011111111111111111111000011111000 GetKey
        //00000010000000000000000000000010000000 GetKeyDown
        //00000000000000000000000001000000001000 GetKeyUp

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = Camera.main.transform.right * x + Camera.main.transform.forward * z;
        rb.AddForce(direction * speed);
        /*if (Input.GetKey(KeyCode.D))
            rb.AddForce(Vector3.right * speed);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(Vector3.left * speed);
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(Vector3.forward * speed);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(Vector3.back * speed);*/
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy" && !invulnarable) {
            transform.localScale *= 0.9f;
            StartCoroutine(WaitAfterDamage());
        }
    }
    IEnumerator WaitAfterDamage() {
        invulnarable = true;
        for (int i = 0; i < 15; i++) {
            if (i % 2 == 0) {
                mr.material = fadeMaterial;
            }
            else {
                mr.material = defaultMaterial;
            }
            yield return new WaitForSeconds(0.1f*(i+1)*0.3f);
        }
        invulnarable = false;
        mr.material = defaultMaterial;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Coin") {
            other.gameObject.SetActive(false);
            coinsCollected++;
            if(coinsCollected == coinsOnPlatform)
                door.SetActive(false);
        }
    }

}
