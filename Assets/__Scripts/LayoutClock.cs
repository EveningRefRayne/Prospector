using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class SlotDef2
{
	public float x;
	public float y;
	public float rot;
	public bool faceUp = false;
	public string layerName = "Default";
	public int layerID = 0;
	public int id;
	public string type = "slot";
	public Vector2 stagger;

}


public class LayoutClock : MonoBehaviour {
	public PT_XMLReader xmlr;
	public PT_XMLHashtable xml;
	public Vector2 multiplier;
	public List<SlotDef2> slotDefs;
	public SlotDef2 drawPile;
	public SlotDef2 kings;
	public string[] sortingLayerNames; 

	void Awake()
	{
		sortingLayerNames = new string[] {"Clock","Draw","Kings"};
	}

	public void readLayout (string xmlText)
	{
		xmlr = new PT_XMLReader();
		xmlr.Parse(xmlText);
		xml = xmlr.xml["xml"][0];
		multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
		multiplier.y = float.Parse(xml["multiplier"][0].att("y"));
		SlotDef2 tSD;
		PT_XMLHashList slotsX = xml["slot"];
		for (int i=0; i < slotsX.Count; i++)
		{
			tSD = new SlotDef2();
			if (slotsX[i].HasAtt("type"))
			{
				tSD.type = slotsX[i].att("type");
			}
			else
			{
				tSD.type = "slot";
			}
			tSD.x = float.Parse(slotsX[i].att("x"));
			tSD.y = float.Parse(slotsX[i].att("y"));
			tSD.rot = float.Parse (slotsX [i].att ("rotate"));
			tSD.layerID = int.Parse(slotsX[i].att("layer"));
			tSD.layerName = sortingLayerNames[tSD.layerID-1];
			switch (tSD.type)
			{
				case "slot":
					tSD.faceUp = (slotsX[i].att("faceup") == "1");
					tSD.id = int.Parse(slotsX[i].att("id"));
					slotDefs.Add(tSD);
					break;
				case "drawpile":
					drawPile = tSD;
					break;
				case "kings":
					kings = tSD;
					tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
					break;
			}
		}
	}


}
