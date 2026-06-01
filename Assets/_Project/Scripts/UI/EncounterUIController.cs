using System;
using System.Collections.Generic;
using PasserCard.Cards;
using PasserCard.Encounter;
using UnityEngine;
using UnityEngine.UI;

namespace PasserCard.UI
{
    public sealed class EncounterUIController : MonoBehaviour
    {
        private EncounterSession? _session;
        private EncounterUIBuilder.BuiltUI? _ui;
        private Guid? _selectedCardId;
        private readonly List<GameObject> _handCardObjects = new();
        private string _lastMessage = string.Empty;

        public void Bind(EncounterSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            EnsureUI();
            WireButtons();
            Refresh();
        }

        private void EnsureUI()
        {
            if (_ui != null)
            {
                return;
            }

            _ui = EncounterUIBuilder.Create(transform);
            BalatroSpriteLibrary.EnsureLoaded();
        }

        private void WireButtons()
        {
            if (_ui == null)
            {
                return;
            }

            _ui.DiscardButton.onClick.AddListener(OnDiscardClicked);
            _ui.PrePinButton.onClick.AddListener(OnPrePinPhaseClicked);
            _ui.ToPassPhaseButton.onClick.AddListener(OnToPassPhaseClicked);
            _ui.PassButton.onClick.AddListener(OnPassClicked);
            _ui.ToPlayPhaseButton.onClick.AddListener(OnToPlayPhaseClicked);
            _ui.SubmitButton.onClick.AddListener(OnSubmitClicked);
            _ui.NextQuotaButton.onClick.AddListener(OnNextQuotaClicked);

            for (var i = 0; i < _ui.SlotButtons.Length; i++)
            {
                var slotIndex = i;
                _ui.SlotButtons[i].onClick.AddListener(() => OnSlotClicked(slotIndex));
            }
        }

        private void OnDiscardClicked()
        {
            if (_session == null || !_selectedCardId.HasValue)
            {
                SetMessage("请先选择一张手牌。");
                return;
            }

            if (_session.TryDiscard(_selectedCardId.Value))
            {
                _selectedCardId = null;
                SetMessage("已弃牌并补牌。");
            }
            else
            {
                SetMessage("无法弃牌（次数用尽或阶段不对）。");
            }

            Refresh();
        }

        private void OnPrePinPhaseClicked()
        {
            _session?.EnterPrePinPhase();
            SetMessage("预钉阶段：选牌后点击迷雾槽。");
            Refresh();
        }

        private void OnToPassPhaseClicked()
        {
            _session?.EnterPassPhase();
            SetMessage("Pass 阶段：选牌后点 Pass。");
            Refresh();
        }

        private void OnPassClicked()
        {
            if (_session == null || !_selectedCardId.HasValue)
            {
                SetMessage("请先选择一张手牌。");
                return;
            }

            var result = _session.TryPassCard(_selectedCardId.Value);
            SetMessage(result.Message);
            Refresh();
        }

        private void OnToPlayPhaseClicked()
        {
            _session?.EnterPlayPhase();
            SetMessage("出牌阶段：选牌后点击空槽。");
            Refresh();
        }

        private void OnSubmitClicked()
        {
            if (_session == null)
            {
                return;
            }

            if (_session.TryResolveCurrentQuota(out var result, out var error))
            {
                SetMessage($"牌型 {result.Category} · 得分 {result.TotalScore} · 最高 {_session.BestScore}");
            }
            else
            {
                SetMessage(error);
            }

            Refresh();
        }

        private void OnNextQuotaClicked()
        {
            if (_session == null)
            {
                return;
            }

            if (_session.Phase == EncounterPhase.ResolveQuota)
            {
                _session.FinishQuota();
                _selectedCardId = null;
                if (_session.IsComplete)
                {
                    var outcome = _session.IsVictory ? "胜利" : "失败";
                    SetMessage($"Encounter 结束：{outcome}。魂币 {_session.Wallet.Balance}");
                }
                else
                {
                    SetMessage("进入下一出牌配额。");
                }
            }
            else
            {
                SetMessage("请先完成计分。");
            }

            Refresh();
        }

        private void OnSlotClicked(int slotIndex)
        {
            if (_session == null)
            {
                return;
            }

            var slot = _session.PlayArea.Slots[slotIndex];
            if (!slot.IsEmpty)
            {
                SetMessage("该槽已有牌。");
                return;
            }

            if (!_selectedCardId.HasValue)
            {
                SetMessage("请先选择手牌。");
                return;
            }

            var cardId = _selectedCardId.Value;
            var success = _session.Phase switch
            {
                EncounterPhase.Discard or EncounterPhase.PrePinFog => _session.TryPinToFogSlot(slotIndex, cardId),
                EncounterPhase.PlayToSlots => _session.TryPlaceCard(slotIndex, cardId),
                _ => false
            };

            if (success)
            {
                _selectedCardId = null;
                SetMessage(_session.Phase == EncounterPhase.PlayToSlots ? "已放入出牌槽。" : "已预钉到迷雾槽。");
            }
            else
            {
                SetMessage("无法放入该槽（阶段不对、非雾槽或该牌已 Pass 过）。");
            }

            Refresh();
        }

