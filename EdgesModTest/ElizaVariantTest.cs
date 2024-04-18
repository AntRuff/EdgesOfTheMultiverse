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

	public class ElizaVariantTest : BaseTest
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

		[Test()]
		public void TestLastWatchLoads()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");
			Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

			Assert.IsNotNull(eliza);
			Assert.IsInstanceOf(typeof(LastWatchElizaCharacterCardController), eliza.CharacterCardController);

			Assert.AreEqual(23, eliza.CharacterCard.HitPoints);
		}

		[Test()]
		public void TestLastWatchPowerDestroyOnlyCopyDevice()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");
			
			StartGame();

			var rr = PutIntoPlay("RunicRapier");
			var mdp = GetCardInPlay("MobileDefensePlatform");

			DecisionDestroyCard = mdp;
			QuickHandStorage(eliza.ToHero());
			UsePower(eliza.CharacterCard);
			QuickHandCheck(1);
			AssertInTrash(eliza, rr);
			AssertInTrash(baron, mdp);
		}

		[Test()]
		public void TestLastWatchPowerDestroyMulticopyOngoing()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");

			StartGame();

			PutIntoPlay("ArcaneArm");
			PutIntoPlay("TelekineticCell");

			var rr = PutIntoPlay("RunicRapier");
			var rr2 = PutIntoPlay("RunicRapier");
			var lff = PutIntoPlay("LivingForceField");

			DecisionSelectCards = new List<Card> { rr, lff };
			QuickHandStorage(eliza.ToHero());
			UsePower(eliza.CharacterCard);
			QuickHandCheck(1);
			AssertInTrash(eliza, rr);
			AssertInTrash(baron, lff);
			AssertIsInPlay(rr2);
		}

		[Test()]
		public void TestLastWatchPowerNoRapiersSearch()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");

			StartGame();

			var rr = PutOnDeck("RunicRapier");

			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Deck);
			DecisionSelectCard = rr;
			UsePower(eliza.CharacterCard);
			AssertIsInPlay(rr);
		}

		[Test()]
		public void TestLastWatchPowerNoRapiersSearchTrash()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");

			StartGame();

			var rr = PutInTrash("RunicRapier");

			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Trash);
			DecisionSelectCard = rr;
			UsePower(eliza.CharacterCard);
			AssertIsInPlay(rr);
		}

		[Test()]
		public void TestLastWatchPowerNoRapiersSearchTrashAndDeck()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Megalopolis");

			StartGame();

			var rr = PutOnDeck("RunicRapier");

			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Deck);
			DecisionSelectCard = rr;
			UsePower(eliza.CharacterCard);
			AssertIsInPlay(rr);

			DestroyCard(rr);
			ResetDecisions();

			DecisionSelectLocation = new LocationChoice(eliza.HeroTurnTaker.Trash);
			DecisionSelectCard = rr;
			UsePower(eliza.CharacterCard);
			AssertIsInPlay(rr);
		}

		[Test()]
		public void TestLastWatchIncap1()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);

			AssertIncapacitated(eliza);

			var lff = PutIntoPlay("LivingForceField");
			var tp = PutIntoPlay("TrafficPileup");

			AssertNextDecisionChoices(new Card[] { lff, tp });
			DecisionSelectCard = lff;
			UseIncapacitatedAbility(eliza, 0);
			AssertInTrash(lff);
			AssertIsInPlay(tp);
		}

		[Test()]
		public void TestLastWatchIncap2()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);

			AssertIncapacitated(eliza);

			var ip = PutInHand(legacy, "InspiringPresence");

			DecisionSelectTurnTaker = legacy.TurnTaker;
			DecisionSelectCardToPlay = ip;

			UseIncapacitatedAbility(eliza, 1);
			AssertIsInPlay(ip);
		}

		[Test()]
		public void TestLastWatchResponseYes()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/LastWatchElizaCharacter", "Legacy", "Megalopolis");

			StartGame();
			SetupIncap(baron);

			AssertIncapacitated(eliza);

			SetHitPoints(legacy.CharacterCard, 2);

			DecisionYesNo = true;
			QuickHPStorage(legacy.CharacterCard);

			PutOnDeck("LivingForceField");
			PlayCard(baron, "HastenDoom");

			QuickHPCheck(0);
			AssertIsInPlay(legacy.CharacterCard);
			AssertOutOfGame(eliza.CharacterCard);

			PutOnDeck("LivingForceField");
			PlayCard(baron, "HastenDoom");
		}

		[Test()]
		public void TestAngelSlayerLoads()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Megalopolis");
			Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

			Assert.IsNotNull(eliza);
			Assert.IsInstanceOf(typeof(AngelSlayerElizaCharacterCardController), eliza.CharacterCardController);

			Assert.AreEqual(20, eliza.CharacterCard.HitPoints);
		}

		[Test()]
		public void TestAngelSlayerPowerDamageAndHeal()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Megalopolis");

			StartGame();

			var aa = PutIntoPlay("ArcaneArm");

			DealDamage(baron, aa, 4, DamageType.Melee);
			QuickHPStorage(eliza.CharacterCard, aa);

			UsePower(eliza);

			QuickHPCheck(-2, 3);
		}

		[Test()]
		public void TestAngelSlayerPowerDraw()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Megalopolis");

			StartGame();

			QuickHandStorage(eliza);
			UsePower(eliza);
			QuickHandCheck(2);
		}

		[Test()]
		public void TestAngelSlayerIncap1()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Legacy", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			var mdp = GetCardInPlay("MobileDefensePlatform");
			var tp = PutIntoPlay("TrafficPileup");

			SetHitPoints(baron.CharacterCard, 20);
			SetHitPoints(legacy.CharacterCard, 20);
			SetHitPoints(mdp, 6);
			SetHitPoints(tp, 6);

			QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, mdp, tp);

			UseIncapacitatedAbility(eliza, 0);

			QuickHPCheck(1, 1, 1, 1);
		}

		[Test()]
		public void TestAngelSlayerIncap2()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Bunker", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			DecisionSelectTurnTaker = bunker.TurnTaker;

			QuickHandStorage(bunker);
			UseIncapacitatedAbility(eliza, 1);
			QuickHandCheck(1);
		}

		[Test()]
		public void TestAngelSlayerIncap3()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/AngelSlayerElizaCharacter", "Legacy", "Bunker", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			var mdp = GetCardInPlay("MobileDefensePlatform");

			DecisionSelectCard = legacy.CharacterCard;
			DecisionSelectTarget = mdp;

			QuickHPStorage(mdp);
			UseIncapacitatedAbility(eliza, 2);
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestMageKnightLoads()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Megalopolis");
			Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

			Assert.IsNotNull(eliza);
			Assert.IsInstanceOf(typeof(MageKnightElizaCharacterCardController), eliza.CharacterCardController);

			Assert.AreEqual(24, eliza.CharacterCard.HitPoints);
		}

		[Test()]
		public void TestMageKnightInnateR0P1()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Megalopolis");

			StartGame();

			var tc = PutInHand("TelekineticCell");

			DecisionSelectCardToPlay = tc;

			UsePower(eliza, 0);
			AssertInPlayArea(eliza, tc);
		}

		[Test()]
		public void TestMageKnightInnateR1P1()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Megalopolis");

			StartGame();


			DiscardAllCards(eliza);
			var tc = PutInHand("TelekineticCell");
			var ic = PutIntoPlay("ImpactCell");

			DecisionSelectCards = new Card[] { ic, tc, null };
			QuickHandStorage(eliza);

			UsePower(eliza, 0);
			AssertInPlayArea(eliza, tc);
			AssertInHand(eliza, ic);
			QuickHandCheck(0);
		}

		[Test()]
		public void TestMageKnightInnateReturnAndPlaySameCard()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Megalopolis");

			StartGame();

			var dc = PutIntoPlay("DeflectionCell");
			DiscardAllCards(eliza);

			DecisionSelectCards = new Card[] { dc, dc, null };
			QuickHandStorage(eliza);
			
			UsePower(eliza, 0);
			AssertInPlayArea(eliza, dc);
			QuickHandCheck(1);
		}

		[Test()]
		public void TestMageKnightInnateR3P4()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Megalopolis");

			StartGame();

			var dc = PutIntoPlay("DeflectionCell");
			var dc2 = PutIntoPlay("DeflectionCell");
			var ic = PutIntoPlay("ImpactCell");
			DiscardAllCards(eliza);
			var tc = PutInHand("TelekineticCell");

			DecisionSelectCards = new Card[] { dc, dc2, ic, dc, ic, tc, dc2 };
			QuickHandStorage(eliza);

			UsePower(eliza, 0);
			AssertInPlayArea(eliza, dc);
			AssertInPlayArea(eliza, dc2);
			AssertInPlayArea(eliza, tc);
			AssertInPlayArea(eliza, ic);

			QuickHandCheck(1);
		}

		[Test()]
		public void TestMageKnightIncap1()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Legacy", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			var ip = PutInHand(legacy, "InspiringPresence");

			DecisionSelectTurnTaker = legacy.TurnTaker;
			DecisionSelectCardToPlay = ip;

			UseIncapacitatedAbility(eliza, 0);
			AssertIsInPlay(ip);
		}

		[Test()]
		public void TestMageKnightIncap2()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Bunker", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			UseIncapacitatedAbility(eliza, 1);

			var mdp = GetCardInPlay("MobileDefensePlatform");
			DestroyCard(mdp);

			QuickHPStorage(baron);
			DealDamage(bunker, baron, 2, DamageType.Melee);
			QuickHPCheck(-3);

			GoToStartOfTurn(eliza);

			QuickHPStorage(baron);
			DealDamage(bunker, baron, 2, DamageType.Melee);
			QuickHPCheck(-2);
		}

		[Test()]
		public void TestMageKnightIncap3()
		{
			SetupGameController("BaronBlade", HeroNamespace + "/MageKnightElizaCharacter", "Bunker", "Megalopolis");

			StartGame();

			SetupIncap(baron);

			UseIncapacitatedAbility(eliza, 2);

			QuickHPStorage(bunker);
			DealDamage(baron, bunker, 2, DamageType.Melee);
			QuickHPCheck(-1);

			GoToStartOfTurn(eliza);

			QuickHPStorage(bunker);
			DealDamage(baron, bunker, 2, DamageType.Melee);
			QuickHPCheck(-2);
		}
	}
}