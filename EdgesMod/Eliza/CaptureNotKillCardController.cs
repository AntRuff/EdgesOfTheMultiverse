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
			AddTrigger((DestroyCardAction dc) => dc.CardSource != null && dc.CardToDestroy.CanBeDestroyed && dc.WasCardDestroyed && dc.CardSource.Card.Owner == base.TurnTaker && dc.CardToDestroy.Card.IsTarget && dc.PostDestroyDestinationCanBeChanged && (dc.DealDamageAction == null || dc.DealDamageAction.DamageSource.Card == base.CharacterCard) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers), MoveInsteadResponse, new TriggerType[2]
				  { TriggerType.MoveCard,
					TriggerType.ChangePostDestroyDestination
				}, TriggerTiming.After);
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

			dc.SetPostDestroyDestination(dc.CardToDestroy.Card.Owner.Deck, true);
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