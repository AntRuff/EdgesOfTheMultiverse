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
		public TelekineticCellCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController)
		{
		}
		// Rapier Max handled in Rapier. Return Rapier to hand when it leaves play.
		public override void AddTriggers()
		{
			AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
			AddAfterLeavesPlayAction(WhenLeavesPlay, TriggerType.MoveCard);
		}

		public override IEnumerator WhenLeavesPlay(GameAction ga)
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