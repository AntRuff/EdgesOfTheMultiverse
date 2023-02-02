using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class ElizaCharacterCardController : HeroCharacterCardController
	{
		public ElizaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{ }
			
		public override IEnumerator UsePower(int index = 0)
		{
			// "Draw 1 card.

			var numberOfCards = GetPowerNumeral(0, 1);

			IEnumerator routine = DrawCards(this.HeroTurnTakerController, numberOfCards);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(routine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(routine);
			}

			//{Eliza} deals 1 target 2 melee damage.
			var targets = GetPowerNumeral(1, 1);
			var damage = GetPowerNumeral(2, 2);

			DamageSource ds = new DamageSource(this.GameController, this.Card);
			IEnumerator routine2 = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController,
				ds, damage, DamageType.Melee, targets, false, targets, cardSource: base.GetCardSource());

			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(routine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(routine2);
			}
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			IEnumerator e = null;

			switch (index)
			{
				case 0:
					{
						// Reduce damage dealt to 1 target by 1 until the start of your next turn
						List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
						e = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.SelectTarget, 
							new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget, "targets in play", false), storedResults, false, cardSource: base.GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(e);
						} else
						{
							base.GameController.ExhaustCoroutine(e);
						}

						if (DidSelectCard(storedResults))
						{
							Card selectedCard = GetSelectedCard(storedResults);
							ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
							rdse.TargetCriteria.IsSpecificCard = selectedCard;
							rdse.UntilStartOfNextTurn(base.TurnTaker);
							rdse.UntilTargetLeavesPlay(selectedCard);

							e = base.AddStatusEffect(rdse);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(e);
							}else
							{
								base.GameController.ExhaustCoroutine(e);
							}
						}
						break;
					}
				case 1:
					{
						// One Hero deals 1 target 1 melee damage and 1 melee damage
						List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
						e = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.SelectTargetFriendly, false, false, storedResults);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(e);
						} else
						{
							base.GameController.ExhaustCoroutine(e);
						}

						DamageSource ds = new DamageSource(GameController, storedResults[0].SelectedTurnTaker.CharacterCard);
						List<DealDamageAction> list = new List<DealDamageAction>
						{
							new DealDamageAction(GetCardSource(), ds, null, 1, DamageType.Melee),
							new DealDamageAction(GetCardSource(), ds, null, 1, DamageType.Melee)
						};

						e = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, 1, 1);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(e);
						}
						else
						{
							base.GameController.ExhaustCoroutine(e);
						}
						break;
					}
				case 2:
					{
						// One player may draw a card now
						e = this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(e);
						}
						else
						{
							base.GameController.ExhaustCoroutine(e);
						}
						break;
					}
			}
			yield break;
		}
	}
}