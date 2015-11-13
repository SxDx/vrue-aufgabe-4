﻿/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Schönauer, Georg Gerstweiler, Hannes Kaufmann
 * mossel | schoenauer | gerstweiler | kaufmann @ims.tuwien.ac.at
 * Interactive Media Systems Group, Vienna University of Technology, Austria
 * www.ims.tuwien.ac.at
 * 
 * ====================================================================================
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * =====================================================================================
 */
using UnityEngine;
using System.Collections;

/// <summary>
/// Class to select and manipulate scene objects with gogo interaction technique (IT). 
/// 
/// GoGo is a 1st person view IT
/// </summary>
public class GoGoInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement GoGo interaction technique
	----------------------------------------------------------------- */
	GameObject tracker = null;
	GameObject torso = null;
	private float armLenght = 0.0f;
	private float thresholdDistance = 0.0f;
	private float gogoFactor = 0.0f;
	
	public void Start ()
	{
		tracker = GameObject.Find ("TrackerObject");
		torso = GameObject.Find ("InteractionOrigin");
		
		armLenght = 0.6f * 20.0f; // 60cm * scaling
		thresholdDistance = (2 / 3) * this.armLenght; // 2/3 of arm lenght
		gogoFactor = (1 / 6) + 1.0f;
		
		Debug.Log ("Start Go-Go");
	}
	
	protected override void UpdateSelect ()
	{
		if (!tracker) {
			Debug.Log ("No GameObject with name - TrackerObject - found in scene");
			return;
		}
		
		TrackBase trackBase = tracker.transform.parent.GetComponent<TrackBase> ();
		if (!trackBase.isTracked ()) {
			trackBase.setVisability (gameObject, false);
			return;
		}
		
		trackBase.setVisability (this.gameObject, true);
		
		float distance = Vector3.Distance (tracker.transform.position, torso.transform.position);
		Vector3 vecDistance = (tracker.transform.position - torso.transform.position);
		
		float gogoScaling = 0.0f;
		if (distance > this.thresholdDistance) {
			gogoScaling = gogoFactor * Mathf.Pow ((distance - thresholdDistance), 2.0f);
		}
		
		this.transform.position = tracker.transform.position + (gogoScaling * vecDistance.normalized);
		this.transform.rotation = tracker.transform.rotation;
		
		if (selected) {
			this.transformInter (this.transform.position, this.transform.rotation);
		}
	} 
	
	
	// ------------------ VRUE Tasks END ----------------------------
	
	/// <summary>
	/// Callback
	/// If our selector-Object collides with anotherObject we store the other object 
	/// 
	/// For usability purpose change color of collided object
	/// </summary>
	/// <param name="other">GameObject giben by the callback</param>
	public void OnTriggerEnter (Collider other)
	{		
		if (isOwnerCallback ()) {
			GameObject collidee = other.gameObject;
			
			if (hasObjectController (collidee)) {
				
				collidees.Add (collidee.GetInstanceID (), collidee);
				//Debug.Log(collidee.GetInstanceID());
				
				// change color so user knows of intersection
				collidee.renderer.material.SetColor ("_Color", Color.blue);
			}
		}
	}
	
	/// <summary>
	/// Callback
	/// If our selector-Object moves out of anotherObject we remove the other object from our list
	/// 
	/// For usability purpose change color of collided object
	/// </summary>
	/// <param name="other"></param>
	public void OnTriggerExit (Collider other)
	{
		if (isOwnerCallback ()) {
			GameObject collidee = other.gameObject;
			
			if (hasObjectController (collidee)) {
				collidees.Remove (collidee.GetInstanceID ());
				
				// change color so user knows of intersection end
				collidee.renderer.material.SetColor ("_Color", Color.white);
			}
		}
	}
}