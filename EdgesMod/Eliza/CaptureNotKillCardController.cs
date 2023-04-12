using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class CaptureNotKillCardController : CardController
	{
		public CaptureNotKillCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController) 
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DestroyCardAction dc) => !base.GameController.IsCardIndestructible(dc.CardToDestroy.Card) && !dc.CardToDestroy.Card.IsCharacter && dc.CardToDestroy.Card.IsTarget, MoveInsteadResponse, 
				TriggerType.MoveCard, TriggerTiming.Before);
		}

		private IEnumerator MoveInsteadResponse(DestroyCardAction dc)
		{
			IEnumerator e = CancelAction(dc);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}

			e = base.GameController.MoveCard(base.TurnTakerController, dc.CardToDestroy.Card, dc.CardToDestroy.Card.Owner.Deck, toBottom: true, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}
		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator e = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, false);
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