using UnityEngine;
using System.Collections;

public class RotateWithMouse : MonoBehaviour {

	public int speed =1;
	public float friction =1f;
	
	public float lerpSpeed = 1f;
	
	private float xDeg;
	
	private float yDeg;
	
	private Quaternion fromRotation;
	
	private Quaternion toRotation;

	void Update () {
			if(Input.GetMouseButton(1)) {
				
				xDeg -= Input.GetAxis("Mouse X") * speed * friction;
				yDeg += Input.GetAxis("Mouse Y") * speed * friction;
				fromRotation = transform.rotation;
				toRotation = Quaternion.Euler(yDeg,xDeg,0);
				transform.rotation = Quaternion.Lerp(fromRotation,toRotation,Time.deltaTime  * lerpSpeed);
			}
	}
}
