using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ClockPatience : MonoBehaviour {

	public static ClockPatience S;
	public Deck deck;
	public TextAsset deckXML;
	public LayoutClock layout;
	public TextAsset layoutXML;
	public Vector3 layoutCenter;
	public float xOffset = 3f;
	public float yOffset = -2.5f;
	public Transform layoutAnchor;
	public List<CardClock> clock;
	public List<CardClock> drawPile;
	public List<CardClock> kings;


	void Awake()
	{
		S=this;
	}
	void 

	Start()
	{
		deck = GetComponent<Deck>();
		deck.initDeck(deckXML.text);
		Deck.shuffle(ref deck.cards);
		layout = GetComponent<LayoutClock>();
		layout.readLayout(layoutXML.text);
		drawPile = convertListCardsToListCardClock(deck.cards);
		layoutGame();
		Invoke("fixTheDamnSortingOrder",0.01f);
	}
	CardClock draw()
	{
		CardClock cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
	}
	CardClock findCardByLayoutID(int layoutID)
	{
		foreach(CardClock tCP in clock)
		{
			if (tCP.layoutID == layoutID)
			{
				return(tCP);
			}
		}
		return null;
	}

	void fixTheDamnSortingOrder()
	{
		int layer = 0;
		foreach(CardClock cp in clock)
		{
			cp.setSortOrder (layer);
			layer+=5;
		}
	}

	void layoutGame()
	{
		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		CardClock cp;
		foreach(SlotDef2 tSD in layout.slotDefs)
		{
			for (int i = 0; i < 4; i++)
			{
				cp = draw ();
				if (cp.rank == 13)
				{
					moveToKings (cp);
					i--;
				}
				else
				{
					cp.faceUp = tSD.faceUp;
					cp.transform.parent = layoutAnchor;
					cp.transform.localPosition = new Vector3 (layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID * i);
					cp.transform.rotation = Quaternion.Euler (0, 0, tSD.rot);
					cp.layoutID = tSD.id;
					cp.slotDef = tSD;
					cp.state = CardState2.clock;
					clock.Add (cp);
					cp.setSortingLayerName (tSD.layerName);
					cp.setSortOrder (i*100);
				}
			}
		}
		updateDrawPile();
	}


	List<CardClock> convertListCardsToListCardClock(List<Card> lCD)
	{
		List<CardClock> lCP = new List<CardClock>();
		CardClock tCP;
		foreach(Card tCD in lCD)
		{
			tCP = tCD as CardClock;
			lCP.Add(tCP);
		}
		return lCP;
	}

	public void cardClicked(CardClock cd)
	{
		switch(cd.state)
		{
			case CardState2.drawpile:
				//Not doing game logic now
				break;
			case CardState2.clock:
				//Not doing game logic now
				break;
		}
		//checkForGameOver(); Don't do it, because no game logic here.
	}


	void moveToKings(CardClock cd)
	{
		cd.state = CardState2.kings;
		kings.Add(cd);
		cd.transform.parent = layoutAnchor;
		cd.transform.localPosition = new Vector3(layout.multiplier.x*layout.kings.x+(layout.kings.stagger.x)*kings.Count,layout.multiplier.y*layout.kings.y,-layout.kings.layerID+0.5f);
		cd.faceUp = true;
		cd.setSortingLayerName(layout.kings.layerName);
		cd.setSortOrder(-1*kings.Count);
	}

	void updateDrawPile()
	{
		CardClock cd;
		for(int i=0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;
			cd.transform.localPosition = new Vector3(layout.multiplier.x*(layout.drawPile.x),layout.multiplier.y*(layout.drawPile.y+i),-layout.drawPile.layerID+0.1f*i);
			cd.faceUp = false;
			cd.state = CardState2.drawpile;
			cd.setSortingLayerName(layout.drawPile.layerName);
			cd.setSortOrder(-1*i);
		}
	}
		

	void checkForGameOver()
	{
		//No logic here, would need to check based on this particular game's conditions.
		gameOver(false);
	}

	void gameOver(bool won)
	{
		if (won)
		{
			//scoreManager(ScoreEvent.gameWin);
		}
		else
		{
			//scoreManager(ScoreEvent.gameLoss);
		}
		Invoke("reloadScene", 2f);
	}

	void reloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}		
}