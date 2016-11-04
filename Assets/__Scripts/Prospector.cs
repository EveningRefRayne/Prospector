﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prospector : MonoBehaviour {
	public static Prospector S;
	public Deck deck;
	public TextAsset deckXML;
	public Layout layout;
	public TextAsset layoutXML;
	public List<CardProspector> drawPile;
	public Vector3 layoutCenter;
	public float xOffset = 3f;
	public float yOffset = -2.5f;
	public Transform layoutAnchor;

	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;



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
		drawPile = convertListCardsToListCardProspectors(deck.cards);

		layoutGame();
	}

	CardProspector draw()
	{
		CardProspector cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
	}


	void layoutGame()
	{
		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		CardProspector cp;
		foreach(SlotDef tSD in layout.slotDefs)
		{
			cp = draw();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(layout.multiplier.x*tSD.x, layout.multiplier.y*tSD.y,-tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			cp.state = CardState.tableau;
			cp.setSortingLayerName(tSD.layerName);
			tableau.Add(cp);
		}
		moveToTarget(draw());
		updateDrawPile();
	}


	List<CardProspector> convertListCardsToListCardProspectors(List<Card> lCD)
	{
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach(Card tCD in lCD)
		{
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
		}
		return lCP;
	}

	public void cardClicked(CardProspector cd)
	{
		switch(cd.state)
		{
			case CardState.target:
				break;
			case CardState.drawpile:
				moveToDiscard(target);
				moveToTarget(draw());
				updateDrawPile();
				break;
			case CardState.tableau:
				//Does the logic to see if the move is valid. Guess I'm writing it later?
				break;
		}
	}

	void moveToDiscard(CardProspector cd)
	{
		cd.state = CardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x*layout.discardPile.x,layout.multiplier.y*layout.discardPile.y,-layout.discardPile.layerID+0.5f);
		cd.faceUp = true;
		cd.setSortingLayerName(layout.discardPile.layerName);
		cd.setSortOrder(-100+discardPile.Count);
	}
	void moveToTarget(CardProspector cd)
	{
		if (target !=null)
			moveToDiscard(target);
		target = cd;
		cd.state = CardState.target;
		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x*layout.discardPile.x, layout.multiplier.y*layout.discardPile.y,-layout.discardPile.layerID);
		cd.faceUp = true;
		cd.setSortingLayerName(layout.discardPile.layerName);
		cd.setSortOrder(0);
	}

	void updateDrawPile()
	{
		CardProspector cd;
		for(int i=0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;
			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(layout.multiplier.x*(layout.drawPile.x+i*dpStagger.x),layout.multiplier.y*(layout.drawPile.y+i*dpStagger.y),-layout.drawPile.layerID+0.1f*i);
			cd.faceUp = false;
			cd.state = CardState.drawpile;
			cd.setSortingLayerName(layout.drawPile.layerName);
			cd.setSortOrder(-1*i);
		}
	}

}
