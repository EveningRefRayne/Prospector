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

	public BartokLayout layout;
	public Transform layoutAnchor;

	public float handFanDegrees = 10f;
	public List<Player> players;
	public CardBartok targetCard;

	public int numStartingCards = 7;
	public float drawTimeStagger = 0.1f;



	void Awake()
	{
		S = this;
	}

	void Start()
	{
		deck = GetComponent<Deck> ();
		deck.initDeck (deckXML.text);
		Deck.shuffle (ref deck.cards);
		layout = GetComponent<BartokLayout> ();
		layout.readLayout (layoutXML.text);
		drawPile = upgradeCardsList (deck.cards);
		layoutGame ();
	}

	public void arrangeDrawPile()
	{
		CardBartok tCB;
		for(int i=0; i<drawPile.Count;i++)
		{
			tCB = drawPile[i];
			tCB.transform.parent =layoutAnchor;
			tCB.transform.localPosition = layout.drawPile.pos;
			tCB.faceUp = false;
			tCB.setSortingLayerName(layout.drawPile.layerName);
			tCB.setSortOrder(-i*4);
			tCB.state = CBState.drawpile;
		}
	}

	void layoutGame()
	{
		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject ("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		arrangeDrawPile ();
		Player pl;
		players = new List<Player> ();
		foreach (SlotDefB tSD in layout.slotDefs)
		{
			pl = new Player ();
			pl.handSlotDef = tSD;
			players.Add (pl);
			pl.playerNum = players.Count;
		}
		players [0].type = PlayerType.human;

		CardBartok tCB;
		for (int i = 0; i < numStartingCards; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				tCB = draw ();
				tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);
				players [(j + 1) % 4].addCard (tCB);
			}
		}
		Invoke ("drawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
	}

	public CardBartok draw()
	{
		CardBartok cd = drawPile [0];
		drawPile.RemoveAt (0);
		return(cd);
	}

	public void drawFirstTarget()
	{
		CardBartok tCB = moveToTarget(draw());
	}

	public CardBartok moveToTarget(CardBartok tCB)
	{
		tCB.timeStart = 0;
		tCB.moveTo(layout.discardPile.pos+Vector3.back);
		tCB.state = CBState.toTarget;
		tCB.faceUp = true;
		tCB.setSortingLayerName ("10");
		tCB.eventualSortLayer = layout.target.layerName;
		if (targetCard != null)
		{
			moveToDiscard (targetCard);
		}
		targetCard = tCB;
		return(tCB);
	}

	public CardBartok moveToDiscard(CardBartok tCB)
	{
		tCB.state = CBState.discard;
		discardPile.Add (tCB);
		tCB.setSortingLayerName(layout.discardPile.layerName);
		tCB.setSortOrder (discardPile.Count * 4);
		tCB.transform.localPosition = layout.discardPile.pos + Vector3.back / 2;
		return tCB;
	}
		

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			players[0].addCard(draw());
		}
		if (Input.GetKeyDown (KeyCode.Alpha2))
		{
			players [1].addCard (draw ());
		}
		if (Input.GetKeyDown (KeyCode.Alpha3))
		{
			players [2].addCard (draw ());
		}
		if (Input.GetKeyDown (KeyCode.Alpha4))
		{
			players [3].addCard (draw ());
		}
	}


	List<CardBartok> upgradeCardsList(List<Card> lCD)
	{
		List<CardBartok> lCB = new List<CardBartok> ();
		foreach(Card tCD in lCD)
		{
			lCB.Add(tCD as CardBartok);
		}
		return lCB;
	}
}
