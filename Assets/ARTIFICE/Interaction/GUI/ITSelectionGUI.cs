/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Sch?nauer, Georg Gerstweiler, Hannes Kaufmann
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
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to show a GUI to select a IT by ARToolkitMarker during runtime. 
/// </summary>
public class ITSelectionGUI : MonoBehaviour
{
	public GUIStyle vrue11Style;
	protected string _vh;
	protected string _triggerMarker;
	
	/* ------------------ VRUE Tasks START --------------------------
	* Place required member variables here
	----------------------------------------------------------------- */
	
	private ObjectSelectionBase[] interactionTechniques;
	private ObjectSelectionBase currentTechnique = null;
	private ObjectSelectionBase newTechnique = null;
	private Dictionary<string, ObjectSelectionBase> itDictionary = new Dictionary<string, ObjectSelectionBase> ();
	private GameObject virtualHand;
	private bool itSelectionEnabled = false;
	private bool itSelected = false;
	
	// ------------------ VRUE Tasks END ----------------------------
	
	/// <summary>
	/// Set StartUp Data. Method is called by OnEnable Unity Callback
	/// Must be overwritten in deriving class
	/// </summary>
	protected virtual void StartUpData ()
	{	
		// name of interaction object in Unity Hierarchy
		_vh = "VirtualHand";
		
		// name of trigger marker
		_triggerMarker = "Marker2";
	}
	
	/// <summary>
	/// </summary>
	void OnEnable ()
	{	
		// set init data
		StartUpData ();
		Debug.Log ("IT Selection GUI enabled");
		
		/* ------------------ VRUE Tasks START --------------------------
		* find ITs (components) attached to interaction game object
		* if none is attached, manually attach 3 ITs to interaction game object
		* initially det default IT
		----------------------------------------------------------------- */
		
		virtualHand = GameObject.Find (_vh);
		interactionTechniques = virtualHand.GetComponents<ObjectSelectionBase> ();
		
		if (interactionTechniques.Length == 0) {
			virtualHand.AddComponent<VirtualHandInteraction> (); 
			virtualHand.AddComponent<GoGoInteraction> (); 
			virtualHand.AddComponent<HomerInteraction> (); 
			interactionTechniques = virtualHand.GetComponents<ObjectSelectionBase> ();
		}
		
		itDictionary.Add ("Marker4", interactionTechniques [0]);
		itDictionary.Add ("Marker0", interactionTechniques [1]);
		itDictionary.Add ("Marker5", interactionTechniques [2]);
		
		enableInteractionTechnique (interactionTechniques [0]);
		
		// ------------------ VRUE Tasks END ----------------------------
	}
	
	private void enableInteractionTechnique (ObjectSelectionBase it)
	{
		disableAllTechniques ();
		it.enabled = true;
		currentTechnique = it;
	}
	
	private void disableAllTechniques ()
	{
		foreach (ObjectSelectionBase it in interactionTechniques) {
			it.enabled = false;
		}
	}
	
	
	/// <summary>
	/// Unity Callback
	/// OnGUI is called every frame for rendering and handling GUI events.
	/// </summary>
	void OnGUI ()
	{
		/* ------------------ VRUE Tasks START --------------------------
		* check if ITs are available
		* if trigger marker is visible and no objects are currently selected by interaction game object show GUI
		* depending on visible marker switch through availabe ITs
		* implement user confirmation and set selected IT only if user has confirmed it
		* disable the GUI if virtual hand has selected objects and if user has confirmend an IT
		----------------------------------------------------------------- */
		
		Debug.Log ("Selection Enabled: " + itSelectionEnabled);
		
		if (interactionTechniques.Length == 0) {
			return;
		}
		
		itSelected = currentTechnique.getSelectionState ();
		String marker = gameObject.GetComponent<MultiMarkerSwitch> ().GetFaceFront ();
		
		if (marker == _triggerMarker && !itSelectionEnabled) {
			itSelectionEnabled = true;
		} else if (itSelectionEnabled && Input.GetButtonUp ("Fire2")) {
			enableInteractionTechnique (newTechnique);
			itSelectionEnabled = false;
		}
		
		if (!itSelected && itSelectionEnabled) {
			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, 20, 300, 500));
			
			ObjectSelectionBase temp;
			if (itDictionary.TryGetValue (marker, out temp)) {
				newTechnique = temp;
				GUI.Box (new Rect (0, 0, 300, 50), newTechnique.GetType ().ToString (), vrue11Style);
			}
			else {
				GUI.Box (new Rect (0, 0, 300, 50), currentTechnique.GetType ().ToString (), vrue11Style);
			}
			
			GUI.EndGroup ();
		}
		
		// ------------------ VRUE Tasks END ----------------------------
	}
	
}
