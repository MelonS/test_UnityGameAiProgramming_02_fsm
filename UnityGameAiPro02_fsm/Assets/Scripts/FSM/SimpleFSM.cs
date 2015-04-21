using UnityEngine;
using System.Collections;

public class SimpleFSM : FSM {

	public enum FSMState {
		None,
		Patrol,
		Chase,
		Attack,
		Dead,
	}

	public FSMState curState;

	private float curSpeed;
	private float curRotSpeed;

	public GameObject Bullet;
	public Rigidbody rigBody;

	private bool isDead;
	private int health;

	protected override void Initialize() 
	{
		curState = FSMState.Patrol;
		curSpeed = 1.0f;
		curRotSpeed = 2.0f;
		isDead = false;
		elapsedTime = 0.0f;
		shootRate = 3.0f;
		health = 100;

		pointList = GameObject.FindGameObjectsWithTag("WanderPoint");

		FindNextPoint();

		GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
		playerTransform = objPlayer.transform;

		if (!playerTransform)
			Debug.LogError("Player does't exist");
		else
			print("find player");

		turret = gameObject.transform.GetChild(0).transform;
		bulletSpawnPoint = turret.GetChild(0).transform;
	}

	protected override void FSMUpdate()
	{
		switch(curState) 
		{
			case FSMState.Patrol: UpdatePatrolState(); break;
			case FSMState.Chase: UpdateChaseState(); break;
			case FSMState.Attack: UpdateAttackState(); break;
			case FSMState.Dead: UpdateDeadState(); break;
		}

		elapsedTime += Time.deltaTime;

		if (health <= 0)
			curState = FSMState.Dead;
	}

	protected void FindNextPoint() {
		print("["+ID+"] Finding next point");
		int rndIndex = Random.Range(0, pointList.Length);
		float rndRadius = 10.0f;
		Vector3 rndPosition = Vector3.zero;
		destPos = pointList[rndIndex].transform.position + rndPosition;

		if (IsInCurrentRange(destPos)) {
			rndIndex = Random.Range(0, pointList.Length);
			rndPosition = new Vector3(Random.Range(-rndRadius,rndRadius), 
			                          0.0f, 
			                          Random.Range(-rndRadius,rndRadius));
			destPos = pointList[rndIndex].transform.position + rndPosition;
		}
	}

	protected bool IsInCurrentRange(Vector3 pos) {
		float xPos = Mathf.Abs(pos.x - transform.position.x);
		float zPos = Mathf.Abs(pos.z - transform.position.z);

		if (xPos <= 50 && zPos <= 50)
			return true;

		return false;
	}

	protected void UpdatePatrolState() {
		float destDist = Vector3.Distance(transform.position, destPos);
		Debug.Log("dest Dist :"+destDist);
		if (destDist <= 10.0f) 
		{
			print("["+ID+"] Reached to the destination point\n"+
			      "calculating the next point");

			FindNextPoint();

		}else if (Vector3.Distance(transform.position, playerTransform.position) <= 30.0f) {
			print("["+ID+"] Switch to Chase Position");
			curState = FSMState.Chase;
		}

		// rotate to dest
		Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation,
		                                      targetRotation,
		                                      Time.deltaTime * curRotSpeed);

		// go forward
		transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
	}

	protected void UpdateChaseState() {
		destPos = playerTransform.position;

		float dist = Vector3.Distance(transform.position, playerTransform.position);

		if (dist <= 20.0f) {
			curState = FSMState.Attack;
		}else if (dist >= 30.0f) {
			curState = FSMState.Patrol;
		}

		transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
	}

	protected void UpdateAttackState() {
		destPos = playerTransform.position;

		float dist = Vector3.Distance(transform.position, playerTransform.position);

		if (dist >= 20.0f && dist < 30.0f) {
			Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      targetRotation,
			                                      Time.deltaTime * curRotSpeed);

			transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
			curState = FSMState.Attack;
		}else if (dist >= 30.0f) {
			curState = FSMState.Patrol;
		}

		Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
		turret.rotation = Quaternion.Slerp(turret.rotation, 
		                                   turretRotation, 
		                                   Time.deltaTime * curRotSpeed);

		ShootBullet();
	}

	private void ShootBullet() {
		if (elapsedTime >= shootRate) {
			Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
			elapsedTime = 0.0f;
		}
	}

	protected void UpdateDeadState() {
		if (!isDead) {
			isDead = true;
			Explode();
		}
	}

	protected void Explode() {
		float rndX = Random.Range(10.0f, 30.0f);
		float rndZ = Random.Range(10.0f, 30.0f);
		for (int i = 0; i < 3; i++) {
			rigBody.AddExplosionForce(10000.0f,
			                          transform.position - new Vector3(rndX, 10.0f, rndZ),
			                          40.0f,
			                          10.0f);
			rigBody.velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
		}

		Destroy(gameObject, 1.5f);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Bullet") {
			health -= collision.gameObject.GetComponent<Bullet>().damage;
		}
	}
}
