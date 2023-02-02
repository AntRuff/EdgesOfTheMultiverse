using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class RapidStrikesCardController : CardController
	{
		public RapidStrikesCardController(Card card, TurnTakerController turnTakerController):base(card, turnTakerController)
		{
		}

		//{Eliza} deals 1 target 2 melee damage, and a second target 2 melee damage
		public override IEnumerator Play()
		{
			var damage = 2;

			//Look at Bunker's Grenade Launcher

			yield break;
		}
	}
}