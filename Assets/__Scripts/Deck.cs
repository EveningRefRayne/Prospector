using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	public bool ___blah______;
	public PT_XMLReader xmlr;


	public void initDeck(string deckXMLText)
	{
		readDeck(deckXMLText);
	}

	public void readDeck(string deckXMLText)
	{
		xmlr = new PT_XMLReader();
		xmlr.Parse(deckXMLText);

		string s = "xml[0] decorator[0] ";
		s += "type="+xmlr.xml["xml"][0]["decorator"][0].att("type");
		s += " x="+xmlr.xml["xml"][0]["decorator"][0].att("x");
		s += " y="+xmlr.xml["xml"][0]["decorator"][0].att("y");
		s += " scale="+xmlr.xml["xml"][0]["decorator"][0].att("scale");
		print(s);
	}
}
