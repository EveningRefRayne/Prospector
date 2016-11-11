using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Bartok : MonoBehaviour {
	public static Bartok S;

	public TextAsset deckXML;
	public TextAsset layoutXML;
	public Vector3 layoutCenter = Vector3.zero;

	public bool __;

	public Deck deck;
	public List<CardBartok> drawPile;
	public List<CardBartok> discardPile;


	void Awake()
	{
		S = this;
	}

	void Start()
	{
		deck = GetComponent<Deck> ();
		deck.initDeck (deckXML.text);
		Deck.shuffle (ref deck.cards);
	}

}