        private void OnHandCardClicked(Guid cardId)
        {
            _selectedCardId = _selectedCardId == cardId ? null : cardId;
            Refresh();
        }

        private void Refresh()
        {
            if (_session == null || _ui == null)
            {
                return;
            }

            RefreshStatus();
            RefreshSlots();
            RefreshHand();
            RefreshButtons();
            _ui.MessageText.text = _lastMessage;
        }

        private void RefreshStatus()
        {
            if (_ui == null || _session == null)
            {
                return;
            }

            var fogCount = 0;
            for (var i = 0; i < _session.PlayArea.Slots.Count; i++)
            {
                if (_session.PlayArea.Slots[i].IsFog)
                {
                    fogCount++;
                }
            }

            _ui.StatusText.text =
                $"PasserCard Encounter\n" +
                $"阶段：{_session.PhaseLabel} · 配额 {_session.CurrentQuota}/{_session.Config.PlayQuotas}\n" +
                $"魂币 {_session.Wallet.Balance} · 目标分 {_session.TargetScore} · 最高分 {_session.BestScore}\n" +
                $"Pass {_session.PassesUsed}/{_session.PassLimit} · 弃牌剩余 {_session.DiscardsRemaining} · 迷雾槽 {fogCount}/5";
        }

        private void RefreshSlots()
        {
            if (_ui == null || _session == null)
            {
                return;
            }

            for (var i = 0; i < _ui.SlotButtons.Length; i++)
            {
                var slot = _session.PlayArea.Slots[i];
                EncounterUIBuilder.StyleSlot(_ui.SlotButtons[i], slot.IsFog, slot.Card);
            }
        }

        private void RefreshHand()
        {
            if (_ui == null || _session == null)
            {
                return;
            }

            for (var i = 0; i < _handCardObjects.Count; i++)
            {
                Destroy(_handCardObjects[i]);
            }

            _handCardObjects.Clear();

            var handCount = _session.Hand.Count;
            const float cardStep = CardVisualHelper.HandCardWidth - 34f;
            var startX = -((handCount - 1) * cardStep) * 0.5f;
            for (var i = 0; i < handCount; i++)
            {
                var card = _session.Hand.Cards[i];
                var selected = _selectedCardId == card.InstanceId;
                var capturedId = card.InstanceId;
                var button = EncounterUIBuilder.CreateHandCardButton(_ui.HandContainer, card, selected,
                    () => OnHandCardClicked(capturedId));

                var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                var fanT = handCount <= 1 ? 0.5f : i / (handCount - 1f);
                var x = startX + i * cardStep;
                var arcY = -Mathf.Abs(fanT - 0.5f) * 16f;
                var rotZ = (fanT - 0.5f) * 7f;
                if (selected)
                {
                    arcY += 18f;
                    rotZ = 0f;
                }

                rect.anchoredPosition = new Vector2(x, arcY);
                rect.localRotation = Quaternion.Euler(0f, 0f, rotZ);

                _handCardObjects.Add(button.gameObject);
            }
        }

        private void RefreshButtons()
        {
            if (_ui == null || _session == null)
            {
                return;
            }

            var phase = _session.Phase;
            var complete = _session.IsComplete;

            _ui.DiscardButton.interactable = !complete && phase is EncounterPhase.Discard or EncounterPhase.PrePinFog or EncounterPhase.Pass;
            _ui.PrePinButton.interactable = !complete && phase == EncounterPhase.Discard;
            _ui.ToPassPhaseButton.interactable = !complete && phase is EncounterPhase.Discard or EncounterPhase.PrePinFog;
            _ui.PassButton.interactable = !complete && phase == EncounterPhase.Pass;
            _ui.ToPlayPhaseButton.interactable = !complete && phase is EncounterPhase.Discard or EncounterPhase.PrePinFog or EncounterPhase.Pass;
            _ui.SubmitButton.interactable = !complete && phase == EncounterPhase.PlayToSlots;
            _ui.NextQuotaButton.interactable = !complete && phase == EncounterPhase.ResolveQuota;

            SetButtonLabel(_ui.DiscardButton, $"弃牌\n({_session.DiscardsRemaining})");
            SetButtonLabel(_ui.PassButton, $"Pass\n({_session.PassesRemaining})");
            SetButtonLabel(_ui.SubmitButton, phase == EncounterPhase.PlayToSlots ? "计分" : "计分");
            SetButtonLabel(_ui.ToPlayPhaseButton, "→ 出牌");
        }

        private static void SetButtonLabel(Button button, string label)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = label;
            }
        }

        private void SetMessage(string message)
        {
            _lastMessage = message;
            if (_ui != null)
            {
                _ui.MessageText.text = message;
            }
        }
    }
}
