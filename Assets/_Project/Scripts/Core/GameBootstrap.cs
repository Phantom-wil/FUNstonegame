using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Run;
using PasserCard.Table;
using PasserCard.UI;
using UnityEngine;

namespace PasserCard.Core
{
    /// <summary>
    /// Scene entry: creates encounter session and optional UI (encounter-only test mode).
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private int startingSoulCoins = 20;
        [SerializeField] private int guardianTargetScore = 120;
        [SerializeField] private bool startEncounterOnPlay = true;
        [SerializeField] private bool useEncounterUI = true;
        [SerializeField] private TableEnvironmentId tableEnvironment = TableEnvironmentId.FogMist;
        [SerializeField] private RunIdentityId identityId = RunIdentityId.Ferryman;
        [SerializeField] private StartingDeckId deckId = StartingDeckId.Standard;

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
            var identity = RunIdentityLibrary.Get(identityId);
            var deck = StartingDeckLibrary.Get(deckId).BuildDeck(new System.Random());
            var config = new EncounterConfig
            {
                TargetScore = guardianTargetScore,
                MistFogSlots = 2,
                TableEnvironment = tableEnvironment
            };
            identity.ApplyToConfig(config);

            var wallet = new SoulCoinWallet(identity.StartingCoins > 0 ? identity.StartingCoins : startingSoulCoins);
            return new EncounterSession(config, deck, wallet, new System.Random());
        }
    }
}
