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
		// Eliza may deal 1 target 2 fire damage.
		public override void AddTriggers()
		{
			AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && 
				FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "ArcaneArm").Count()>=1, 1);
		}
		
		public override IEnumerator Play()
		{
			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard),
				2, DamageType.Fire, 1, false, 0, cardSource: base.GetCardSource());
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