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
			//Search for Arcane Cell in deck and put into hand
			IEnumerator e = SearchForCards(base.HeroTurnTakerController, searchDeck: true, searchTrash: false, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("arcane cell")), putIntoPlay: false, putInHand: true, putOnDeck: false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}

			//You may return an Arcane Cell from play to hand.
			List <SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator move = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardToHand, new LinqCardCriteria((Card c) => c.DoKeywordsContain("arcane cell") && c.IsInPlay, "arcane cell in play"), storedResults, true, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(move);
			}
			else
			{
				base.GameController.ExhaustCoroutine(move);
			}


			SelectCardDecision selectCardDecision = storedResults.Where((SelectCardDecision d) => d.Completed).FirstOrDefault();
			if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
			{
				Card card = selectCardDecision.SelectedCard;
				IEnumerator e2 = base.GameController.MoveCard(base.HeroTurnTakerController, card, new Location(card, LocationName.Hand), cardSource: base.GetCardSource());

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e2);
				}

				IEnumerator e3 = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, new LinqCardCriteria((Card c) => c.IsInHand && c.DoKeywordsContain("arcane cell")), cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e3);
				}
			}
			else
			{
				IEnumerator draw = base.GameController.DrawCard(base.HeroTurnTaker, true, cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(draw);
				}
				else
				{
					base.GameController.ExhaustCoroutine(draw);
				}
			}

		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			return base.UseIncapacitatedAbility(index);
		}
	}
}