using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class TelekineticCellCardController : ArcaneCellCardController
	{

		private const string FirstDamageToArcaneArmThisTurn = "FirstDamageToArcaneArmThisTurn";

		public TelekineticCellCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController)
		{
		}
		// Rapier Max handled in Rapier. Return Rapier to hand when it leaves play.
		public override void AddTriggers()
		{
			AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
			AddTrigger((DealDamageAction dd) => !IsPropertyTrue("FirstDamageToArcaneArmThisTurn") && dd.DamageSource.IsTarget && dd.DidDealDamage && dd.Target.Identifier == "ArcaneArm" && !HasDamageOccurredThisTurn(dd.Target, (Card ds) => ds.IsTarget && !IsHeroTarget(ds), dd), FirstDamageDealtResponse, TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FirstDamageToArcaneArmThisTurn"), TriggerType.Hidden);
			AddAfterLeavesPlayAction(WhenLeavesPlay, TriggerType.MoveCard);
		}

		private IEnumerator FirstDamageDealtResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction("FirstDamageToArcaneArmThisTurn");
			IEnumerator coroutine = DealDamage(base.Card, dd.DamageSource.Card, 1, DamageType.Melee);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator WhenLeavesPlay(GameAction ga)
		{
			IEnumerator e = base.GameController.SelectAndReturnCards(this.HeroTurnTakerController, 1,
				new LinqCardCriteria((Card c) => c.Identifier == "RunicRapier"), true, false, false, 1, cardSource: base.GetCardSource());
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