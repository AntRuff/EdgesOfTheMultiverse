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

		//Adds damage trigger for the first time damage is dealt
		public override void AddTriggers()
		{
			base.AddTriggers();
			AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && !IsPropertyTrue("FirstTimeDealtDamage"), DealMoreDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FirstTimeDealtDamage"), TriggerType.Hidden);
		}

		//When {Eliza} deals damage for the first time on a turn, she deals 1 target 1 damage
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

		//There can only be 1 in play, unless there are Telekinetic Cells in play."
		public override bool CanBePlayedFromLocation()
		{
			int rapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "RunicRapier").Count();
			int maxRapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "TelekineticCell").Count();

			if (rapiers <= maxRapiers) { return true; }
			return false;
		}
		//{Eliza} deals X targets 2 damage where X is the number of Runic Rapiers in play.
		public override IEnumerator UsePower(int index = 0)
		{
			int damage = GetPowerNumeral(0, 2);

			DamageSource ds = new DamageSource(GameController, base.CharacterCard);
			List<DealDamageAction> list = new List<DealDamageAction>();
			//Counts number of Runic Rapiers in play
			IEnumerable<Card> rapiers = FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "RunicRapier");

			IEnumerator e = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard),
				damage, DamageType.Melee, rapiers.Count(), true, 1, cardSource: base.GetCardSource());
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