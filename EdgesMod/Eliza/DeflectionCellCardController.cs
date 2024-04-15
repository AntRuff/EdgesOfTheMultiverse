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
		//Reduces damage Arcane Arm takes by 1
		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c.Identifier == "ArcaneArm", 1);
		}

		//Draw a card
		public override IEnumerator Play()
		{
			IEnumerator routine = DrawCards(this.HeroTurnTakerController, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(routine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(routine);
			}
		}
	}
}