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
/// Class to select and manipulate scene objects with HOMER interaction technique (IT). 
/// 
/// HOMER is a 1st person view IT
/// </summary>
public class HomerInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement Homer interaction technique
	----------------------------------------------------------------- */
	
	private GameObject tracker = null;
	private GameObject torso = null;
	private bool singleSelection = false;
	private LineRenderer lineRenderer = null;
	private Vector3 rayCastHit;
	private bool calculateInitalPositions = true;
	private float initalDistanceTorsoHand = 0.0f;
	private float initalDistanceTorsoObject = 0.0f;
	
	public void Start ()
	{
		tracker = GameObject.Find ("TrackerObject");
		torso = GameObject.Find ("InteractionOrigin");
		
		Debug.Log ("Start HOMER");
	}
	
	public void OnEnable ()
	{
		if (gameObject.GetComponent<LineRenderer> () != null) {
			return;
		}
		
		gameObject.AddComponent<LineRenderer> ();
		lineRenderer = gameObject.GetComponent<LineRenderer> ();
		lineRenderer.SetWidth (0.2f, 0.2f);
		lineRenderer.SetColors (Color.magenta, Color.magenta);
	}
	
	public void onDisable ()
	{
		if (gameObject.GetComponent<LineRenderer> () == null) {
			return;
		}
		
		Destroy (gameObject.GetComponent<LineRenderer> ());
	}
	
	protected override void UpdateSelect ()
	{
		if (!tracker) {
			Debug.Log ("No GameObject with name - TrackerObject - found in scene");
			return;
		}
		
		TrackBase trackBase = tracker.transform.parent.GetComponent<TrackBase> ();
		if (!trackBase.isTracked ()) {
			lineRenderer.enabled = false;
			trackBase.setVisability (gameObject, false);
			removePreviousColidees ();
			selected = false;
			return;
		}
		
		
		if (Input.GetKeyDown ("m")) {  
			singleSelection = !singleSelection;
		}
		
		if (!selected) {
			if (isOwnerCallback ()) {
				lineRenderer.enabled = true;
				this.performRayCasting ();
			}
		} else {
			if (calculateInitalPositions) {
				initalDistanceTorsoHand = Vector3.Distance (tracker.transform.position, torso.transform.position);
				initalDistanceTorsoObject = Vector3.Distance (torso.transform.position, calculateHitPoint ());
				calculateInitalPositions = false;
			}
			
			float distanceTorsoHand = Vector3.Distance (tracker.transform.position, torso.transform.position);
			Vector3 vecTorsoHand = (tracker.transform.position - torso.transform.position).normalized;
			float scalingFactor = distanceTorsoHand * (initalDistanceTorsoObject / initalDistanceTorsoHand);
			
			Vector3 newPosition = torso.transform.position + Vector3.Scale (vecTorsoHand, new Vector3 (scalingFactor, scalingFactor, scalingFactor));
			
			trackBase.setVisability (this.gameObject, true);
			this.transform.position = newPosition;
			this.transform.rotation = tracker.transform.rotation;
			
			this.transformInter (this.transform.position, this.transform.rotation);
			lineRenderer.enabled = false;
		}
		
	}
	
	private void performRayCasting ()
	{
		RaycastHit[] hits = new RaycastHit[1];
		Vector3 rayDirection = (tracker.transform.position - torso.transform.position).normalized;
		Ray ray = new Ray (tracker.transform.position, rayDirection);
		
		lineRenderer.SetPosition (0, ray.origin);
		lineRenderer.SetPosition (1, Vector3.Scale (rayDirection, new Vector3 (1000.0f, 1000.0f, 1000.0f)));
		
		hits = Physics.RaycastAll (tracker.transform.position, rayDirection);
		removePreviousColidees ();
		
		GameObject collidee = null;
		foreach (RaycastHit hit in hits) {
			
			if (singleSelection && collidees.Count > 0) {
				continue;
			}
			
			collidee = hit.collider.gameObject.gameObject;
			
			if (hasObjectController (collidee)) {
				collidees.Add (collidee.GetInstanceID (), collidee);
				collidee.renderer.material.color = Color.blue;
			}
		}                       
	}
	
	private void removePreviousColidees ()
	{
		foreach (GameObject collidee in collidees.Values) {
			collidee.renderer.material.color = Color.white;
		}
		
		collidees.Clear ();
	}
	
	private Vector3 calculateHitPoint ()
	{
		Vector3 hitPoint = Vector3.zero;
		
		foreach (GameObject collidee in collidees.Values) {
			hitPoint += collidee.transform.position;
		}
		
		return hitPoint / collidees.Count;
	}
	
	
	// ------------------ VRUE Tasks END ----------------------------
}