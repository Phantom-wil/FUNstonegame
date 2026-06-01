using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Run;
using PasserCard.UI;
using UnityEngine;

namespace PasserCard.Core
{
    /// <summary>
    /// Scene entry: creates encounter session and optional UI.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private int startingSoulCoins = 20;
        [SerializeField] private int guardianTargetScore = 120;
        [SerializeField] private bool startEncounterOnPlay = true;
        [SerializeField] private bool useEncounterUI = true;

        private EncounterSession? _session;

        public EncounterSession? Session => _session;

        private void Start()
        {
            if (!startEncounterOnPlay)
            {
                return;
            }

            StartNewEncounter();
        }

        [ContextMenu("Start New Encounter")]
        public void StartNewEncounter()
        {
            _session = CreateSession();
            _session.StartEncounter();

            if (useEncounterUI)
            {
                var ui = GetComponent<EncounterUIController>();
                if (ui == null)
                {
                    ui = gameObject.AddComponent<EncounterUIController>();
                }

                ui.Bind(_session);
            }

            Debug.Log($"[PasserCard] Encounter started. Target={guardianTargetScore}, SoulCoins={_session.Wallet.Balance}");
        }

        private EncounterSession CreateSession()
        {
            var config = new EncounterConfig
            {
                TargetScore = guardianTargetScore,
                MistFogSlots = 2
            };

            var wallet = new SoulCoinWallet(startingSoulCoins);
            var deck = PlayingCardFactory.CreateStandardDeck(dualRank: true, random: new System.Random());
            return new EncounterSession(config, deck, wallet, new System.Random());
        }
    }
}
