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

        // Lightly modified copy of CardController.AdjustTargetnessResponse, which is unfortunately private.
        protected IEnumerator AdjustTargetnessResponseNotPrivate(GameAction a, Card card, int maxHP)
        {
            if (a is RemoveTargetAction rta)
            {
                IEnumerator coroutine = CancelAction(rta);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                rta.CardToRemoveTarget.SetMaximumHP(maxHP, alsoSetHP: false);
            }
            else if (a is BulkRemoveTargetsAction brta)
            {
                brta.RemoveCardsFromBulkProcess(c => c == card).ForEach(delegate (Card c)
                {
                    c.SetMaximumHP(maxHP, alsoSetHP: false);
                });
            }
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger(
                (RemoveTargetAction r) => r.CardToRemoveTarget == Card,
                (RemoveTargetAction a) => AdjustTargetnessResponseNotPrivate(a, Card, 6),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );

            AddTrigger(
                (BulkRemoveTargetsAction r) => r.CardsToRemoveTargets.Any((Card c) => c == Card),
                (BulkRemoveTargetsAction a) => AdjustTargetnessResponseNotPrivate(a, Card, 6),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );
        }

        //Redirect Eliza damage and decrease it by 1.
        public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c == base.Card, 1);
			AddRedirectDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard, () => base.Card);
			//AddMaintainTargetTriggers((Card c) => c.Identifier == Card.Identifier, 6, new List<string> { "equipment" });
		}
		
	}
}