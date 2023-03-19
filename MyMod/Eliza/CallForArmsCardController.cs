using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class CallForArmsCardController : CardController
	{
		public CallForArmsCardController(Card card, TurnTakerController turnTakerController): base(card, turnTakerController) 
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator e = SearchForCards(base.HeroTurnTakerController, searchDeck: true, searchTrash: true, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "ArcaneArm" || c.Identifier == "RunicRapier"), putIntoPlay: false, putInHand: true, putOnDeck: false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}

			IEnumerator e2 = DrawCard(base.HeroTurnTakerController.HeroTurnTaker, optional: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e2);
			}
			IEnumerator e3 = SelectAndPlayCardFromHand(base.HeroTurnTakerController);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e3);
			}
		}
	}
}