# Chat Sessions

Cursor agent conversation exports for project continuity.

| Date | Session ID | File | Notes |
|------|------------|------|-------|
| 2026-06-02 | `fd14f60e-fefd-4cee-8e96-92d8336f5251` | [2026-06-02-passercard-session.jsonl](./2026-06-02-passercard-session.jsonl) | **Latest** — includes UI/art work + git push + sync guide |
| 2026-06-01 | `fd14f60e-fefd-4cee-8e96-92d8336f5251` | [2026-06-01-passercard-session.jsonl](./2026-06-01-passercard-session.jsonl) | Earlier snapshot (same thread) |

## Format

- `.jsonl` — one JSON object per line (full Cursor agent transcript).
- Import back into Cursor is not supported; use for review, search, and handoff on another machine.

## Continue on another PC

1. `git pull` this repo
2. Open project in Cursor
3. New Agent chat → ask it to read `2026-06-02-passercard-session.jsonl` + `docs/GDD.md` + `README.md`

## Topics

- PasserCard Unity scaffold → playable encounter UI
- Balatro temp art (`8BitDeck`, `CardFace`, layered cards, selection border)
- UI layout / art alignment fixes
- ECC workflow, git push, cross-device chat sync
