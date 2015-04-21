using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {

	protected static int ID = 0;

	protected Transform playerTransform;
	protected Vector3 destPos; // next dest pos
	protected GameObject[] pointList; // scout point list
	protected float shootRate;
	protected float elapsedTime;

	public Transform turret { get; set; }
	public Transform bulletSpawnPoint { get; set; }

	protected virtual void Initialize() {}
	protected virtual void FSMUpdate() {}
	protected virtual void FSMFixedUpdate() {}
	
	void Start() {
		++ID;
		Initialize();
	}

	void Update() {
		FSMUpdate();
	}

	void FixedUpdate() {
		FSMFixedUpdate();
	}
}
