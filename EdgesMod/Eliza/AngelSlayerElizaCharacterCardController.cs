using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace EdgesOfTheMultiverse.Eliza
{
	public class AngelSlayerElizaCharacterCardController : HeroCharacterCardController
	{
		public AngelSlayerElizaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

		public override IEnumerator UsePower(int index = 0)
		{
			// If Arcane Arm in play, take damage and heal. Otherwise draw 2 cards
			
			IEnumerable<Card> source = FindCardsWhere(new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.Identifier == "ArcaneArm"));
			if (source.Count() > 0)
			{
				DamageSource ds = new DamageSource(this.GameController, source.ElementAt<Card>(0));

				DealDamageAction dealDamageAction = new DealDamageAction(
					GetCardSource(),
					ds,
					this.CharacterCard,
					2,
					DamageType.Radiant
				);
				IEnumerator noRedirect = DoAction(new MakeDamageNotRedirectableAction(GetCardSource(), dealDamageAction));

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(noRedirect);
				}
				else
				{
					base.GameController.ExhaustCoroutine(noRedirect);
				}

				IEnumerator damage = DoAction(dealDamageAction);
				IEnumerator healing = base.GameController.GainHP(source.ElementAt<Card>(0), 3, cardSource: base.GetCardSource());

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(damage);
					yield return base.GameController.StartCoroutine(healing);
				} else
				{
					base.GameController.ExhaustCoroutine(damage);
					base.GameController.ExhaustCoroutine(healing);
				}

			} else
			{
				IEnumerator drawing = base.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(drawing);
				}
				else
				{
					base.GameController.ExhaustCoroutine(drawing);
				}
			}
		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					Func<Card, bool> criteria = (Card c) => c.IsTarget;
					IEnumerator e = base.GameController.GainHP(HeroTurnTakerController, criteria, 1, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e);
					}
					else
					{
						base.GameController.ExhaustCoroutine(e);
					}
					break;
				case 1:
					IEnumerator e2 = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(e2);
					}
					break;
				case 2:
					IEnumerator e3 = base.GameController.SelectHeroToSelectTargetAndDealDamage(this.HeroTurnTakerController, 2, DamageType.Radiant);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(e3);
					}
					break;
			}
		}
	}
}