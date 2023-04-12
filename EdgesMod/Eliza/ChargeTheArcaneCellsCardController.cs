using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class ChargeTheArcaneCellsCardController : CardController
	{
		public ChargeTheArcaneCellsCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController) 
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator e = SearchForCards(base.HeroTurnTakerController, searchDeck: true, searchTrash: false, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("arcane cell")), putIntoPlay: true, putInHand: true, putOnDeck: false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}
		}
	}
}