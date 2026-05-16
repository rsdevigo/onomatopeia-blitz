---
name: Leaderboard JSON
overview: Persistir entradas de leaderboard em JSON (persistentDataPath), expor repositório por interface no Core e ligar o LeaderboardPresenter a dados reais; gravar score ao fim da partida.
todos:
  - id: lb-model
    content: Adicionar LeaderboardEntry + ILeaderboardRepository em Blitz.Core
    status: pending
  - id: lb-json
    content: Implementar JsonLeaderboardRepository em Blitz.App (load/save persistentDataPath)
    status: pending
  - id: lb-presenter
    content: Refatorar LeaderboardPresenter + LeaderboardView para dados reais
    status: pending
  - id: lb-submit
    content: Ao MatchEnd, TryAdd com nome/score (fonte do nome a definir com fluxo de cenas/perfil)
    status: pending
isProject: false
---

# Leaderboard persistente em JSON — plano de implementação

## Estado atual

- [`LeaderboardPresenter`](Assets/Scripts/UI/Presenters/LeaderboardPresenter.cs): lista fixa “DemoPlayer”.
- [`LeaderboardView`](Assets/Scripts/UI/Views/LeaderboardView.cs): não injeta repositório.
- [`Blitz.UI`](Assets/Scripts/UI/Blitz.UI.asmdef) referencia `Blitz.Core` e `Blitz.Gameplay` — implementação de ficheiros deve viver em **`Blitz.App`** (já referencia UI) ou novo asmdef “Persistence” se quiseres isolamento; o plano assume **`Blitz.App`** para `JsonLeaderboardRepository` (usa `UnityEngine.Application`).

## Modelo e API (Core)

- Em [`Blitz.Core`](Assets/Scripts/Core/): `LeaderboardEntry` (readonly struct ou record) com campos alinhados ao blueprint do arquitetura: nome, score, dificuldade/minigame opcional, `DateTime` ou string ISO UTC.
- Interface `ILeaderboardRepository`: `IReadOnlyList<LeaderboardEntry> LoadTop(int max)`; `bool TryAdd(LeaderboardEntry entry)` (insere, ordena desc por score, trunca a top N, persiste).

## Implementação (App)

- Classe `JsonLeaderboardRepository : ILeaderboardRepository`: caminho `Path.Combine(Application.persistentDataPath, "leaderboard.json")`; serialização com `UnityEngine.JsonUtility` **ou** `System.Text.Json` (wrapper lista `{ "entries": [...] }` como no plano de arquitetura).
- Tratamento de ficheiro em falta/corrupto: começar lista vazia; log opcional.

## UI

- Alterar `LeaderboardPresenter` para aceitar `ILeaderboardRepository` (ou `IReadOnlyList<LeaderboardEntry>`) no construtor e formatar linhas a partir de entradas reais.
- `LeaderboardView`: campo serializado opcional `MonoBehaviour` que implementa `ILeaderboardRepository` **ou** factory estática no App que regista singleton — padrão simples: **`[SerializeField] ScriptableObject`** não serve para JSON mutável; usar **`[SerializeField] JsonLeaderboardRepository`** (MonoBehaviour no mesmo GO que a view) ou **`LeaderboardRepositoryHost`** no App que expõe `ILeaderboardRepository` e o View faz `GetComponent` em runtime. Preferência: **componente App** `LeaderboardServiceBehaviour` no bootstrap que `DontDestroyOnLoad` e `LeaderboardView` resolve via `FindFirstObjectByType` ou referência serializada no inspector.

## Escrita ao fim da partida

- Subscrever `IMatchSession.StateChanged` ou evento de fim de partida onde o score final está disponível (ex.: quando `Phase == MatchEnd` em [`LocalMatchSession`](Assets/Scripts/Gameplay/Match/LocalMatchSession.cs)): obter nome do jogador (de [`MainMenuViewModel`](Assets/Scripts/UI/ViewModels/) ou serviço de perfil mínimo), chamar `TryAdd`.

## Critérios de pronto

- Abrir leaderboard mostra entradas persistidas após reiniciar o editor/player.
- Nova partida com score melhor entra no top; lista respeita limite (ex. 20).
