using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class RapidStrikesCardController : CardController
	{
		public RapidStrikesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		//{Eliza} deals up to 2 targets 2 melee damage each.
		public override IEnumerator Play()
		{
			var damage = 2;
			var targets = 2;

			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damage, DamageType.Melee, targets, false, 0, cardSource: base.GetCardSource());
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