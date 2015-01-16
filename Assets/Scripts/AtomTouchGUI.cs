﻿/**
 * Class: AtomTouchGUI.cs
 * Created by: Justin Moeller
 * Description: The sole purpose of this class is to define the GUI of the game. Because of this,
 * most of the global variables are declared public to easily swap out the graphics of the UI.
 * There are a couple of weird quirks with the creation of the UI. The first is that the graph
 * is NOT defined in this class, it has its own class. This is because the function responsible
 * for creating the UI, OnGUI(), draws over OnPostRender, meaning it draws over the graph. To solve
/**
 * Class: AtomTouchGUI.cs
 * Created by: Justin Moeller
 * Description: The sole purpose of this class is to define the GUI of the game. Because of this,
 * most of the global variables are declared public to easily swap out the graphics of the UI.
 * There are a couple of weird quirks with the creation of the UI. The first is that the graph
 * is NOT defined in this class, it has its own class. This is because the function responsible
 * for creating the UI, OnGUI(), draws over OnPostRender, meaning it draws over the graph. To solve
 * this problem, the UI simply draws around the graph and the graph draws in the blank space. (sort of
 * like a cookie cutter) The second quirk is that, in order to get around Unity's restrictions of drawing
 * buttons, most of the buttons are drawn with a texture behind them and a blank button (with no text)
 * over the top of it. This is to provide the same functionality of the button, but get around Unity's
 * restrictions of drawing buttons
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class AtomTouchGUI : MonoBehaviour {
	
	//state of the UI
	private bool atomTouchActive = true;
	private bool toolbarActive = true;
	[HideInInspector]public bool dataPanelActive = false;
	private bool addAtomActive = true;
	private bool temperaturePanelActive = true;
	private bool volumePanelActive = true;
	private bool whiteCornerActive = false;
	private bool potentialsActive = false;
	private float oldTemperaure = -1;
	//plane materials
	public Material matPlane1;
	public Material matPlane1_5;
	public Material matPlane2;
	public Material matPlane2_5;
	public Material matPlane3;
	public Material matPlane3_5;
	public Material matPlane4;

	//textures for the UI
	public Texture lightBackground;
	public Texture darkBackground;
	public Texture darkBlueBackground;
	public Texture lightBlueBackground;
	public Texture whiteCornerArrow;
	public Texture downArrow;
	public Texture upArrow;
	//some references to the UI
	public GameObject hud;
	public GameObject timer;
	public GameObject tempSlider;//temperature
	public GameObject volSlider;//volume
	public GameObject bondLineBtn; 
	public GameObject selectAtomPanel;
	public GameObject selectAtomGroup;
	public GameObject settingsCanvas;
	public GameObject deselectButton;

	public Text selectAllText;
	private bool selectedAll;
	private bool settingsActive;
	//prefabs to spawn
	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	
	//reset button
	public Texture resetButtonUp;
	public Texture resetButtonDown;
	private bool resetPressed;
	private float resetTime;
	
	//snap camera button
	public Texture cameraButtonUp;
	public Texture cameraButtonDown;
	private bool cameraPressed;
	private float cameraTime;
	
	//bond line button
	public Texture bondLineUp;
	public Texture bondLineDown;
	
	//atom kick button
	public Texture atomKickUp;
	public Texture atomKickDown;
	private bool atomKickPressed;
	private float atomKickTime;
	
	//time button
	public Texture normalTimeButton;
	public Texture slowTimeButton;
	public Texture stoppedTimeButton;
	
	//red x button
	public Texture redXButton;
	
	//add atom buttons
	public Texture copperTexture;
	public Texture goldTexture;
	public Texture platinumTexture;
	public Texture copperTextureAdd;
	public Texture goldTextureAdd;
	public Texture platinumTextureAdd;
	public Texture garbageTexture;
	public Texture garbageTextureDown;
	private bool garbagePressed;
	private float garbageTime;
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	[HideInInspector]public bool addGraphicCopper;
	[HideInInspector]public bool addGraphicGold;
	[HideInInspector]public bool addGraphicPlatinum;
	public GUISkin sliderControls;
	private bool firstPass = true;
	
	[HideInInspector]public bool changingSlider = false;
	private float guiVolume;
	
	private int slowMotionFrames;
	
	public static StaticVariables.TimeSpeed currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
	
	private Slider tempSliderComponent;
	private Slider volSliderComponent;
	void Awake(){
		tempSliderComponent = tempSlider.GetComponent<Slider> ();
		volSliderComponent = volSlider.GetComponent<Slider>();
		//set slider range
		tempSliderComponent.minValue = StaticVariables.minTemp;
		tempSliderComponent.maxValue = StaticVariables.maxTemp;
		//tempSliderComponent.value = StaticVariables.defaultTemp;

		volSliderComponent.minValue = StaticVariables.minVol * 0.1f; //to nm
		volSliderComponent.maxValue = StaticVariables.maxVol * 0.1f; //to nm
		//volSliderComponent.value = StaticVariables.defaultVol;

		//tempSliderComponent.value = StaticVariables.defaultTemp;
		//volSliderComponent.value = (StaticVariables.maxVol-StaticVariables.defaultVol) * 0.1f;
		Atom.EnableSelectAtomGroup(false);
		settingsCanvas.SetActive(false);
		selectedAll = false;
		settingsActive = false;
		//Debug.Log("Settings canvas enabled: " + SettingsCanvas.activeSelf);
	}
	void Start () {
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
		guiVolume = myEnvironment.volume;	
	}
	
	//this function creates all UI elements in the game EXCEPT for the graph
	void OnGUI(){
		
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;

		if (sliderControls != null) {
			GUI.skin = sliderControls;
		}
		
		//create the "Atomtouch" menu
		GUIStyle buttonStyle = GUI.skin.label;
		Rect arrowBackgroundRect = new Rect (0.0f, 0.0f, Screen.width * .14f, Screen.height * .13f * .3f);
		Texture atomTouchArrow = atomTouchActive ? upArrow : downArrow;
		GUI.DrawTexture (arrowBackgroundRect, darkBackground);
		GUI.DrawTexture (new Rect (arrowBackgroundRect.width * .45f, 0.0f, 20.0f, arrowBackgroundRect.height), atomTouchArrow); 
		if (GUI.Button (arrowBackgroundRect, "", buttonStyle)) {
			atomTouchActive = !atomTouchActive;
		}
		
		Rect atomTouchRect = new Rect (0.0f, Screen.height * .13f * .3f, Screen.width * .14f, (Screen.height * .13f) - Screen.height * .13f * .3f);
		if (atomTouchActive) {
			GUIStyle textStyle = GUI.skin.label;
			textStyle.alignment = TextAnchor.MiddleCenter;
			textStyle.fontSize = 25;
			textStyle.normal.textColor = Color.white;
			GUI.DrawTexture (atomTouchRect, lightBackground);
			if (GUI.Button (atomTouchRect, "Atom Touch", textStyle)) {
				potentialsActive = !potentialsActive;
			}
			GUI.DrawTexture(new Rect(atomTouchRect.x + atomTouchRect.width - 20.0f, atomTouchRect.y + atomTouchRect.height - 20.0f, 20.0f, 20.0f), whiteCornerArrow);
			
			//create the dropdown of different potentials
			if(potentialsActive){
				GUIStyle potentialText = GUI.skin.label;
				potentialText.alignment = TextAnchor.MiddleCenter;
				potentialText.fontSize = 20;
				potentialText.normal.textColor = Color.white;
				Rect lennardJonesRect = new Rect(atomTouchRect.x, atomTouchRect.y + atomTouchRect.height, atomTouchRect.width, atomTouchRect.height * .75f);
				GUI.DrawTexture(lennardJonesRect, darkBackground);
				if(GUI.Button(lennardJonesRect, "Lennard-Jones", buttonStyle)){
					potentialsActive = false;
					Potential.currentPotential = Potential.potentialType.LennardJones;
					myEnvironment.preCompute();
					myEnvironment.InitAtoms();
					slowMotionFrames = StaticVariables.slowMotionFrames;
				}
				Rect buckinghamRect = new Rect(lennardJonesRect.x, lennardJonesRect.y + lennardJonesRect.height, lennardJonesRect.width, lennardJonesRect.height);
				GUI.DrawTexture(buckinghamRect, lightBackground);
				if(GUI.Button(buckinghamRect, "Buckingham", buttonStyle)){
					potentialsActive = false;
					Potential.currentPotential = Potential.potentialType.Buckingham;
					myEnvironment.preCompute();
					myEnvironment.InitAtoms();
					slowMotionFrames = StaticVariables.slowMotionFrames;
				}
				Rect brennerRect = new Rect(buckinghamRect.x, buckinghamRect.y + buckinghamRect.height, buckinghamRect.width, buckinghamRect.height);
				GUI.DrawTexture(brennerRect, darkBackground);
				if(GUI.Button(brennerRect, "Brenner", buttonStyle)){
					potentialsActive = false;
					Potential.currentPotential = Potential.potentialType.Brenner;
					myEnvironment.preCompute();
					myEnvironment.InitAtoms();
					slowMotionFrames = StaticVariables.slowMotionFrames;
				}
			}
			
		}
		
		GUIStyle currStyle = GUI.skin.label;
		currStyle.alignment = TextAnchor.MiddleCenter;
		currStyle.fontSize = 25;
		currStyle.normal.textColor = Color.white;
		
		Rect arrowBackgroundRectToolbar = new Rect (Screen.width * .14f + 5.0f, 0.0f, Screen.width * .28f, Screen.height * .13f * .3f);
		Texture toolbarArrow = toolbarActive ? upArrow : downArrow;
		GUI.DrawTexture (arrowBackgroundRectToolbar, darkBackground);
		GUI.DrawTexture (new Rect (arrowBackgroundRectToolbar.x + (arrowBackgroundRectToolbar.width*.5f), 0.0f, 20.0f, arrowBackgroundRectToolbar.height), toolbarArrow); 
		if (GUI.Button (arrowBackgroundRectToolbar, "", buttonStyle)) {
			toolbarActive = !toolbarActive;
		}
		
		Rect panelArrowRect = new Rect (Screen.width * .5f, Screen.height - (Screen.height * .13f * .3f), 20.0f, Screen.height * .13f * .3f);
		Rect panelRect = new Rect (0.0f, Screen.height - (Screen.height * .27f), Screen.width, (Screen.height * .27f));
		Rect openPanelRect = new Rect(0.0f, panelRect.y, (Screen.width * .6f) + 10.0f, panelRect.height);
		Rect bottomRect = new Rect(panelRect.x + openPanelRect.width, panelArrowRect.y, Screen.width - openPanelRect.width, panelArrowRect.height);
		
		//specify the graph's coordinates
		Graph graph = Camera.main.GetComponent<Graph>();
		graph.xCoord = bottomRect.x;
		graph.yCoord = Screen.height - bottomRect.y;
		graph.width = bottomRect.width;
		graph.height = openPanelRect.height - bottomRect.height;
		if (firstPass) {
			graph.RecomputeMaxDataPoints();
			firstPass = false;
		}
		
		//create the data panel
		if (dataPanelActive) {
			GUI.DrawTexture(openPanelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, downArrow);
			GUI.DrawTexture(bottomRect, lightBackground);
			if(GUI.Button(bottomRect, "", buttonStyle)){
				dataPanelActive = !dataPanelActive;
			}
			
			bool doubleTapped = false;
			for(int i = 0; i < Atom.AllAtoms.Count; i++){
				Atom currAtom = Atom.AllAtoms[i];
				if(currAtom.doubleTapped){
					doubleTapped = true;
				}
			}
			
			if(!doubleTapped){
				DisplaySystemProperties(openPanelRect);
			}
		}
		else{
			panelRect = new Rect(0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height);
			openPanelRect = new Rect(0.0f, panelRect.y, (Screen.width * .6f) + 10.0f, panelRect.height);
			GUI.DrawTexture(panelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, upArrow);
		}
		if (GUI.Button (new Rect (0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height), "", buttonStyle)) {
			dataPanelActive = !dataPanelActive;
		}
		
		//create the toolbar options menu (i.e atomkick, reset camera, etc)
		GUIStyle toolBarButtonStyle = GUI.skin.label;
		toolBarButtonStyle.alignment = TextAnchor.MiddleCenter;
		toolBarButtonStyle.fontSize = 25;
		toolBarButtonStyle.normal.textColor = Color.white;
		
		Rect toolbarRect = new Rect(arrowBackgroundRectToolbar.x, arrowBackgroundRectToolbar.height, arrowBackgroundRectToolbar.width, atomTouchRect.height);
		if (toolbarActive) {
			GUI.DrawTexture(toolbarRect, lightBackground);
			
			//create reset button
			Texture reset = resetPressed ? resetButtonDown : resetButtonUp;
			if(GUI.Button(new Rect(toolbarRect.x, toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), reset, buttonStyle)){
				resetPressed = true;
				resetTime = Time.realtimeSinceStartup;
				myEnvironment.InitAtoms();
				slowMotionFrames = StaticVariables.slowMotionFrames;
			}
			if(Time.realtimeSinceStartup - resetTime > .05f){
				resetPressed = false;
			}
			
			//create camera button
			Texture camera = cameraPressed ? cameraButtonDown : cameraButtonUp;
			if(GUI.Button(new Rect(toolbarRect.x + (toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), camera, buttonStyle)){
				cameraPressed = true;
				cameraTime = Time.realtimeSinceStartup;
				Camera.main.transform.position = new Vector3(0.0f, 0.0f, -40.0f);
				Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			}
			if(Time.realtimeSinceStartup - cameraTime > .05f){
				cameraPressed = false;
			}
			
			//create bond line button
			Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
			if(GUI.Button(new Rect(toolbarRect.x + 2*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), bondLine, buttonStyle)){
				StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
			}
			
			//create atom kick button
			Texture atomKick = atomKickPressed ? atomKickDown : atomKickUp;
			if(GUI.Button(new Rect(toolbarRect.x + 3*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), atomKick, buttonStyle)){
				atomKickPressed = true;
				atomKickTime = Time.realtimeSinceStartup;
				AllAtomsKick();
			}
			if(Time.realtimeSinceStartup - atomKickTime > .05f){
				atomKickPressed = false;
			}
			
			//create the current time button
			Texture timeTexture = normalTimeButton;
			if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
				timeTexture = normalTimeButton;
				Time.timeScale = 1.0f;
				MotionBlur blur = Camera.main.GetComponent<MotionBlur>();
				blur.blurAmount = 0.0f;
			}
			else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
				timeTexture = slowTimeButton;
				Time.timeScale = .05f;
				MotionBlur blur = Camera.main.GetComponent<MotionBlur>();
				blur.blurAmount = 0.68f;
				
			}
			else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
				timeTexture = stoppedTimeButton;
			}
			
			if(GUI.Button(new Rect(toolbarRect.x + 4*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), timeTexture, buttonStyle)){
				if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
					currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
					StaticVariables.pauseTime = true;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
					currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
					StaticVariables.pauseTime = false;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
					currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
				}
			}
			
			//create the red x if the user double tapped an atom
			for (int i = 0; i < Atom.AllAtoms.Count; i++) {
				Atom currAtom = Atom.AllAtoms[i];
				if(currAtom.doubleTapped){
					if(GUI.Button(new Rect(toolbarRect.x + 5*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), redXButton, buttonStyle)){
						myEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
						currAtom.doubleTapped = false;
						Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
						currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
						currAtom.RemoveBondText();
						currAtom.ResetTransparency();
						RedXClicked();
					}
					
					DisplayAtomProperties(Atom.AllAtoms[i], openPanelRect);
					
				}
			}
		}
		
		
		
		//create the add atom toolbar
		Rect addAtomRect = new Rect (panelRect.x, panelRect.y - panelArrowRect.height, Screen.width * .2f, panelArrowRect.height);
		GUI.DrawTexture (addAtomRect, darkBackground);
		Texture addAtom = addAtomActive ? downArrow : upArrow;
		GUI.DrawTexture (new Rect (addAtomRect.width * .5f, addAtomRect.y, 20.0f, addAtomRect.height), addAtom);
		if (GUI.Button (addAtomRect, "", buttonStyle)) {
			addAtomActive = !addAtomActive;
		}
		
		Rect lightAddAtom = new Rect(addAtomRect.x, addAtomRect.y - (Screen.height*.1f), addAtomRect.width, (Screen.height*.1f));
		if (addAtomActive) {
			GUI.DrawTexture(lightAddAtom, lightBackground);
			
			//create the copper button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f, lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), copperTextureAdd, buttonStyle)){
				if(!clicked){
					clicked = true;
					startTime = Time.realtimeSinceStartup;
					first = true;
				}
				else{
					float currTime = Time.realtimeSinceStartup - startTime;
					if(currTime > holdTime){
						if(first){
							first = false;
							addGraphicCopper = true;
						}
					}
				}
			}
			//create the gold button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f+(addAtomRect.width / 4.0f), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), goldTextureAdd, buttonStyle)){
				if(!clicked){
					clicked = true;
					startTime = Time.realtimeSinceStartup;
					first = true;
				}
				else{
					float currTime = Time.realtimeSinceStartup - startTime;
					if(currTime > holdTime){
						if(first){
							first = false;
							addGraphicGold = true;
						}
					}
				}
			}
			//create the platinum button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f+(2*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), platinumTextureAdd, buttonStyle)){
				if(!clicked){
					clicked = true;
					startTime = Time.realtimeSinceStartup;
					first = true;
				}
				else{
					float currTime = Time.realtimeSinceStartup - startTime;
					if(currTime > holdTime){
						if(first){
							first = false;
							addGraphicPlatinum = true;
						}
					}
				}
			}
			
			//create the garbage button for deleting atoms
			Texture garbage = garbagePressed ? garbageTextureDown : garbageTexture;
			if(GUI.Button(new Rect(lightAddAtom.x + 5.0f+(3*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), garbage, buttonStyle)){
				for(int i = 0; i < Atom.AllAtoms.Count; i++){
					Atom currAtom = Atom.AllAtoms[i];
					if(currAtom.doubleTapped){
						myEnvironment.centerPos = Vector3.zero;
						Camera.main.transform.LookAt(Vector3.zero);
					}
					if(currAtom.selected){
						Atom.UnregisterAtom(currAtom);
						Destroy(currAtom.gameObject);
					}
					if(currAtom.selected && currAtom.doubleTapped){
						currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
						currAtom.RemoveBondText();
						currAtom.ResetTransparency();
					}
				}
				garbagePressed = true;
				garbageTime = Time.realtimeSinceStartup;
			}
			
			if(Time.realtimeSinceStartup - garbageTime > .05f){
				garbagePressed = false;
			}
		}
		
		if (addGraphicCopper) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), copperTexture);
			GUI.color = Color.white;
		}
		
		if (addGraphicGold) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), goldTexture);
			GUI.color = Color.white;
		}
		
		if (addGraphicPlatinum) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), platinumTexture);
			GUI.color = Color.white;
		}
		
		//create the temperature panel and the temperature slider
		Rect temperatureArrowBackgroundRect = new Rect(addAtomRect.x + addAtomRect.width + 10.0f, addAtomRect.y, (Screen.width - (addAtomRect.width+20)) / 2.0f, addAtomRect.height);
		GUI.DrawTexture(temperatureArrowBackgroundRect, darkBackground);
		Texture tempTexture = temperaturePanelActive ? downArrow : upArrow;
		GUI.DrawTexture(new Rect(temperatureArrowBackgroundRect.x + temperatureArrowBackgroundRect.width * .5f, temperatureArrowBackgroundRect.y, 20.0f, temperatureArrowBackgroundRect.height), tempTexture);
		if(GUI.Button(temperatureArrowBackgroundRect, "", buttonStyle)){
			temperaturePanelActive = !temperaturePanelActive;
		}
		
		if(temperaturePanelActive){
			Rect temperatureBackgroundRect = new Rect(temperatureArrowBackgroundRect.x, temperatureArrowBackgroundRect.y - (lightAddAtom.height), temperatureArrowBackgroundRect.width, lightAddAtom.height);
			GUI.DrawTexture(temperatureBackgroundRect, lightBackground);
			
			GUIStyle tempertureText = GUI.skin.label;
			tempertureText.alignment = TextAnchor.MiddleCenter;
			tempertureText.fontSize = 25;
			tempertureText.normal.textColor = Color.white;
			GUI.Label (new Rect(temperatureArrowBackgroundRect.x, temperatureBackgroundRect.y, temperatureBackgroundRect.width, temperatureBackgroundRect.height * .4f), "Temperature", tempertureText);
			
			GUIStyle tempNumberText = GUI.skin.label;
			tempNumberText.alignment = TextAnchor.MiddleLeft;
			tempNumberText.fontSize = 14;
			tempNumberText.normal.textColor = Color.white;
			
			GUI.Label (new Rect (temperatureBackgroundRect.x + temperatureBackgroundRect.width - 120.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)), 200.0f, 20), StaticVariables.desiredTemperature + "K" + " (" + (Math.Round(StaticVariables.desiredTemperature - 273.15, 2)).ToString() + "C)", tempNumberText);
			float newTemp = GUI.HorizontalSlider (new Rect (temperatureBackgroundRect.x + 25.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)), temperatureBackgroundRect.width - 150.0f, 20.0f), StaticVariables.desiredTemperature, StaticVariables.tempRangeLow, StaticVariables.tempRangeHigh);
			if (newTemp != StaticVariables.desiredTemperature) {
				changingSlider = true;
				StaticVariables.desiredTemperature = newTemp;
			}
			else{
				//the gui temperature has been set, we can safely change the desired temperature
				int temp = (int)StaticVariables.desiredTemperature;
				int remainder = temp % 20;
				temp -= remainder;
				StaticVariables.desiredTemperature = temp;
			}
		}
		
		//create the volume panel and the volume slider
		Rect volumeArrowBackgroundRect = new Rect (temperatureArrowBackgroundRect.x + temperatureArrowBackgroundRect.width + 10.0f, addAtomRect.y, temperatureArrowBackgroundRect.width, temperatureArrowBackgroundRect.height);
		GUI.DrawTexture (volumeArrowBackgroundRect, darkBackground);
		Texture volumeArrow = volumePanelActive ? downArrow : upArrow;
		GUI.DrawTexture (new Rect (volumeArrowBackgroundRect.x + volumeArrowBackgroundRect.width * .5f, volumeArrowBackgroundRect.y, 20.0f, volumeArrowBackgroundRect.height), volumeArrow);
		if (GUI.Button (volumeArrowBackgroundRect, "", buttonStyle)) {
			volumePanelActive = !volumePanelActive;
		}
		
		if (volumePanelActive) {
			Rect volumeBackgroundRect = new Rect(volumeArrowBackgroundRect.x, volumeArrowBackgroundRect.y - (lightAddAtom.height), volumeArrowBackgroundRect.width, lightAddAtom.height);
			GUI.DrawTexture(volumeBackgroundRect, lightBackground);
			
			GUIStyle volumeText = GUI.skin.label;
			volumeText.alignment = TextAnchor.MiddleCenter;
			volumeText.fontSize = 25;
			volumeText.normal.textColor = Color.white;
			GUI.Label (new Rect(volumeBackgroundRect.x, volumeBackgroundRect.y, volumeBackgroundRect.width, volumeBackgroundRect.height * .4f), "Volume", volumeText);
			
			GUIStyle volNumberText = GUI.skin.label;
			volNumberText.alignment = TextAnchor.UpperLeft;
			volNumberText.fontSize = 14;
			volNumberText.normal.textColor = Color.white;
			GUI.Label (new Rect (volumeBackgroundRect.x + volumeBackgroundRect.width - 120.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)) - 5.0f, 200.0f, 80.0f), guiVolume + " Angstroms\n cubed", volNumberText);
			float newVolume = GUI.HorizontalSlider (new Rect (volumeBackgroundRect.x + 25.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)), volumeBackgroundRect.width - 150.0f, 20.0f), guiVolume, 1000.0f, 64000.0f);
			
			if (newVolume != guiVolume) {
				guiVolume = newVolume;
				changingSlider = true;
				slowMotionFrames = StaticVariables.slowMotionFrames;
			}
			else{
				int volume = (int)guiVolume;
				int remainder10 = Math.Abs(1000 - volume);
				int remainder15 = Math.Abs(3375 - volume);
				int remainder20 = Math.Abs(8000 - volume);
				int remainder25 = Math.Abs(15625 - volume);
				int remainder30 = Math.Abs(27000 - volume);
				int remainder35 = Math.Abs(42875 - volume);
				int remainder40 = Math.Abs(64000 - volume);
				if(remainder10 < remainder15 && remainder10 < remainder20 && remainder10 < remainder25 && remainder10 < remainder30 && remainder10 < remainder35 && remainder10 < remainder40){
					myEnvironment.volume = 1000;
					guiVolume = 1000;
				}
				else if(remainder15 < remainder10 && remainder15 < remainder20 && remainder15 < remainder25 && remainder15 < remainder30 && remainder15 < remainder35 && remainder15 < remainder40){
					myEnvironment.volume = 3375;
					guiVolume = 3375;
				}
				else if(remainder20 < remainder15 && remainder20 < remainder10 && remainder20 < remainder25 && remainder20 < remainder30 && remainder20 < remainder35 && remainder20 < remainder40){
					myEnvironment.volume = 8000;
					guiVolume = 8000;
				}
				else if(remainder25 < remainder10 && remainder25 < remainder15 && remainder25 < remainder20 && remainder25 < remainder30 && remainder25 < remainder35 && remainder25 < remainder40){
					myEnvironment.volume = 15625;
					guiVolume = 15625;
				}
				else if(remainder30 < remainder15 && remainder30 < remainder20 && remainder30 < remainder25 && remainder30 < remainder10 && remainder30 < remainder35 && remainder30 < remainder40){
					myEnvironment.volume = 27000;
					guiVolume = 27000;
				}
				else if(remainder35 < remainder10 && remainder35 < remainder15 && remainder35 < remainder20 && remainder35 < remainder25 && remainder35 < remainder30 && remainder35 < remainder40){
					myEnvironment.volume = 42875;
					guiVolume = 42875;
				}
				else if(remainder40 < remainder15 && remainder40 < remainder20 && remainder40 < remainder25 && remainder40 < remainder30 && remainder40 < remainder35 && remainder40 < remainder10){
					myEnvironment.volume = 64000;
					guiVolume = 64000;
				}
			}
			
			
		}
		//CheckAtomVolumePositions();
		
		if (Input.GetMouseButtonUp (0)) {
			
			//possibly adjust the z value here depending on the position of the camera
			
			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
			{
				myEnvironment.createAtom(copperPrefab);
			}
			
			
			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
			{
				myEnvironment.createAtom(goldPrefab);
			}
			
			
			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
			{
				myEnvironment.createAtom(platinumPrefab);
			}
			
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			changingSlider = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}
		
		//create the atom selected graphic if the user has select 1 or more atoms
		int selectedAtoms = CountSelectedAtoms ();
		if (selectedAtoms > 0) {
			GUIStyle atomsSelectedText = GUI.skin.label;
			atomsSelectedText.alignment = TextAnchor.MiddleCenter;
			atomsSelectedText.fontSize = 22;
			atomsSelectedText.normal.textColor = Color.white;
			Rect darkBlueRect = new Rect(toolbarRect.x + toolbarRect.width + 5.0f, 0.0f, 200.0f, 65.0f);
			GUI.DrawTexture(darkBlueRect, darkBlueBackground);
			String atomSelectedString = selectedAtoms == 1 ? " Atom Selected" : " Atoms Selected";
			GUI.Label(darkBlueRect, selectedAtoms.ToString() + atomSelectedString);
			
			Rect lightBlueRect = new Rect(darkBlueRect.x + darkBlueRect.width, 0.0f, 15.0f, darkBlueRect.height);
			GUI.DrawTexture(lightBlueRect, lightBlueBackground);
			GUI.DrawTexture(new Rect(lightBlueRect.x, lightBlueRect.y + (lightBlueRect.height) - 15.0f, lightBlueRect.width, 15.0f), whiteCornerArrow);
			if(GUI.Button(lightBlueRect, "", buttonStyle)){
				whiteCornerActive = !whiteCornerActive;
			}
			
			if(whiteCornerActive){
				Rect selectAllRect = new Rect(darkBlueRect.x, darkBlueRect.y + darkBlueRect.height, darkBlueRect.width + lightBlueRect.width, 45.0f);
				GUI.DrawTexture(selectAllRect, darkBackground);
				GUIStyle selectAllText = GUI.skin.label;
				selectAllText.alignment = TextAnchor.MiddleCenter;
				selectAllText.fontSize = 22;
				selectAllText.normal.textColor = Color.white;
				if(selectedAtoms == Atom.AllAtoms.Count){
					GUI.Label(selectAllRect, "Deselect All", selectAllText);
					if(GUI.Button(selectAllRect, "", buttonStyle)){
						DeselectAllAtoms();
						whiteCornerActive = false;
					}
				}
				else{
					GUI.Label(selectAllRect, "Select All", selectAllText);
					if(GUI.Button(selectAllRect, "", buttonStyle)){
						SelectAllAtoms();
						whiteCornerActive = false;
					}
				}
				
			}
		}
		
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 18;
		timeText.normal.textColor = Color.white;
		GUI.Label (new Rect (Screen.width - 75.0f, 10.0f, 70.0f, 40.0f), Math.Round(StaticVariables.currentTime, 1) + "ps");
		
		/*
		if (slowMotionFrames > 0){
			StaticVariables.MDTimestep = StaticVariables.MDTimestep / 5.0f;
			StaticVariables.MDTimestepSqr = StaticVariables.MDTimestep * StaticVariables.MDTimestep;
			StaticVariables.MDTimestepInPicosecond = StaticVariables.MDTimestep / Mathf.Pow (10, -12);
			myEnvironment.preCompute();
			slowMotionFrames --;
		}else if (slowMotionFrames == 0){
			StaticVariables.MDTimestep = StaticVariables.MDTimestep * 5.0f;
			StaticVariables.MDTimestepSqr = StaticVariables.MDTimestep * StaticVariables.MDTimestep;
			StaticVariables.MDTimestepInPicosecond = StaticVariables.MDTimestep / Mathf.Pow (10, -12);
			myEnvironment.preCompute();
			slowMotionFrames = -1;
		}
		*/
		
		
	}
	
	//this function displays properties that are apart of the system as a whole,
	// such as the number of atoms
	void DisplaySystemProperties(Rect displayRect){
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 14;
		timeText.normal.textColor = Color.white;
		
		int totalAtoms = Atom.AllAtoms.Count;
		int copperAtoms = 0;
		int goldAtoms = 0;
		int platinumAtoms = 0;
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			if (currAtom.GetComponent<Copper> () != null) {
				copperAtoms++;
			}
			else if (currAtom.GetComponent<Gold> () != null) {
				goldAtoms++;
			}
			else if (currAtom.GetComponent<Platinum> () != null) {
				platinumAtoms++;
			}
		}
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 10.0f, 225, 30), "Total Atoms: " + totalAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 40.0f, 225, 30), "Copper Atoms: " + copperAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 70.0f, 225, 30), "Gold Atoms: " + goldAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 100.0f, 225, 30), "Platinum Atoms: " + platinumAtoms);
	}
	
	//this function display the properties that are specific to one specific atom such as its position, velocity, and type
	void DisplayAtomProperties(Atom currAtom, Rect displayRect){
		
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 14;
		timeText.normal.textColor = Color.white;
		
		String elementName = "";
		String elementSymbol = "";
		
		//probably a better way to do this via polymorphism
		if (currAtom.GetComponent<Copper> () != null) {
			elementName = "Copper";
			elementSymbol = "Cu";
		}
		else if (currAtom.GetComponent<Gold> () != null) {
			elementName = "Gold";
			elementSymbol = "Au";
		}
		else if (currAtom.GetComponent<Platinum> () != null) {
			elementName = "Platinum";
			elementSymbol = "Pt";
		}
		
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 20.0f, 200, 30), "Element Name: " + elementName);
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 50.0f, 200, 30), "Element Symbol: " + elementSymbol);
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 80.0f, 200, 50), "Position: " + currAtom.transform.position.ToString("E0"));
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 130.0f, 200, 50), "Velocity: " + currAtom.transform.rigidbody.velocity.ToString("E0"));
		
		DisplayBondProperties (currAtom, displayRect);
		
	}
	
	//this function displays the angles of the bonds to other atoms
	void DisplayBondProperties(Atom currAtom, Rect displayRect){
		
		List<Vector3> bonds = new List<Vector3>();
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom atomNeighbor = Atom.AllAtoms[i];
			if(atomNeighbor.gameObject == currAtom.gameObject) continue;
			if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < currAtom.BondDistance(atomNeighbor)){
				bonds.Add(atomNeighbor.transform.position);
			}
		}
		
		//need more than 1 bond to form an angle
		if (bonds.Count > 1) {
			int angleNumber = 1;
			//to display the angles, we must compute the angles between every pair of bonds
			float displayWidth = 200.0f;
			for(int i = 0; i < bonds.Count; i++){
				for(int j = i+1; j < bonds.Count; j++){
					float xCoord;
					if(angleNumber < 7){
						xCoord = displayRect.x + displayWidth;
					}
					else if (angleNumber >= 7 && angleNumber < 13){
						xCoord = displayRect.x + (2*displayWidth);
					}
					else{
						xCoord = displayRect.x + (3*displayWidth);
					}
					if(angleNumber > 18){
						break;
					}
					Vector3 vector1 = (bonds[i] - currAtom.transform.position);
					Vector3 vector2 = (bonds[j] - currAtom.transform.position);
					float angle = (float)Math.Round(Vector3.Angle(vector1, vector2), 3);
					float angleNum = (angleNumber-1) % 6;
					//GUI.Label(new Rect(Screen.width - 285, 230 + (bonds.Count * 30) + ((angleNumber-1)*30), 225, 30), "Angle " + angleNumber + ": " + angle + " degrees");
					GUI.Label(new Rect(xCoord, displayRect.y + 10.0f + (angleNum*30.0f), displayWidth, 30), "Angle " + angleNumber + ": " + Math.Round(angle,1) + " degrees");
					angleNumber++;
				}
			}
		}
		
	}
	
	//this function selects all of the atoms in the scene
	void SelectAllAtoms(){
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.selected = true;
			currAtom.SetSelected(true);
		}
	}
	
	//this function deselects all of the atoms in the scene
	void DeselectAllAtoms(){
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.selected = false;
			currAtom.SetSelected(false);
		}
		Atom.EnableSelectAtomGroup(false);
	}



	public void DeleteSelectedAtoms(){
		Debug.Log("DeleteSelectedAtoms called");
		for(int i=Atom.AllAtoms.Count-1; i >= 0;i--){
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				//delete this atom from the list
				Debug.Log("deleting atom: " + i);
				currAtom.selected = false;
				currAtom.SetSelected(false);
				//Atom.AllAtoms.RemoveAt(i);
				Atom.UnregisterAtom(currAtom);
				//delete the object
				Destroy(currAtom.gameObject);
			}
		}
		Atom.EnableSelectAtomGroup(false);
	}
	//this function returns the number of atoms that are selected
	public int CountSelectedAtoms(){
		return NumberofAtom.selectedAtoms;
	}
	//this function checks the position of all of the atoms to make sure they are inside of the box
	void CheckAtomVolumePositions(){
		
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			Vector3 newPosition = currAtom.transform.position;
			if(currAtom.transform.position.x > CreateEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer){
				newPosition.x = CreateEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.x < CreateEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer){
				newPosition.x = CreateEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y > CreateEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer){
				newPosition.y = CreateEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y < CreateEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer){
				newPosition.y = CreateEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z > CreateEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer){
				newPosition.z = CreateEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z < CreateEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer){
				newPosition.z = CreateEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
			}
			currAtom.transform.position = newPosition;
		}
		
		
	}
	
	//this function changes the panels so they are in double clicked mode
	public void SetDoubleClicked(){
		dataPanelActive = true;
		atomTouchActive = false;
		toolbarActive = true;
		addAtomActive = false;
		temperaturePanelActive = false;
		volumePanelActive = false;
	}
	
	//this function disables the panels so it goes back to the default mode
	void RedXClicked(){
		dataPanelActive = false;
		atomTouchActive = true;
		toolbarActive = true;
		addAtomActive = true;
		temperaturePanelActive = true;
		volumePanelActive = true;
	}
	//kick all selected atoms or all if none is selected
	public void SelectedAtomsKick(){
		bool hasAtomSelected = false;
		for(int i = 0; i < Atom.AllAtoms.Count; i++){
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				hasAtomSelected = true;
				AtomKick(i);
			}
		}
		if(!hasAtomSelected)AllAtomsKick();
	}
	public void AllAtomsKick(){
		for(int i = 0; i < Atom.AllAtoms.Count; i++){
			Atom currAtom = Atom.AllAtoms[i];
			AtomKick(i);
		}
	}

	//kick only one atom
	public void AtomKick(int i){
		//for(int i = 0; i < Atom.AllAtoms.Count; i++){
			Atom currAtom = Atom.AllAtoms[i];
			float xVelocity = 0.0f;
			float yVelocity = 0.0f;
			float zVelocity = 0.0f;
			//this is maximum random velocity and needs to be determined emperically.
			//float maxVelocity = 0.05f / StaticVariables.MDTimestep; 
			float maxVelocity = 2.0f*Mathf.Sqrt(3.0f*StaticVariables.kB*StaticVariables.desiredTemperature/currAtom.massamu/StaticVariables.amuToKg)/StaticVariables.angstromsToMeters; //this is maximum random velocity and needs to be determined emperically.

			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				xVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
			}
			else{
				xVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
			}
			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				yVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
			}
			else{
				yVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
			}
			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				zVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
			}
			else{
				zVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
			}
			currAtom.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
			//currAtom.accelerationOld = Vector3.zero;
			//currAtom.accelerationNew = Vector3.zero;
		//}
	}
	
	



	public void AddPlatinumAtom(){
		
		if(Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(platinumPrefab, curPosition, curRotation);
			myEnvironment.createAtom(platinumPrefab);
			
		}
	
	}

	public void AddGoldAtom(){
		
		if(Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(goldPrefab, curPosition, curRotation);
			myEnvironment.createAtom(goldPrefab);
			
		}

	}

	public void AddCopperAtom(){
		
		if(Input.mousePosition.x < Screen.width 
			&& Input.mousePosition.x > 0 && Input.mousePosition.y > 0 
			&& Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(copperPrefab, curPosition, curRotation);
			myEnvironment.createAtom(copperPrefab);
			
		}

	
	

	}

	public void ResetAll(){
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
		myEnvironment.InitAtoms ();
		slowMotionFrames = StaticVariables.slowMotionFrames;
		Atom.EnableSelectAtomGroup(false);
	}
	//for the left panel
	public void createBondline(){
		RawImage ri = bondLineBtn.GetComponent<RawImage>();
		Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
		ri.texture = bondLine;
		StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
	}

	//for volume slider
	//range: 1.5 - 4.5 nm 
	//step size: 0.5
	public void SnapVolumeToInterval(float stepSize){
		float rawVal = volSliderComponent.value;
		float floor = Mathf.Floor(rawVal / stepSize);
		if(!Mathf.Approximately(rawVal / stepSize, floor))
			volSliderComponent.value = floor * stepSize + stepSize;

	}

	public void changeTimer(){
		RawImage ri = timer.GetComponent<RawImage>();
		if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
			currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
			StaticVariables.pauseTime = true;
			ri.texture = stoppedTimeButton;

		}
		else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
			currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
			Time.timeScale = .05f;
			StaticVariables.pauseTime = false;
			ri.texture = slowTimeButton;
		}
		else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
			currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
			Time.timeScale = 1.0f;
			ri.texture = normalTimeButton;
		}	
	}

	public void RestoreTimer(StaticVariables.TimeSpeed oldTimeSpeed){
		RawImage ri = timer.GetComponent<RawImage>();
		currentTimeSpeed = oldTimeSpeed;
		if(oldTimeSpeed == StaticVariables.TimeSpeed.Normal){
			Time.timeScale = 1.0f;
			ri.texture = normalTimeButton;
		}
		else if(oldTimeSpeed == StaticVariables.TimeSpeed.Stopped){
			StaticVariables.pauseTime = true;
			ri.texture = stoppedTimeButton;
		}
		else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
			Time.timeScale = 0.05f;
			StaticVariables.pauseTime = false;
			ri.texture = slowTimeButton;
		}	
	}

	//toggle settings callback
	//TODO
	public void SettingsOnClick(){
		bool oldStatus = settingsCanvas.activeSelf;
		settingsCanvas.SetActive(!oldStatus);
	 	//if active, pause game, change timer state to stopped
	 	StaticVariables.TimeSpeed oldTimeSpeed = currentTimeSpeed;
	 	Debug.Log("old time speed:" +oldTimeSpeed);
	 	//if originally settings is not active
	 	if(!settingsActive)
	 	{
	 		//RawImage ri = timer.GetComponent<RawImage>();
		 	//currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
			//StaticVariables.pauseTime = true;
			//ri.texture = stoppedTimeButton;
			//turn off hud
			//hud.SetActive(false);
		}
		/*
		else
		{
			hud.SetActive(true);
			Debug.Log("restoring timer");
			RestoreTimer(oldTimeSpeed);
		}
		*/
		settingsActive = settingsCanvas.activeSelf;
	}
	public void test(){
		Debug.Log("mio");
	}
	public void resetCamera(){
		Camera.main.transform.position = new Vector3(0.0f, 0.0f, -40.0f);
		Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	}

	public void ToggleSelectAll() {
		selectedAll = !selectedAll;
		if(selectedAll)
		{
			SelectAllAtoms();
			//selectAllText.text = "Deselect All";
			//hide deselect button
			//deselectButton.SetActive(false);
		}
		else
		{
			DeselectAllAtoms();
			//selectAllText.text = "Select All";
			//deselectButton.SetActive(true);
		}

	}

	public void deselectAll(){
		DeselectAllAtoms ();
	}

	public void ChangeAtomVolume(){
		//GameObject[] volumetag = GameObject.FindGameObjectsWithTag("scrolltag1");
		//GUI.Label (new Rect (volumetag.x + volumetag.width - 120.0f, (volumetag.y + (volumetag.height/2.0f)), 200.0f, 20), TemperatureCalc.desiredTemperature + "K" + " (" + (Math.Round(TemperatureCalc.desiredTemperature - 273.15, 2)).ToString() + "C)", tempNumberText);
		
		//Slider volumeSlider = null;
		//GameObject volume = GameObject.FindWithTag ("Scrolltag2");
		
		CreateEnvironment createEnvironment = CreateEnvironment.myEnvironment;
		//these are in angstroms
		float offset = StaticVariables.maxVol + StaticVariables.minVol;
		createEnvironment.width = Math.Abs(offset -volSliderComponent.value*10.0f);
		createEnvironment.height = Math.Abs(offset -volSliderComponent.value*10.0f);
		createEnvironment.depth = Math.Abs(offset - volSliderComponent.value*10.0f);
		createEnvironment.volume = 
			createEnvironment.width*
			createEnvironment.height*
			createEnvironment.depth; //to nm^3
		//since slider is upside down...
		float realVol = createEnvironment.width * 0.1f;
		ChangePlaneMaterial(realVol);
	}

	public void ChangePlaneMaterial(float realVol){

		MeshRenderer topMesh = CreateEnvironment.topPlane.GetComponent<MeshRenderer>();
		MeshRenderer backMesh = CreateEnvironment.backPlane.GetComponent<MeshRenderer>();
		MeshRenderer frontMesh = CreateEnvironment.frontPlane.GetComponent<MeshRenderer>();
		MeshRenderer leftMesh = CreateEnvironment.leftPlane.GetComponent<MeshRenderer>();
		MeshRenderer rightMesh = CreateEnvironment.rightPlane.GetComponent<MeshRenderer>();
		MeshRenderer bottomMesh = CreateEnvironment.bottomPlane.GetComponent<MeshRenderer>();

		if(Mathf.Approximately(realVol, 1.0f)){
			topMesh.material = matPlane1;
			backMesh.material = matPlane1;
			frontMesh.material = matPlane1;
			leftMesh.material = matPlane1;
			rightMesh.material = matPlane1;
			bottomMesh.material = matPlane1;
		}
		else if(Mathf.Approximately(realVol, 1.5f)){
			topMesh.material = matPlane1_5;
			backMesh.material = matPlane1_5;
			frontMesh.material = matPlane1_5;
			leftMesh.material = matPlane1_5;
			rightMesh.material = matPlane1_5;
			bottomMesh.material = matPlane1_5;
		}
		else if(Mathf.Approximately(realVol,2.0f)){
			topMesh.material = matPlane2;
			backMesh.material = matPlane2;
			frontMesh.material = matPlane2;
			leftMesh.material = matPlane2;
			rightMesh.material = matPlane2;
			bottomMesh.material = matPlane2;
		}
		else if(Mathf.Approximately(realVol,2.5f)){
			topMesh.material = matPlane2_5;
			backMesh.material = matPlane2_5;
			frontMesh.material = matPlane2_5;
			leftMesh.material = matPlane2_5;
			rightMesh.material = matPlane2_5;
			bottomMesh.material = matPlane2_5;
		}
		else if(Mathf.Approximately(realVol,3.0f)){
			topMesh.material = matPlane3;
			backMesh.material = matPlane3;
			frontMesh.material = matPlane3;
			leftMesh.material = matPlane3;
			rightMesh.material = matPlane3;
			bottomMesh.material = matPlane3;
		}
		else if(Mathf.Approximately(realVol,3.5f)){
			topMesh.material = matPlane3_5;
			backMesh.material = matPlane3_5;
			frontMesh.material = matPlane3_5;
			leftMesh.material = matPlane3_5;
			rightMesh.material = matPlane3_5;
			bottomMesh.material = matPlane3_5;
		}
		else if(Mathf.Approximately(realVol,4.0f)){
			topMesh.material = matPlane4;
			backMesh.material = matPlane4;
			frontMesh.material = matPlane4;
			leftMesh.material = matPlane4;
			rightMesh.material = matPlane4;
			bottomMesh.material = matPlane4;
		}
	}
	//check if all of the atoms are static
	public bool CheckAllAtomsStatic(){
		for(int i=0;i<Atom.AllAtoms.Count;i++){
			Atom currentAtom = Atom.AllAtoms[i];
			Vector3 atomVel = currentAtom.rigidbody.velocity;
			float diff = Vector3.Distance(atomVel, Vector3.zero);
			if(!Mathf.Approximately(diff, 0.0f)){
				return false;
			}
		}
		return true;
	}
	public void ToggleTempSliderSelected(bool selected){
		SettingsControl.TempUpdating = selected;
	}
	public void ToggleVolSliderSelected(bool selected){
		SettingsControl.VolUpdating = selected;
	}
	public void ChangeAtomTemperature(){
		oldTemperaure = StaticVariables.desiredTemperature;
		StaticVariables.desiredTemperature = Math.Abs(5000 - tempSliderComponent.value);
		Debug.Log("temp changing");
		//turn off camera
		
		if(oldTemperaure < 0){
			return;
		}else if(Mathf.Approximately(oldTemperaure, 0.0f)){
			//if all atoms are static, kick all
			if(CheckAllAtomsStatic()){
				AllAtomsKick();
			}
		}
		
	}

	/*
	public void displayInfo(){

			
		//Boolean toDisplay;
		if (toDisplay == true) {
			GameObject atomDisplay = GameObject.FindGameObjectWithTag("atomInfo");
			toDisplay = false;			
			atomDisplay.SetActive(false);
	
						
		} 
		else {

			//str.text ="Atom info";
			GameObject atomDisplay = GameObject.FindGameObjectWithTag("atomInfo");




		
			

		}
		
	
	}
	*/

}
