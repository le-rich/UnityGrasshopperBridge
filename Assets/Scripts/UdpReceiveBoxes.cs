﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;


using System.Net.Sockets;
using System.Threading;






public class UdpReceiveBoxes : MonoBehaviour
{
	// read Thread
	Thread readThread;

    // udpclient object

    UdpClient socket;

	// port number
	public int port = 8050;
	
	// storage variable for the UDP packets
	public string lastReceivedPacket = "";
	string previousPacket = "";

	bool messageChanged = false;


	public GameObject boxprefab;
	public Transform hoverBoxHolder;
	private Transform[] boxes;
	private UDPContinousBoxes boxSendScript;
	

	void Start ()
	{
		// create a thread for reading UDP messages
		readThread = new Thread (new ThreadStart (ReceiveData));
		readThread.IsBackground = true;
		readThread.Start ();
		//first create the 4 boxes in a container transform and hide them
		boxes = new Transform[4];
		GameObject tmp;
		for (int i = 0; i < 4; i++) {
			tmp = Instantiate (boxprefab, Vector3.zero, Quaternion.identity) as GameObject;
			tmp.transform.parent = hoverBoxHolder;
			tmp.GetComponent<BoxCollider> ().enabled = false;
			boxes [i] = tmp.transform;

		}
		hoverBoxHolder.gameObject.SetActive (false);
		boxSendScript = Camera.main.GetComponent<UDPContinousBoxes> ();
	}
	

	void Update ()
	{

		//code for creating the 4 boxes 

		//if the message has changed and the user is over a box with the mouse cursor
		// then modify the 4 boxes with the new coordiantes and scale
		if (messageChanged && boxSendScript.mouseOverBox) {
			hoverBoxHolder.gameObject.SetActive (true);
			parseDataToBoxes (lastReceivedPacket);
			messageChanged = false;
		}
		//if user is not over a box the deactivate(hide) the 4 boxes
		if (!boxSendScript.mouseOverBox)
			hoverBoxHolder.gameObject.SetActive (false);

	
	}
	
	// Unity Application Quit Function
	void OnApplicationQuit ()
	{
		stopThread ();
	}
	
	// Stop reading UDP messages
	private void stopThread ()
	{
		if (readThread.IsAlive) {
			readThread.Abort ();
		}

        socket.Close();

	}
	
	// receive thread function
	private void ReceiveData ()
	{


        socket = new UdpClient(port);


		while (true) {
			try {
				// receive bytes

				IPEndPoint anyIP = new IPEndPoint (IPAddress.Any, 0);

                byte[] data = socket.Receive (ref anyIP);

				// decode UTF8-coded bytes to text format
				string text = Encoding.UTF8.GetString (data);

                // show received message
                print(">> " + text);
				
				// store new message as latest message
				previousPacket = lastReceivedPacket;
				lastReceivedPacket = text;
				if (lastReceivedPacket != previousPacket)
					messageChanged = true;
            }
            catch (Exception err) {
				print (err.ToString ());
			}
		}
	}

	//parse the string message from Grasshopper into coordiantes and dimensions for the 4 boxes
	private void parseDataToBoxes (string msg)
	{

		try {
			string[] data = msg.Split (';');
			for (int i = 0; i < 4; i ++) {

				string[] strScales = data [i].Split (',');
				string[] strCoords = data [i + 4].Split (',');

				boxes [i].position = new Vector3 (float.Parse (strCoords [0]), float.Parse (strCoords [1]), float.Parse (strCoords [2]));
				boxes [i].localScale = new Vector3 (float.Parse (strScales [0]), float.Parse (strScales [1]), float.Parse (strScales [2]));
			}
		} catch (Exception err) {
			print (err.ToString ());
		}
	}
}
