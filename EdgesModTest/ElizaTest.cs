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

namespace EdgesModTest
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
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Bunker", "Megalopolis");

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
		public void TestIncap2Sentinels()
		{
			SetupGameController("BaronBlade", HeroNamespace, "TheSentinels", "Megalopolis");
			StartGame();
			SetupIncap(baron);
			AssertIncapacitated(eliza);
			Card mdp = GetCardInPlay("MobileDefensePlatform");

			GoToUseIncapacitatedAbilityPhase(eliza);
			QuickHPStorage(mdp);
			DecisionSelectTurnTaker = sentinels.TurnTaker;
			DecisionSelectCard = mainstay;
			DecisionSelectTarget = mdp;
			UseIncapacitatedAbility(eliza, 1);
			QuickHPCheck(-2);

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

		[Test()]
		public void TestRapidStrikes()
		{
			//{Eliza} deals up to 2 targets 2 melee damage
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			PutIntoPlay("BladeBattalion");

			// Targets Mobile Defense Platform and Blade Battalion
			var mdp = GetCardInPlay("MobileDefensePlatform");
			var bb = GetCardInPlay("BladeBattalion");
			Card[] targets = { mdp, bb };

			//Check 2 damage to 2 targets
			DecisionSelectTargets = targets;
			QuickHPStorage(mdp, bb, baron.CharacterCard);
			var testCard = PutInHand(eliza, "RapidStrikes");
			PlayCard(testCard);
			QuickHPCheck(-2, -2, 0);
			QuickHPCheckZero();

			//Check select 1 target
			Card[] targets2 = { mdp, null };
			DecisionSelectTargets = targets2;
			testCard = PutInHand(eliza, "RapidStrikes");
			PlayCard(testCard);
			QuickHPCheck(-2, 0, 0);
			QuickHPCheckZero();

			//Check no target
			DecisionDoNotSelectCard = SelectionType.SelectTarget;
			testCard = PutInHand(eliza, "RapidStrikes");
			PlayCard(testCard);
			QuickHPCheck(0, 0, 0);

		}

		[Test()]
		public void TestSwiftIncapacitation()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			PutIntoPlay("BladeBattalion");

			var mdp = GetCardInPlay("MobileDefensePlatform");
			var bb = GetCardInPlay("BladeBattalion");
			var testCard = PutInHand(eliza, "SwiftIncapacitation");

			bb.SetHitPoints(2);
			mdp.SetHitPoints(2);

			// No targets selected
			DecisionSelectCards = new Card[] { null, null };
			PlayCard(testCard);
			AssertIsInPlay(mdp);
			AssertIsInPlay(bb);

			ResetDecisions();

			// Checks if card targeting is correct, and 2 card destroy
			AssertNextDecisionChoices(new Card[] { mdp, bb }, new Card[] { baron.CharacterCard });
			Card[] cards = { mdp, bb };
			DecisionSelectCards = cards;
			testCard = PutInHand(eliza, "SwiftIncapacitation");
			PlayCard(testCard);
			AssertInTrash(mdp);
			AssertInTrash(bb);


			ResetDecisions();

			PutIntoPlay("BladeBattalion");
			var bb2 = GetCardInPlay("BladeBattalion");
			bb2.SetHitPoints(2);

			// 1 card destroy
			Card[] cards2 = { bb2, null };
			DecisionSelectCards = cards2;
			testCard = PutInHand(eliza, "SwiftIncapacitation");
			PlayCard(testCard);
			AssertInTrash(bb);
		}

		[Test()]
		public void TestArcaneArmRetainTargetStatus()
		{
			SetupGameController("BaronBlade", HeroNamespace, "RealmOfDiscord");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");
			var rr = PutIntoPlay("RunicRapier");
			AssertIsTarget(aa, 6);
			AssertNotTarget(rr);

			SetHitPoints(aa, 4);

			var iv = PutIntoPlay("ImbuedVitality");
			AssertIsTarget(aa, 4);
			AssertIsTarget(rr, 6);

			DestroyCard(iv);
			AssertIsTarget(aa, 4);
			AssertNotTarget(rr);
		}

		//Tests damage redirection
		[Test()]
		public void TestArcaneArmNoCells()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");
			var mdp = GetCardInPlay("MobileDefensePlatform");

			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DealDamage(mdp, eliza, 3, DamageType.Melee);
			QuickHPCheck(0, -2, 0);
		}

		//Tests playing Arcane Cells
		[Test()]
		public void TestArcaneArmPlayCells()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");

			GoToPlayCardPhase(eliza);

			var dc = PutInHand(eliza, "DeflectionCell");
			PlayCard(dc);
			AssertNextToCard(dc, aa);

			var ic = PutInHand(eliza, "ImpactCell");
			PlayCard(ic);
			AssertNextToCard(ic, aa);

			var tc = PutInHand(eliza, "TelekineticCell");
			PlayCard(tc);
			AssertNextToCard(tc, aa);

			//Test playing cell when 3 are already in play.
			var tc2 = PutInHand(eliza, "TelekineticCell");
			DecisionSelectCard = tc;
			PlayCard(tc2);
			AssertNextToCard(tc2, aa);
			AssertInTrash(tc);

			//Destroy Arm to destroy cells
			ResetDecisions();
			DestroyCard("ArcaneArm");

			AssertInTrash(aa, dc, ic, tc2);
		}

		//Test Deflection Cell interactions
		[Test()]
		public void TestDeflectionCell()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");
			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			var dc = PutInHand(eliza, "DeflectionCell");
			PlayCard(dc);

			//Reduce damage
			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DealDamage(mdp, eliza, 3, DamageType.Melee);
			QuickHPCheck(0, -1, 0);

			dc = PutInHand(eliza, "DeflectionCell");
			PlayCard(dc);

			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DealDamage(mdp, eliza, 3, DamageType.Melee);
			QuickHPCheck(0, 0, 0);

			//Destroy Deflection cell and draw a card.
			QuickHandStorage(eliza.ToHero());
			DestroyCard(dc);
			QuickHandCheck(1);
		}

		//Test Impact Cell
		[Test()]
		public void TestImpactCell()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");
			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			var ic = PutInHand(eliza, "ImpactCell");
			PlayCard(ic);

			//Increase Damage
			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DealDamage(eliza, mdp, 3, DamageType.Melee);
			QuickHPCheck(-4, 0, 0);

			var ic2 = PutInHand(eliza, "ImpactCell");
			PlayCard(ic2);

			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DealDamage(eliza, mdp, 3, DamageType.Melee);
			QuickHPCheck(-5, 0, 0);

			SetHitPoints(mdp, 10);

			//Destroy cell and deal damage.
			QuickHPStorage(mdp, aa, eliza.CharacterCard);
			DecisionSelectTarget = mdp;
			DestroyCard(ic2);
			QuickHPCheck(-3, 0, 0);
		}

		//Test Single Rapier Passive and Power
		[Test()]
		public void TestSingleRunicRapier()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			var rr = PutInHand(eliza, "RunicRapier");
			var rr2 = PutInHand(eliza, "RunicRapier");

			PlayCard(rr);
			PlayCard(rr2); //Should fail to play

			QuickHPStorage(mdp);
			DecisionSelectTargets = new Card[] { mdp, mdp };
			UsePower(rr);
			QuickHPCheck(-3);
		}

		//Test playing multiple Rapiers and Telekinetic Cell interaction
		[Test()]
		public void TestMultipleRunicRapiersAndTelekineticCells()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			DiscardAllCards(eliza);

			var aa = PutIntoPlay("ArcaneArm");

			var rr = PutInHand(eliza, "RunicRapier");
			var rr2 = PutInHand(eliza, "RunicRapier");
			var rr3 = PutInHand(eliza, "RunicRapier");
			var rr4 = PutInHand(eliza, "RunicRapier");

			var tc = PutInHand(eliza, "TelekineticCell");
			var tc2 = PutInHand(eliza, "TelekineticCell");
			var tc3 = PutInHand(eliza, "TelekineticCell");

			PlayCard(rr);
			PlayCard(rr2); //Should fail to play

			PlayCard(tc);
			PlayCard(rr2); //Should now succeed

			PlayCard(tc2);
			PlayCard(tc3);
			PlayCard(rr3);
			PlayCard(rr4);

			//Deal 1 damage and 4 triggers
			QuickHPStorage(mdp);
			DecisionSelectTargets = new Card[] { mdp, mdp, mdp, mdp };
			DealDamage(eliza, mdp, 1, DamageType.Melee);
			QuickHPCheck(-5);

			ResetDecisions();

			SetHitPoints(mdp, 10);

			var bb = PutIntoPlay("BladeBattalion");
			var bb2 = PutIntoPlay("BladeBattalion");
			var bb3 = PutIntoPlay("BladeBattalion");

			//4 Rapier power
			QuickHPStorage(mdp, bb, bb2, bb3);
			DecisionSelectTargets = new Card[] { mdp, bb, bb2, bb3 };
			UsePower(rr);
			QuickHPCheck(-2, -2, -2, -2);

			ResetDecisions();

			//4 Rapier power, choose 2 targets
			QuickHPStorage(mdp, bb, bb2, bb3);
			DecisionSelectTargets = new Card[] { mdp, bb, null, null };
			UsePower(rr);
			QuickHPCheck(-2, -2, 0, 0);

			GoToStartOfTurn(eliza);

			ResetDecisions();

			//Check first time dealing damage reset
			QuickHPStorage(mdp);
			DecisionSelectTargets = new Card[] { mdp, mdp, mdp, mdp };
			DealDamage(eliza, mdp, 1, DamageType.Melee);
			QuickHPCheck(-5);

			ResetDecisions();

			//Check destroy Telekinetic Cell
			DecisionSelectCard = rr2;
			DestroyCard(tc);
			AssertInTrash(tc);
			AssertInHand(rr2);

			ResetDecisions();

			// Check move Telekinetic Cell
			DecisionSelectCard = rr3;
			PutInHand(tc2);
			AssertInHand(tc2);
			AssertInHand(rr3);

			ResetDecisions();

			//Check plays after losing Telekinetic Cells and played from deck.
			PlayCard(rr3); //Should fail to play card
			PlayTopCard(eliza);
			PutOnDeck(eliza, rr2);
			PlayTopCard(eliza); //Should fail to play card
		}

		[Test()]
		public void TestCallForArmsRapier()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			PutInTrash("RunicRapier");

			GoToPlayCardPhase(eliza);
			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Deck);
			var rapier = eliza.HeroTurnTaker.Deck.Cards.First((Card c) => c.Identifier == "RunicRapier");
			var ca = PutInHand(eliza, "CallForArms");
			var rs = PutInHand(eliza, "RapidStrikes");
			DecisionSelectCards = new Card[] { rapier, rs, null };

			QuickShuffleStorage(eliza);
			QuickHandStorage(eliza.ToHero());
			PlayCard(ca);
			QuickHandCheck(0);
			QuickShuffleCheck(1);
			AssertInHand(eliza, rapier);
			AssertInTrash(ca, rs);
		}

		[Test()]
		public void TestCallForArmsArcaneNoPlay()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutInTrash("ArcaneArm");

			GoToPlayCardPhase(eliza);
			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Trash);
			var ca = PutInHand(eliza, "CallForArms");
			DecisionSelectCard = aa;
			DecisionDoNotSelectCard = SelectionType.PlayCard;

			QuickShuffleStorage(eliza);
			QuickHandStorage(eliza.ToHero());
			PlayCard(ca);
			QuickHandCheck(1);
			QuickShuffleCheck(0);
			AssertInHand(eliza, aa);
			AssertInTrash(ca);
		}

		[Test()]
		public void TestChargeTheArcaneCellsPlayNoArm()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var cc = PutInHand("ChargeTheArcaneCells");

			GoToPlayCardPhase(eliza);
			var ic = eliza.HeroTurnTaker.Deck.Cards.First((Card c) => c.Identifier == "ImpactCell");
			DecisionMoveCardDestination = new MoveCardDestination(eliza.HeroTurnTaker.PlayArea);
			DecisionSelectCard = ic;

			QuickShuffleStorage(eliza);
			QuickHandStorage(eliza.ToHero());
			PlayCard(cc);
			QuickHandCheck(-1);
			QuickShuffleCheck(1);
			AssertInTrash(cc, ic);
		}

		[Test()]
		public void TestChargeTheArcaneCellsPlayArm()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");
			var cc = PutInHand("ChargeTheArcaneCells");

			GoToPlayCardPhase(eliza);
			var ic = eliza.HeroTurnTaker.Deck.Cards.First((Card c) => c.Identifier == "ImpactCell");
			DecisionMoveCardDestination = new MoveCardDestination(eliza.HeroTurnTaker.PlayArea);
			DecisionSelectCard = ic;

			QuickShuffleStorage(eliza);
			QuickHandStorage(eliza.ToHero());
			PlayCard(cc);
			QuickHandCheck(-1);
			QuickShuffleCheck(1);
			AssertNextToCard(ic, aa);
			AssertInTrash(cc);
		}

		[Test()]
		public void TestChargeTheArcaneCellsHand()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var cc = PutInHand("ChargeTheArcaneCells");

			GoToPlayCardPhase(eliza);
			var ic = eliza.HeroTurnTaker.Deck.Cards.First((Card c) => c.Identifier == "ImpactCell");
			DecisionMoveCardDestination = new MoveCardDestination(eliza.HeroTurnTaker.Hand);
			DecisionSelectCard = ic;

			QuickShuffleStorage(eliza);
			QuickHandStorage(eliza.ToHero());
			PlayCard(cc);
			QuickHandCheck(0);
			QuickShuffleCheck(1);
			AssertInTrash(cc);
			AssertInHand(ic);
		}

		[Test()]
		public void TestDeliberateTargeting()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var ele = PutIntoPlay("ElementalRedistributor");

			DestroyCard("MobileDefensePlatform");

			QuickHPStorage(eliza, baron);
			DealDamage(eliza, baron, 2, DamageType.Fire);
			QuickHPCheck(-2, 0);

			var dt = PutIntoPlay("DeliberateTargeting");


			QuickHPStorage(eliza, baron);
			DealDamage(eliza, baron, 2, DamageType.Fire);
			QuickHPCheck(0, -2);

			QuickHPStorage(eliza, baron);
			UsePower(dt, 0);
			QuickHPCheck(-1, -1);
			AssertInTrash(dt);
		}

		[Test()]
		public void TestCaptureNotKill()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");
			var cnk = PutIntoPlay("CaptureNotKill");

			DealDamage(eliza, mdp, 10, DamageType.Melee);
			AssertOnBottomOfDeck(mdp);

			var bb = PutIntoPlay("BladeBattalion");

			DealDamage(legacy, bb, 10, DamageType.Melee);
			AssertInTrash(bb);

			UsePower(cnk, 0);
			AssertInTrash(cnk);
		}

		[Test()]
		public void TestBladeFlurry()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			var bf = PutIntoPlay("BladeFlurry");
			var aa = PutIntoPlay("ArcaneArm");
			var ic = PutIntoPlay("ImpactCell");

			QuickHPStorage(mdp);
			DealDamage(eliza, mdp, 3, DamageType.Melee);
			QuickHPCheck(-6);

			DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard };
			var rs = PutInHand(eliza, "RapidStrikes");
			DecisionSelectCardToPlay = rs;

			QuickHPStorage(baron.CharacterCard);
			GoToEndOfTurn(eliza);
			QuickHPCheck(-4);
			AssertInTrash(mdp);
		}

		[Test()]
		public void TestBladeFlurryFireDamage()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(eliza);

			var bf = PutIntoPlay("BladeFlurry");

			QuickHPStorage(mdp);
			DealDamage(eliza, mdp, 2, DamageType.Fire);
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestBladeFlurrySwapDamageTypes()
		{
			SetupGameController("BaronBlade", HeroNamespace, "TheVisionary", "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");
			
			GoToPlayCardPhase(eliza);

			var bf = PutIntoPlay("BladeFlurry");

			DecisionSelectCard = eliza.CharacterCard;
			var tte = PutIntoPlay("TwistTheEther");

			ResetDecisions();

			DecisionSelectDamageType = DamageType.Fire;
			DealDamage(eliza, mdp, 2, DamageType.Cold);
		}

		[Test()]
		public void TestBladeFlurryHastyAugmentation()
		{
			SetupGameController("BaronBlade", HeroNamespace, "Unity", "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			GoToPlayCardPhase(unity);

			var bf = PutIntoPlay("BladeFlurry");

			DecisionSelectCard = eliza.CharacterCard;
			DecisionSelectPower = eliza.CharacterCard;
			DecisionSelectTarget = mdp;

			QuickHPStorage(mdp);
			PutIntoPlay("HastyAugmentation");
			QuickHPCheck(-6);
		}
	}
}