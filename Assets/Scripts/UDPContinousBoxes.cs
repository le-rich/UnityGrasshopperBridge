using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;



public class UDPContinousBoxes : MonoBehaviour
{


	private string IP;  
	public int port;  
	public bool mouseOverBox = false;
	GameObject lastHitBox;
	
	// "connection" things
	IPEndPoint remoteEndPoint;
	UdpClient client;

	// Use this for initialization
	void Start ()
	{

		IP = "127.0.0.1";
		port = 8051;
		

		remoteEndPoint = new IPEndPoint (IPAddress.Parse (IP), port);
		client = new UdpClient ();
		
		// status
		print ("Sending to " + IP + " : " + port);
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (!Input.GetMouseButton (1)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				if (lastHitBox != hit.collider.gameObject) {
					if (lastHitBox != null)
						lastHitBox.GetComponent<MeshRenderer> ().enabled = true;
					lastHitBox = hit.collider.gameObject;
					lastHitBox.GetComponent<MeshRenderer> ().enabled = false;
				}
				mouseOverBox = true;
				String msg = "";
				msg = hit.collider.gameObject.transform.position.ToString () + ";";
				msg = msg + hit.collider.gameObject.transform.localScale.ToString () + ";";
				msg = msg + hit.point.ToString ();
				//print (msg);
				sendString (msg);
			} else {
				mouseOverBox = false;
				if (lastHitBox != null)
					lastHitBox.GetComponent<MeshRenderer> ().enabled = true;
				lastHitBox = null;
			}
		}
	}

	// a funtion to send data via UDP
	private void sendString (string message)
	{
		try {

			
			// encode string to UTF8-coded bytes
			byte[] data = Encoding.UTF8.GetBytes (message);
			
			// send the data
			client.Send (data, data.Length, remoteEndPoint);

		} catch (Exception err) {
			print (err.ToString ());
		}
	}
}
