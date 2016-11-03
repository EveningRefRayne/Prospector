using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prospector : MonoBehaviour {
	public static Prospector S;
	public Deck deck;
	public TextAsset deckXML;
	public Layout layout;
	public TextAsset layoutXML;


	void Awake()
	{
		S=this;
	}
	void Start()
	{
		deck = GetComponent<Deck>();
		deck.initDeck(deckXML.text);
		Deck.shuffle(ref deck.cards);

		layout = GetComponent<Layout>();
		layout.readLayout(layoutXML.text);
	}
}
