﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worldmanager : MonoBehaviour {

	private Text HEALTH;

	// Use this for initialization
	void Start () {
		HEALTH = GameObject.FindGameObjectWithTag ("health").GetComponent<Text> ();
		HEALTH.text = System.Convert.ToString (Teamstate.teamstate.getHealth (1));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
