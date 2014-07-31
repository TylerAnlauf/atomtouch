using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
	private Color currentColor;
	private Color goldColor = new Color (1.0f, .8431f, 0.0f, 1.0f);
	private float sigmaValue = 2.6367f;

	protected override float epsilon
	{
		get { return 5152.9f * 1.381f * (float)Math.Pow (10, -23); } // J
	}
	
	public override float sigma(GameObject otherAtom = null){
		if (otherAtom == null) return sigmaValue;
		Atom otherAtomScript = otherAtom.GetComponent<Atom> ();
		float otherSigma = otherAtomScript.sigma ();
		if (otherSigma == sigmaValue) return sigmaValue;
		return (float)Math.Pow(otherSigma + sigmaValue, .5f);
	}

	public override float sigma(){
		return sigmaValue;
	}
	
	protected override float massamu
	{
		get { return 196.967f; } //amu
	}

	public override Color color {
		get {
			return currentColor;
		}
	}

	protected override void SetSelected (bool selected){
		if (selected) {
			currentColor = StaticVariables.selectedColor;
		}
		else{
			currentColor = goldColor;
		}
	}

	public override void ChangeColor (Color color){
		if (color == Color.black) {
			currentColor = goldColor;
		}
		else{
			currentColor = color;
		}
	}

	void Start ()
	{
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
		//gameObject.rigidbody.velocity = new Vector3(0.0f, 5.0f, 0.0f);
	}
}

