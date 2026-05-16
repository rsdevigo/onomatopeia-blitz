---
name: Fluxo cenas MainMenu
overview: "Substituir o arranque único em SampleScene por fluxo explícito: cena de menu, cena de jogo (e opcionalmente bootstrap), carregamento assíncrono e navegação a partir dos botões da UI."
todos:
  - id: scenes-assets
    content: Criar/organizar cenas 10_MainMenu e gameplay offline; registar em Build Settings
    status: completed
  - id: scene-flow
    content: Implementar SceneFlow (App) com LoadSceneAsync e nomes centralizados
    status: completed
  - id: wire-mainmenu
    content: MainMenuView continue → LoadOfflineGame + persistir nome/dificuldade mínimos
    status: completed
  - id: wire-results
    content: ResultsView botões → LoadLeaderboard / LoadMainMenu + passagem de score
    status: completed
  - id: wire-leaderboard-back
    content: LeaderboardView back → LoadMainMenu
    status: completed
  - id: docs-scenes
    content: Documentar fluxo e índices de build no Manual-Programador
    status: completed
isProject: false
---

# Fluxo de cenas e MainMenu — plano de implementação

## Estado atual

- [`Assets/Scenes/`](Assets/Scenes/): essencialmente [`SampleScene.unity`](Assets/Scenes/SampleScene.unity) + [`OfflineQuickStart`](Assets/Scripts/App/OfflineQuickStart.cs) inicia partida no `Start`.
- [`MainMenuView`](Assets/Scripts/UI/Views/MainMenuView.cs): botão `continue` só faz `Debug.Log`.
- [`ResultsView`](Assets/Scripts/UI/Views/ResultsView.cs): `to-leaderboard` / `to-menu` só `Debug.Log`.

## Objetivo

Fluxo **Menu → Jogo (offline)** e **Resultados → Leaderboard / Menu**, usando `SceneManager` com nomes estáveis e um pequeno orquestrador em **`Blitz.App`** para não acoplar UI Toolkit a detalhes de cena.

## Cenas (Editor + Build Settings)

1. Criar (ou duplicar a partir de SampleScene) pelo menos:
   - `10_MainMenu.unity` — só UI de menu + eventual `EventSystem`.
   - `30_Gameplay_Offline.unity` (ou renomear uso de SampleScene) — `LocalMatchSession`, mesa, `OfflineQuickStart` ou arranque explícito via orquestrador.
2. Adicionar ambas a **File → Build Settings → Scenes in Build** com índices documentados (0 = bootstrap ou menu).

Opcional: `00_Bootstrap.unity` que carrega `10_MainMenu` na primeira frame (útil para `DontDestroyOnLoad` do serviço de leaderboard).

## Orquestrador (`SceneFlow` ou `GameAppController`)

- Novo tipo em `Assets/Scripts/App/`: métodos estáticos ou singleton leve `LoadMainMenu()`, `LoadOfflineGame()`, `LoadLeaderboard()`, `LoadResults(int finalScore)` com `SceneManager.LoadSceneAsync` (modo `Single` para troca full-screen).
- Constantes de nomes de cena num único ficheiro (ex. `SceneNames.cs`) para evitar strings espalhadas.

## Ligação UI

- `MainMenuView.OnContinue`: ler `MainMenuViewModel` (nome, dificuldade), guardar em **`PlayerPrefs`** ou **`GameSessionStatic`** mínimo para o leaderboard; chamar `SceneFlow.LoadOfflineGame()`.
- `ResultsView`: passar score via `PlayerPrefs` / static no momento da transição para resultados, ou cena de resultados recebe `int` por componente na cena anterior (simples: `MatchEndScoreHolder` `DontDestroyOnLoad` destruído após Results ler).
- `LeaderboardView.OnBack` → `LoadMainMenu()`.

## Arranque do jogo offline

- Remover ou condicionar [`OfflineQuickStart`](Assets/Scripts/App/OfflineQuickStart.cs): quando a cena de jogo carrega a partir do menu, `StartMatch` deve usar seed/rules coerentes com a dificuldade escolhida (pode ficar fase 2: primeiro só carregar cena e manter defaults).

## Critérios de pronto

- Play a partir do Menu leva à cena de jogo; voltar ao menu funciona.
- Resultados navegam para leaderboard/menu sem só `Debug.Log`.
- Documentar no [Manual-Programador](Docs/Manual-Programador.md) os nomes das cenas e ordem na build.
