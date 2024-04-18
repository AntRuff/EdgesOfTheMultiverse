using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace EdgesOfTheMultiverse.Eliza
{
	public class MageKnightElizaCharacterCardController : HeroCharacterCardController
	{
		public MageKnightElizaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

		public override IEnumerator UsePower(int index = 0)
		{
			LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.DoKeywordsContain("arcane cell"), "arcane cell");
			int maxNumberOfCards = 3;
			List<MoveCardDestination> list = new List<MoveCardDestination>();
			list.Add(new MoveCardDestination(base.HeroTurnTaker.Hand));
			List<MoveCardAction> storedMoves = new List<MoveCardAction>();
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			Location playArea = base.TurnTaker.PlayArea;
			int? minNumberOfCards = 0;
			TurnTaker turnTaker = base.TurnTaker;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectCardsFromLocationAndMoveThem(decisionMaker, playArea, minNumberOfCards, maxNumberOfCards, cardCriteria, list, isPutIntoPlay: false, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: false, null, storedMoves, autoDecideCard: false, flipFaceDown: false, showOutput: false, turnTaker, isDiscardIfMovingToTrash: false, allowAutoDecide: false, null, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			int num = storedMoves.Where((MoveCardAction mc) => mc.IsSuccessful && mc.Destination == base.HeroTurnTaker.Hand).Count()+1;
			if (num > 0)
			{
				coroutine = SelectAndPlayCardsFromHand(DecisionMaker, num, optional: false, 0, cardCriteria);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			IEnumerator e = null;

			switch (index)
			{
				//Play Card
				case 0:
					{
						e = SelectHeroToPlayCard(base.DecisionMaker);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(e);
						} else
						{
							base.GameController.ExhaustCoroutine(e);
						}
						break;
					}
				//+1 Damage
				case 1:
					{
						IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
						idse.SourceCriteria.IsHeroCharacterCard = true;
						idse.UntilStartOfNextTurn(base.TurnTaker);
						e = AddStatusEffect(idse);
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
				//-1 Damage
				case 2:
					{
						ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
						rdse.TargetCriteria.IsHeroCharacterCard = true;
						rdse.UntilStartOfNextTurn(base.TurnTaker);
						e = AddStatusEffect(rdse);
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
		}
	}
}