using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class OmnifensiveFightingCardController : CardController
	{
		public OmnifensiveFightingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			var targets = 1;
			var damage = 2;
			var block = 1;

			//"{Eliza} deals 1 target 2 melee damage. {Eliza} reduces the next damage she takes by 1."
			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard),
				damage, DamageType.Melee, targets, false, targets, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			} else
			{
				base.GameController.ExhaustCoroutine(e);
			}

			ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(block);
			rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
			rdse.NumberOfUses = 1;
			rdse.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
			e = AddStatusEffect(rdse);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			} else
			{
				base.GameController.ExhaustCoroutine(e);
			}
			yield break;
		}
	}
}