using UnityEngine;
using System.Collections;

public class PlayerTankController : MonoBehaviour {

	public GameObject Bullet;

	private Transform Turret;
	private Transform bulletSpawnPoint;
	private float curSpeed, targetSpeed, rotSpeed;
	private float turretRotSpeed = 10.0f;
	private float maxForwardSpeed = 15.0f;
	private float maxBackwardSpeed = -15.0f;

	protected float shootRate = 0.5f;
	protected float elapsedTime;
	
	// Use this for initialization
	void Start () {
		// tank setting
		rotSpeed = 150.0f;

		Turret = gameObject.transform.GetChild(0).transform;
		bulletSpawnPoint = Turret.GetChild(0).transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateWeapon();
		UpdateControl();
	}

	void UpdateWeapon() 
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			elapsedTime += Time.deltaTime;

			if (elapsedTime >= shootRate) {
				// time reset
				elapsedTime = 0.0f;

				// create bullet
				Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
			}
		}
	}

	void UpdateControl() {
		Plane playerPlane = new Plane(Vector3.up, transform.position + Vector3.zero);

		Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.Log("mouse pos : "+Input.mousePosition.ToString());
		float HitDist = 0;

		if (playerPlane.Raycast(RayCast, out HitDist)) {
			Debug.Log("RayCast TRUE");
			Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

			Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);

			Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, 
			                                             targetRotation,
			                                             Time.deltaTime * turretRotSpeed);
		}

		if (Input.GetKey(KeyCode.W)) {
			targetSpeed = maxForwardSpeed;
		}else if (Input.GetKey(KeyCode.S)) {
			targetSpeed = maxBackwardSpeed;
		}else{
			targetSpeed = 0;
		}

		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
		}else if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
		}

		curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);

		transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
	}

}
