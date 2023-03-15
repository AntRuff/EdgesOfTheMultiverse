using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class RunicRapierCardController : CardController
	{

		
		public RunicRapierCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController) 
		{
		}

		public override void AddTriggers()
		{
			base.AddTriggers();
			AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && !IsPropertyTrue("FirstTimeDealtDamage"), DealMoreDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FirstTimeDealtDamage"), TriggerType.Hidden);
		}

		private IEnumerator DealMoreDamageResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction("FirstTimeDealtDamage");
			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Melee, 1, optional: false, 1, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(e);
			}
			else
			{
				base.GameController.ExhaustCoroutine(e);
			}
		}

		public override IEnumerator Play()
		{
			return base.Play();
		}

		public override bool CanBePlayedFromLocation()
		{
			int rapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "RunicRapier").Count();
			int maxRapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "TelekineticCell").Count();

			if (rapiers <= maxRapiers) { return true; }
			return false;
		}
		public override IEnumerator UsePower(int index = 0)
		{
			int damage = GetPowerNumeral(0, 2);

			DamageSource ds = new DamageSource(GameController, base.CharacterCard);
			List<DealDamageAction> list = new List<DealDamageAction>();

			IEnumerable<Card> rapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "RunicRapier");

			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard),
				damage, DamageType.Melee, rapiers.Count(), false, rapiers.Count(), cardSource: base.GetCardSource());
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