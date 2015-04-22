using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	// third view point var
	public float distance = 10.0f;
	public float height = 5.0f;

	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;

	public GameObject target; // player
	
	void LateUpdate() {
		ThirdView();
	}

	void ThirdView() {
		if (target == null) {
			target = GameObject.FindWithTag("Player");
		}else{
			float wantedRotationAngle = target.transform.eulerAngles.y;
			float wantedHeight = target.transform.position.y + height;

			float currentRotationAngle = transform.eulerAngles.y;
			float currentHeight = transform.position.y;

			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

			// player position
			transform.position = target.transform.position;
			// move back
			transform.position -= currentRotation * Vector3.forward * distance;

			transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
			transform.LookAt(target.transform);
		}
	}
}
