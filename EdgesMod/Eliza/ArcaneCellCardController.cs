using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public abstract class ArcaneCellCardController : CardController
	{
		protected List<ITrigger> _triggers;
		public ArcaneCellCardController(Card card, TurnTakerController turnTakerController): base(card, turnTakerController) { }

		public abstract override void AddTriggers();

	}
}