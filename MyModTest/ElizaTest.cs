using NUnit.Framework;
using System;
using EdgesOfTheMultiverse;
using EdgesOfTheMultiverse.Eliza;
using Handelabra;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Collections.Generic;

namespace MyModTest
{
	[TestFixture()]

	public class ElizaTest : BaseTest
	{
		protected HeroTurnTakerController eliza { get { return FindHero("Eliza"); } }

		#region HelperFunctions
		protected string HeroNamespace = "EdgesOfTheMultiverse.Eliza";

		private void SetupIncap(TurnTakerController villain)
		{
			SetHitPoints(eliza, 1);
			DealDamage(villain, eliza, 2, DamageType.Melee);
		}

		#endregion

		//[Test()]
		/*
		 * public void Card {
		 * 
		 *	SetupGameController("BaronBlade", "EdgesOfTheMulitverse.Eliza", "Megalopolis");
		 *	StartGame();
		 *	GoToPlayCardPhase(eliza);
		 *	
		 *	Test cases
		 * 
		 */


		[Test()]
		public void TestInnatePower()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			QuickHandStorage(eliza.ToHero());
			DecisionSelectTarget = mdp;
			QuickHPStorage(mdp);

			UsePower(eliza.CharacterCard);

			QuickHandCheck(1);
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestIncap1()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);
			AssertIncapacitated(eliza);
			Card mdp = GetCardInPlay("MobileDefensePlatform");

			GoToUseIncapacitatedAbilityPhase(eliza);
			DecisionSelectCard = legacy.CharacterCard;
			UseIncapacitatedAbility(eliza, 0);

			QuickHPStorage(legacy);
			DealDamage(mdp, legacy, 2, DamageType.Melee);
			QuickHPCheck(-1);
		}

		[Test()]
		public void TestOmnifensiveFighting()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");
			//DiscardAllCards(eliza);

			var testCard = PutInHand(eliza, "OmnifensiveFighting");

			GoToPlayCardPhase(eliza);

			DecisionSelectTarget = mdp;
			QuickHPStorage(mdp);
			PlayCard(testCard);
			// 1 target 2 damage
			QuickHPCheck(-2);
			// Reduce next damage by 1
			QuickHPStorage(eliza);
			DealDamage(mdp, eliza, 3, DamageType.Melee);
			QuickHPCheck(-2);

			// Damage Should not be reduced this time
			QuickHPStorage(eliza);
			DealDamage(mdp, eliza, 3, DamageType.Melee);
			QuickHPCheck(-3);
		}
	}
}