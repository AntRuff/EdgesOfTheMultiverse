using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class BladeFlurryCardController : CardController
	{
		public BladeFlurryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) 
		{ 
		}


		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => IsFirstOrOnlyCopyOfThisCardInPlay() && dd.OriginalAmount >= 2 && dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard)
			&& dd.CardSource.Card.Identifier != base.Card.Identifier, MultistrikeResponse, new TriggerType[2]
			{
				TriggerType.CancelAction,
				TriggerType.DealDamage
			}, TriggerTiming.Before);
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => SelectAndPlayCardFromHand(DecisionMaker), TriggerType.PlayCard);
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.GameController.DestroyCard(DecisionMaker, base.Card, false, cardSource: GetCardSource()), TriggerType.DestroySelf);
		}

		private IEnumerator MultistrikeResponse(DealDamageAction dd)
		{			
			IEnumerator e = CancelAction(dd, showOutput: false, cancelFutureRelatedDecisions: false);
			IEnumerator e2 = base.GameController.SendMessageAction(base.Card.Title + " causes " + dd.DamageSource.TitleOrName + " to deal damage multiple times!", Priority.Low, GetCardSource(), null, showCardSource: true);

			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
				yield return base.GameController.StartCoroutine(e2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
				base.GameController.ExhaustCoroutine(e2);
			}

			if (IsRealAction(dd))
			{
				CardSource associatedCardSource = dd.CardSource;
				AddAssociatedCardSource(associatedCardSource);
				Power power = dd.CardSource.PowerSource;
				AddPowerInUse(power);
				if (dd.StoredResults == null)
				{
					dd.StoredResults = new List<DealDamageAction>();
				}
				List<DealDamageAction> hits = new List<DealDamageAction>();

				for (int i = 0; i < dd.OriginalAmount; i++)
				{
					hits.Add(new DealDamageAction(associatedCardSource, dd.DamageSource, null, 1, dd.DamageType));
				}

				IEnumerator e3 = DealMultipleInstancesOfDamage(hits, (Card c) => c == dd.Target);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e3);
				}
				RemovePowerInUse(power);
				RemoveAssociatedCardSource(associatedCardSource);
			}

			yield return null;
		}
	}
}