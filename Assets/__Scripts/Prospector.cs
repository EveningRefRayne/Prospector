using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

	CardProspector findCardByLayoutID(int layoutID)
	{
		foreach(CardProspector tCP in tableau)
		{
			if (tCP.layoutID == layoutID)
			{
				return(tCP);
			}
		}
		return null;
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

		foreach(CardProspector tCP in tableau)
		{
			foreach(int hid in tCP.slotDef.hiddenBy)
			{
				cp = findCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
			}
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
				bool validMatch = true;
				if (!cd.faceUp)
				{
					validMatch = false;
				}
				if (!adjacentRank(cd, target))
				{
					validMatch = false;
				}
				if (!validMatch) return;
				tableau.Remove(cd);
				moveToTarget(cd);
				setTableauFaces();
				break;
		}
		checkForGameOver();
	}


	void setTableauFaces()
	{
		foreach(CardProspector cd in tableau)
		{
			bool fup = true;
			foreach(CardProspector cover in cd.hiddenBy)
			{
				if (cover.state == CardState.tableau)
				{
					fup = false;
				}
			}
			cd.faceUp = fup;
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

	public bool adjacentRank(CardProspector c0, CardProspector c1)
	{
		if (!c0.faceUp || !c1.faceUp) return(false);
		if (Mathf.Abs(c0.rank - c1.rank) == 1)
		{
			return true;
		}
		if (Mathf.Max(c0.rank, c1.rank) - Mathf.Min(c0.rank,c1.rank) == 12)
		{
			return true;
		}
		return false;
	}

	void checkForGameOver()
	{
		if (tableau.Count == 0)
		{
			gameOver(true);
			return;
		}
		if (drawPile.Count >0)
		{
			return;
		}
		foreach(CardProspector cd in tableau)
		{
			if (adjacentRank(cd,target))
			{
				return;
			}
		}
		gameOver(false);
	}

	void gameOver(bool won)
	{
		if (won)
		{
			print("Game over. You win! :)");
		}
		else
		{
			print("Game over. You lost. :(");
		}
		Invoke("reloadScene", 2f);
	}

	void reloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}
