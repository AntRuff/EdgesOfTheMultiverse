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
			SetupGameController("BaronBlade", "EdgesOfTheMultiverse.Eliza", "Megalopolis");

			StartGame();

			var mdp = GetCardInPlay("MobileDefensePlatform");

			QuickHandStorage(eliza.ToHero());
			DecisionSelectTarget = mdp;
			QuickHPStorage(mdp);

			UsePower(eliza.CharacterCard);

			QuickHandCheck(1);
			QuickHPCheck(-2);
		}
	}
}