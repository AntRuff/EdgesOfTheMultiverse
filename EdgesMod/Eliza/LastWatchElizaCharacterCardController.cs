using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class LastWatchElizaCharacterCardController : HeroCharacterCardController
	{
		public LastWatchElizaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{ }
			
		public override IEnumerator UsePower(int index = 0)
		{
			// Destroy an Runic Rapier.

			List<Card> list = FindCardsWhere((Card c) => c.Identifier == "RunicRapier" && c.IsInPlay).ToList();
			IEnumerator e;
			if (list.Count > 0)
			{
				//If you do, destroy a ongoing or non-character device
				List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
				IEnumerator e2 = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.DestroyCard, list, storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e2);
				} else
				{
					base.GameController.ExhaustCoroutine(e2);
				}

				SelectCardDecision selectCardDecision = storedResults.FirstOrDefault();
				if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
				{
					IEnumerator e3 = base.GameController.DestroyCard(base.DecisionMaker, selectCardDecision.SelectedCard, optional: false, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e3);
					} else
					{
						base.GameController.ExhaustCoroutine(e3);
					}
					IEnumerator e4 = base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) || (c.IsDevice && !c.IsCharacter)), optional: false, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e4);
					} else
					{
						base.GameController.ExhaustCoroutine(e4);
					}
					IEnumerator e5 = base.GameController.DrawCard(this.HeroTurnTaker, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(e5);
					} else
					{
						base.GameController.ExhaustCoroutine(e5);
					}
				}
			} else
			{
				//Otherwise, search your deck for a Runic Rapier and put it into play.
				e = SearchForCards(this.DecisionMaker, true, false, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "RunicRapier"), true, false, false);
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
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			IEnumerator e = null;

			switch (index)
			{
				case 0:
					{
						//"Destroy an Ongoing or Enviroment card."
						e = base.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c) || c.IsEnvironment, "ongoing or enviroment"), false, cardSource: base.GetCardSource());
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
				case 1:
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
			}
			yield break;
		}
	
		public override void AddSideTriggers()
		{
			if (base.Card.IsFlipped)
			{
				AddSideTrigger(AddTrigger((DealDamageAction da) => da.Target.IsHeroCharacterCard && da.TargetHitPointsAfterBeingDealtDamage <= 0, PrevantDamageResponse, new TriggerType[3]
				{
					TriggerType.WouldBeDealtDamage,
					TriggerType.CancelAction,
					TriggerType.RemoveFromGame
				}, TriggerTiming.Before));
			}
		}

		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			RemoveSideTriggers();
			AddSideTriggers();
			yield return null;
		}


		private IEnumerator PrevantDamageResponse(DealDamageAction da)
		{

			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			IEnumerator e0 = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.PreventDamage, da.Target, da, storedResults, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e0);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e0);
			}

			if (DidPlayerAnswerYes(storedResults))
			{

				IEnumerator e = CancelAction(da, showOutput: false, cancelFutureRelatedDecisions: false);
				IEnumerator e2 = base.GameController.SendMessageAction(base.Card.Title + " prevants " + da.DamageSource.TitleOrName + " from dealing damage!", Priority.Low, GetCardSource(), null, showCardSource: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e);
					yield return base.GameController.StartCoroutine(e2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e);
					base.GameController.ExhaustCoroutine(e2);
				}

				e = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
				e2 = base.GameController.SendMessageAction(base.Card.Title + "is removed from the game.", Priority.Medium, GetCardSource(), null, showCardSource: true);

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e);
					yield return base.GameController.StartCoroutine(e2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e);
					base.GameController.ExhaustCoroutine(e2);
				}
			}
		}
	}
}