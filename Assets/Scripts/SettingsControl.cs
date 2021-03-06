﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {

	public GameObject bottomLayer;
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;
	public GameObject bondLineOn;
	public GameObject lenJonesOn;
	public GameObject buckinghamOn;
	public GameObject nmOn;
	public GameObject sliderPanel;
	public GameObject graphOn;
	public GameObject graphPanel;
	//waiting for Brenner to be done
	public GameObject brennerOn;
	public AtomTouchGUI atomTouchGUI;

	public static bool mouseExitsSettingsPanel; //aka, pause the game

	private static Potential.potentialType currentPotentialType;
	private static bool simTypeChanged;
	
	private static bool gamePaused;
	private Toggle nmToggle;

	private bool doResume = false;
    public static bool GamePaused{
    	get { return gamePaused; }
       	//set { this._Name = value; }  
    }
	void Awake(){
		mouseExitsSettingsPanel = true;
		gamePaused = false;
		currentPotentialType = Potential.potentialType.LennardJones;
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		nmToggle = nmOn.GetComponent<Toggle>();
		simTypeChanged = false;
	}
	void Start () {
		
	}
	
	
	public void ResumeGame(){
		//Debug.Log("mio");
		settingsCanvas.SetActive(false);
		hudCanvas.SetActive(true);
		//resume
		Time.timeScale = 1.0f;

		//if sim type is changed, reset
		if(simTypeChanged){
			CreateEnvironment.myEnvironment.preCompute();
			atomTouchGUI.ResetAll();
			simTypeChanged = false;
		}
		gamePaused = false;
		StaticVariables.mouseClickProcessed = false;
	}

	public void PauseGame(){
		gamePaused = true;
		settingsCanvas.SetActive(true);
		hudCanvas.SetActive(false);
		//pause
		Time.timeScale = 0.0f;
	}

	

	public void OnClick_SettingsButton(){
		PauseGame();
	}

	

	public void OnToggle_Bondline(){
		StaticVariables.drawBondLines = bondLineOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_Graph(){
		Chart.show = graphOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_VolUnitNm(){
		if(nmToggle.isOn){
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Nanometer;
		}else{
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Angstrom;
		}
	}

	//checks if mouse clicks outside the settings, if so, exit settings and resume
	public void CheckExitSettings(){
		
		Vector3 mp = Input.mousePosition;

		RectTransform rt = settingsPanel.GetComponent<RectTransform>();
		if(mp.x > rt.anchorMin.x * Screen.width //lower left
			&& mp.x < rt.anchorMax.x * Screen.width //upper right
			&& mp.y > rt.anchorMin.y * Screen.height
			&& mp.y < rt.anchorMax.y * Screen.height){ 
			Debug.Log("IN RECT");

		}else{
			Debug.Log("OUT RECT");
			StaticVariables.mouseClickProcessed = true;
			ResumeGame();
		}
		
	}

	public void OnChange_SimType(){
		if(lenJonesOn.GetComponent<Toggle>().isOn){
			Potential.currentPotential = Potential.potentialType.LennardJones;
			if(currentPotentialType != Potential.potentialType.LennardJones){
				simTypeChanged = true;
			}
			currentPotentialType = Potential.potentialType.LennardJones;
		}else if(buckinghamOn.GetComponent<Toggle>().isOn){
			Potential.currentPotential = Potential.potentialType.Buckingham;
			if(currentPotentialType != Potential.potentialType.Buckingham){
				simTypeChanged = true;
			}
			currentPotentialType = Potential.potentialType.Buckingham;
		}
	}
}
