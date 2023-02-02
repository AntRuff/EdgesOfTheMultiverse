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
			//Draw 1 card and deal 1 target 2 melee damage
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			
			QuickHandStorage(eliza.ToHero());
			DecisionSelectTarget = mdp;
			QuickHPStorage(mdp);

			UsePower(eliza.CharacterCard);
			//Draw Card
			QuickHandCheck(1);
			// 2 melee damage to mdp
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestIncap1()
		{
			//Reduce damage to 1 target by 1 until the start of your next turn
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);
			AssertIncapacitated(eliza);
			Card mdp = GetCardInPlay("MobileDefensePlatform");

			//Use incap
			GoToUseIncapacitatedAbilityPhase(eliza);
			DecisionSelectTarget = legacy.CharacterCard;
			UseIncapacitatedAbility(eliza, 0);

			//Test damage against Legacy
			QuickHPStorage(legacy);
			DealDamage(mdp, legacy, 2, DamageType.Melee);
			QuickHPCheck(-1);

			GoToDrawCardPhase(legacy);

			//Test damage later against Legacy
			QuickHPStorage(legacy);
			DealDamage(mdp, legacy, 2, DamageType.Melee);
			QuickHPCheck(-1);

			//Test damage against Legacy after effect leaves
			GoToUseIncapacitatedAbilityPhase(eliza);
			QuickHPStorage(legacy);
			DealDamage(mdp, legacy, 2, DamageType.Melee);
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestIncap2()
		{
			//One hero deals 1 target 1 melee damage and 1 melee damage
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);
			AssertIncapacitated(eliza);
			Card mdp = GetCardInPlay("MobileDefensePlatform");
			
			//Legacy deals mdp 1 damage and 1 damage, 2 damage total
			GoToUseIncapacitatedAbilityPhase(eliza);
			QuickHPStorage(mdp);
			DecisionSelectTurnTaker = legacy.TurnTaker;
			DecisionSelectTarget = mdp;
			UseIncapacitatedAbility(eliza, 1);
			QuickHPCheck(-2);

			DestroyCard(mdp);

			//Legacy deals Baron Blade 1 damage and 1 damage.
			//Nemesis applies to both hits, for a total of 4 damage
			QuickHPStorage(baron);
			DecisionSelectTurnTaker = legacy.TurnTaker;
			DecisionSelectTarget = baron.CharacterCard;
			UseIncapacitatedAbility(eliza, 1);
			QuickHPCheck(-4);
		}

		[Test()]
		public void TestIncap3()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);
			AssertIncapacitated(eliza);

			QuickHandStorage(legacy);
			DecisionSelectTurnTaker = legacy.TurnTaker;
			UseIncapacitatedAbility(eliza, 2);
			QuickHandCheck(1);
		}

		[Test()]
		public void TestOmnifensiveFighting()
		{
			//Deal 1 target 2 damage. Reduce the next damage {Eliza} takes by 1
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