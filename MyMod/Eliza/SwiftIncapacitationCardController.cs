using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class SwiftIncapacitationCardController: CardController
	{
		public SwiftIncapacitationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		//Destoy up to 2 targets with 2 or fewer HP.
		public override IEnumerator Play()
		{
			IEnumerator e = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 2, "targets with 2 or fewer HP", useCardsSuffix: false), 2, false, 0, cardSource: base.GetCardSource());
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