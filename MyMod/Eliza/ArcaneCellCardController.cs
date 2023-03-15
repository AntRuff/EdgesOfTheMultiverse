using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public abstract class ArcaneCellCardController : CardController
	{
		protected List<ITrigger> _triggers;
		public ArcaneCellCardController(Card card, TurnTakerController turnTakerController): base(card, turnTakerController) { }

		public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources,
			Location overridePlayArea = null, LinqTurnTakerCriteria additionalturnTakerCriteria = null)
		{
			IEnumerator e = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneArm"), storedResults, isPutIntoPlay, decisionSources);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			} 
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}
		}

		public abstract override void AddTriggers();

		public abstract IEnumerator WhenLeavesPlay();

		public override IEnumerator Play()
		{
			if (FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("arcane cell") && c != base.Card).Count() >= 3)
			{
				IEnumerator e = base.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsInPlay && c.DoKeywordsContain("arcane cell") && c != base.Card), 1, optional: false, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(e);
				}
				else
				{
					base.GameController.ExhaustCoroutine(e);
				}
			} else
			{
				yield return base.Play();
			}
			
			
		}

	}
}