using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace EdgesOfTheMultiverse.Eliza
{
	public class ArcaneArmCardController : CardController
	{
		public ArcaneArmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		//Redirect Eliza damage and decrease it by 1.
		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c == base.Card, 1);
			AddRedirectDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard, () => base.Card);
			AddMaintainTargetTriggers((Card c) => c.Identifier == Card.Identifier, 6, new List<string> { "equipment" });
		}
		
	}
}