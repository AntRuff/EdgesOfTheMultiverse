using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class DeflectionCellCardController: ArcaneCellCardController
	{
		public DeflectionCellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) 
		{
		}
		//Reduces damage Arcane Arm takes by 1, draw a card when it leaves play
		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c.Identifier == "ArcaneArm", 1);
			AddAfterLeavesPlayAction(WhenLeavesPlay);
		}

		public override IEnumerator WhenLeavesPlay()
		{
			IEnumerator e = base.GameController.DrawCard(base.HeroTurnTaker, true);
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