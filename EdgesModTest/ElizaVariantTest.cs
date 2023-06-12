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

			Assert.AreEqual(22, eliza.CharacterCard.HitPoints);
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
	}
}