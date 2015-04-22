using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameObject Explosion;

	public float Speed = 10.0f;
	public float LifeTime = 3.0f;
	public int damage = 50;
	
	void Start () {
		Destroy(gameObject, LifeTime);	
	}

	void Update () {
		transform.position += (transform.forward * Speed) * Time.deltaTime;
	}

	void OnCollisionEnter(Collision collision) {
		//ContactPoint contact = collision.contacts[0];
		//if (Explosion != null) Instantiate(Explosion, contact.point, Quaternion.identity);
		
		if (collision.transform.tag == "Player") {
			Destroy(gameObject);
		}
	}
}
