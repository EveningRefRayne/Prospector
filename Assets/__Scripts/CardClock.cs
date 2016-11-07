using UnityEngine;
using System.Collections;


public enum CardState2
{
	clock,
	drawpile,
	kings
}
		
public class CardClock : Card {
	public CardState2 state = CardState2.drawpile;
	public int layoutID;
	public SlotDef2 slotDef;
	override public void OnMouseUpAsButton()
	{
		ClockPatience.S.cardClicked(this);
		base.OnMouseUpAsButton ();
	}
}