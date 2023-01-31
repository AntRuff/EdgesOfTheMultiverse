using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class DeflectionCellCardController: CardController
	{
		public DeflectionCellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) 
		{
		}

		public override IEnumerator Play()
		{
			return base.Play();
		}
	}
}