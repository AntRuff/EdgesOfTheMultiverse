using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class DeliberateTargetingCardController: CardController
	{
		public DeliberateTargetingCardController(Card card, TurnTakerController turnTakerController): base(card, turnTakerController) 
		{ 
		}


		public override void AddTriggers()
		{
			AddMakeDamageNotRedirectableTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard));
		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator e = DealDamage(base.CharacterCard, (Card c) => c.IsTarget, 1, DamageType.Melee);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}

			e = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, false);
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