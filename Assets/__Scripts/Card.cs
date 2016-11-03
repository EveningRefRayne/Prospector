using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Card : MonoBehaviour {
	//Going to define the class later, making all of its internal classes and stuff now.

}




[System.Serializable]
public class Decorator
{
	public string type;
	public Vector3 loc;
	public bool flip = false;
	public float scale = 1f;
}




[System.Serializable]
public class CardDefinition
{
	public string face;
	public int rank;
	public List<Decorator> pips = new List<Decorator>();
}