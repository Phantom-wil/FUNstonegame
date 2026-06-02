using System;
using System.Collections.Generic;
using PasserCard.Core;
using PasserCard.Map;
using UnityEngine;
using UnityEngine.UI;

namespace PasserCard.UI
{
    public sealed class RunMapUIController : MonoBehaviour
    {
        private RunBootstrap? _bootstrap;
        private RunMapBuiltUI? _ui;
        private readonly List<Button> _nodeButtons = new();

        public void Bind(RunBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
            EnsureUI();
            Refresh();
        }

        public void Refresh()
        {
            if (_bootstrap?.Run == null || _ui == null)
            {
                return;
            }

            var run = _bootstrap.Run;
            _ui.Root.SetActive(run.Phase == Run.RunPhase.Map || run.Phase == Run.RunPhase.Complete || run.Phase == Run.RunPhase.Failed);

            _ui.StatusText.text =
                $"PasserCard Run · Act {run.Map.Act}\n" +
                $"身份：{run.Identity.DisplayName} · 套牌：{run.DeckDefinition.DisplayName}\n" +
                $"魂币 {run.Wallet.Balance} · 牌库 {run.RunDeck.Count} 张";

            _ui.MessageText.text = string.IsNullOrEmpty(run.LastEventMessage)
                ? "选择下一节点继续旅程。"
                : run.LastEventMessage;

            RebuildNodeButtons(run);
        }

        private void EnsureUI()
        {
            if (_ui != null)
            {
                return;
            }

            var canvasGo = new GameObject("RunMapCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -10;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var root = CreatePanel(canvasGo.transform, "MapRoot", new Color(0.04f, 0.05f, 0.07f, 0.94f));
            _ui = new RunMapBuiltUI
            {
                Root = root,
                StatusText = CreateText(root.transform, "MapStatus", 20, TextAnchor.UpperLeft,
                    new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(760f, -140f)),
                MessageText = CreateText(root.transform, "MapMessage", 18, TextAnchor.UpperCenter,
                    new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-420f, -160f), new Vector2(420f, -210f)),
                NodeContainer = CreatePanel(root.transform, "NodeContainer", new Color(0f, 0f, 0f, 0f),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-520f, -260f), new Vector2(520f, 260f)).transform
            };
        }

        private void RebuildNodeButtons(Run.RunState run)
        {
            for (var i = 0; i < _nodeButtons.Count; i++)
            {
                Destroy(_nodeButtons[i].gameObject);
            }

            _nodeButtons.Clear();
            if (_ui == null)
            {
                return;
            }

            var maxLayer = 0;
            for (var i = 0; i < run.Map.Nodes.Count; i++)
            {
                maxLayer = Math.Max(maxLayer, run.Map.Nodes[i].Layer);
            }

            for (var i = 0; i < run.Map.Nodes.Count; i++)
            {
                var node = run.Map.Nodes[i];
                if (node.Type == MapNodeType.Start)
                {
                    continue;
                }

                var x = -420f + node.Layer * (840f / Math.Max(1, maxLayer));
                var y = -180f + (node.Id % 3) * 120f;
                var button = CreateNodeButton(_ui.NodeContainer, node, new Vector2(x, y));
                button.interactable = node.IsAvailable;
                var nodeId = node.Id;
                button.onClick.AddListener(() => _bootstrap?.BeginEncounterForNode(nodeId));
                _nodeButtons.Add(button);
            }
        }

        private static Button CreateNodeButton(Transform parent, MapNode node, Vector2 anchoredPosition)
        {
            var go = new GameObject($"Node_{node.Id}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(150f, 56f);

            var image = go.GetComponent<Image>();
            image.color = node.IsAvailable
                ? new Color(0.18f, 0.24f, 0.28f, 0.95f)
                : node.IsVisited
                    ? new Color(0.1f, 0.12f, 0.14f, 0.7f)
                    : new Color(0.08f, 0.09f, 0.11f, 0.55f);

            var label = CreateText(go.transform, "Label", 16, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            label.text = node.DisplayLabel;
            label.color = Color.white;

            return go.GetComponent<Button>();
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color,
            Vector2 anchorMin = default, Vector2 anchorMax = default, Vector2 offsetMin = default, Vector2 offsetMax = default)
        {
            anchorMin = anchorMin == default ? Vector2.zero : anchorMin;
            anchorMax = anchorMax == default ? Vector2.one : anchorMax;
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor anchor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = new Color(0.88f, 0.9f, 0.94f, 1f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            return text;
        }

        private sealed class RunMapBuiltUI
        {
            public GameObject Root = null!;
            public Text StatusText = null!;
            public Text MessageText = null!;
            public Transform NodeContainer = null!;
        }
    }
}
