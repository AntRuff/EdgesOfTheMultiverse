using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class ImpactCellCardController: ArcaneCellCardController
	{
		public ImpactCellCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController) 
		{ 
		}
		//Increases Eliza damage by 1, deal 1 target 2 damage when leaves play
		public override void AddTriggers()
		{
			AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard), 1);
			AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
			AddBeforeLeavesPlayAction(WhenLeavesPlay, TriggerType.DealDamage);
		}
		
		public override IEnumerator WhenLeavesPlay(GameAction ga)
		{
			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard),
				2, DamageType.Fire, 1, false, 1, cardSource: base.GetCardSource());
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