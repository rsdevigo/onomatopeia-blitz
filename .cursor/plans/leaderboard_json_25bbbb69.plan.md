---
name: Leaderboard JSON
overview: Persistir entradas de leaderboard em JSON (persistentDataPath), expor repositório por interface no Core e ligar o LeaderboardPresenter a dados reais; gravar score ao fim da partida.
todos:
  - id: lb-model
    content: Adicionar LeaderboardEntry + ILeaderboardRepository em Blitz.Core
    status: completed
  - id: lb-json
    content: Implementar JsonLeaderboardRepository em Blitz.App (load/save persistentDataPath)
    status: completed
  - id: lb-presenter
    content: Refatorar LeaderboardPresenter + LeaderboardView para dados reais
    status: completed
  - id: lb-submit
    content: Ao MatchEnd, TryAdd com nome/score (fonte do nome a definir com fluxo de cenas/perfil)
    status: completed
  - id: lb-docs
    content: Atualizar Manual-Programador e Manual-Game-Designer (persistência JSON, API Core, fluxo TryAdd)
    status: completed
isProject: false
---

# Leaderboard persistente em JSON — plano de implementação

## Estado atual (2026-05-25)

### Já feito (pré-requisitos — plano [fluxo_cenas_mainmenu](fluxo_cenas_mainmenu_b9aec075.plan.md) concluído)

| Área | Estado |
|------|--------|
| Cenas e build | `50_Leaderboard.unity` existe; `SceneNames.Leaderboard` = `"50_Leaderboard"`; cena registada no editor ([`GameplaySceneSetup`](Assets/Editor/GameplaySceneSetup.cs)). |
| Navegação | [`SceneFlow`](Assets/Scripts/Gameplay/Navigation/SceneFlow.cs): `LoadLeaderboard()`, `LoadResults(int)`, `LoadMainMenu()`. |
| UI leaderboard | [`Leaderboard.uxml`](Assets/UI Toolkit/UXML/Leaderboard/Leaderboard.uxml): `ListView` `entries`, título “top 20”, botão `back`. |
| UI resultados | [`ResultsView`](Assets/Scripts/UI/Views/ResultsView.cs): `to-leaderboard` → `LoadLeaderboard()`; `to-menu` → `LoadMainMenu()`. |
| Voltar do ranking | [`LeaderboardView`](Assets/Scripts/UI/Views/LeaderboardView.cs): `back` → `SceneFlow.LoadMainMenu()`. |
| Score até resultados | `MatchEnd` em [`OfflineMinigameOrchestrator`](Assets/Scripts/Gameplay/Minigames/OfflineMinigameOrchestrator.cs) chama `SceneFlow.LoadResults(_session.Score)`; score em `PlayerPrefs` via [`GameSessionPrefs.PendingResultsScore`](Assets/Scripts/Core/GameSessionPrefs.cs). |
| Nome / dificuldade / minigame | Menu grava em `PlayerPrefs` ([`MainMenuView`](Assets/Scripts/UI/Views/MainMenuView.cs)); gameplay lê dificuldade/minigame no orquestrador. |

### Ainda por implementar (este plano)

| Área | Estado |
|------|--------|
| Modelo Core | Não existem `LeaderboardEntry` nem `ILeaderboardRepository` em [`Blitz.Core`](Assets/Scripts/Core/). |
| Persistência JSON | Não existe `JsonLeaderboardRepository`; [`Blitz.App`](Assets/Scripts/App/) só contém [`OfflineQuickStart.cs`](Assets/Scripts/App/OfflineQuickStart.cs). |
| Lista na UI | [`LeaderboardPresenter`](Assets/Scripts/UI/Presenters/LeaderboardPresenter.cs): 10 linhas fixas `DemoPlayer` (UXML promete top **20**). |
| Injeção na view | [`LeaderboardView`](Assets/Scripts/UI/Views/LeaderboardView.cs): instancia presenter sem repositório. |
| Gravação no ranking | Nenhum `TryAdd` em `MatchEnd`, resultados ou menu; score **não** entra no ficheiro JSON. |

### Score atual (contexto)

- O valor passado a `LoadResults` é `_session.Score` ([`LocalMatchSession`](Assets/Scripts/Gameplay/Match/LocalMatchSession.cs) / fase `MatchEnd`) — hoje interpretado na UI de resultados como contagem de cartas corretas ([`ResultsPresenter`](Assets/Scripts/UI/Presenters/ResultsPresenter.cs)), não a fórmula composta do blueprint de arquitetura (+100/penalidades/bónus tempo × multiplicador).

---

## Restrição de asmdefs (importante para implementação)

```
Blitz.App  →  Blitz.UI, Blitz.Gameplay, Blitz.Core
Blitz.UI   →  Blitz.Gameplay, Blitz.Core   (sem referência a App)
Blitz.Gameplay → Blitz.Core               (sem referência a App)
```

- `JsonLeaderboardRepository` deve ficar em **`Blitz.App`** (usa `Application.persistentDataPath`).
- **`LeaderboardView` / `LeaderboardPresenter` (UI)** e **`OfflineMinigameOrchestrator` (Gameplay)** não podem referenciar tipos em App.
- Padrão recomendado: em **Core**, `ILeaderboardRepository` + registo estático leve (ex. `LeaderboardServices.Register(ILeaderboardRepository)`) ou interface `ILeaderboardRepositoryProvider` num `MonoBehaviour` em **Gameplay** com implementação registada no arranque por um **`LeaderboardBootstrap`** em App (`DontDestroyOnLoad` na cena de menu ou bootstrap futuro).
- Alternativa mais simples (menos ideal): submeter entrada em **App** num componente na cena `30_Gameplay_Core` que escuta o mesmo evento de fim de partida — evita acoplar Gameplay ao repositório, mas duplica o hook de `MatchEnd`.

---

## Modelo e API (Core)

- Em [`Blitz.Core`](Assets/Scripts/Core/): `LeaderboardEntry` (readonly struct ou record) alinhado ao [blueprint de arquitetura](blitz_onomatopoeico_architecture_d4df416d.plan.md): **nome**, **score**, **minigame id**, **difficulty id**, data (UTC ISO ou `DateTime`).
- `ILeaderboardRepository`: `IReadOnlyList<LeaderboardEntry> LoadTop(int max)`; `bool TryAdd(LeaderboardEntry entry)` — insere, ordena desc por score, trunca a top N (ex. **20**, coerente com UXML), persiste.

---

## Implementação (App)

- `JsonLeaderboardRepository : ILeaderboardRepository`: `Path.Combine(Application.persistentDataPath, "leaderboard.json")`.
- Serialização: `UnityEngine.JsonUtility` ou `System.Text.Json` com wrapper `{ "entries": [...] }`.
- Ficheiro em falta/corrupto: lista vazia + log opcional.
- `LeaderboardBootstrap` (MonoBehaviour): cria repositório, regista em Core, opcional `DontDestroyOnLoad` (cena `00_Bootstrap` ainda não existe — pode colocar no GO de menu ou gameplay até haver bootstrap).

---

## UI

- `LeaderboardPresenter`: construtor com `ILeaderboardRepository` (ou lista já carregada); `Bind()` chama `LoadTop(20)` e formata linhas (rank, nome, score, opcional dificuldade/minigame).
- `LeaderboardView`: resolver repositório via registo Core / provider serializado / `FindFirstObjectByType` num tipo conhecido em Gameplay — **sem** referência direta a `JsonLeaderboardRepository`.

---

## Escrita ao fim da partida

**Dados já disponíveis no fluxo offline:**

| Campo | Fonte |
|-------|--------|
| Score | `_session.Score` no `MatchEnd` ([`OfflineMinigameOrchestrator`](Assets/Scripts/Gameplay/Minigames/OfflineMinigameOrchestrator.cs) ~L145) ou `GameSessionPrefs.PendingResultsScore` após transição |
| Nome | `PlayerPrefs.GetString(GameSessionPrefs.PlayerName, …)` |
| Dificuldade | `GameSessionPrefs.SelectedDifficultyId` |
| Minigame | `GameSessionPrefs.SelectedMinigameId` |

**Onde chamar `TryAdd` (escolher um):**

1. **Preferido:** no mesmo bloco `MatchEnd` do orquestrador, via `ILeaderboardRepository` do registo Core (uma vez por partida, flag `_leaderboardSubmitted` análoga a `_matchEndHandled`).
2. **Alternativa:** ao abrir [`ResultsView`](Assets/Scripts/UI/Views/ResultsView.cs) / antes de `LoadLeaderboard` — exige registo Core acessível sem asmdef App em UI (mesmo padrão de registo).

Evitar dupla submissão se o jogador voltar a resultados ou repetir cena.

---

## Documentação

Após implementação funcional (repositório, UI e `TryAdd`), atualizar a documentação em `docs/`:

| Documento | O que registar |
|-----------|----------------|
| [`Manual-Programador.md`](../../docs/Manual-Programador.md) | Secção **Ranking / leaderboard**: `LeaderboardEntry`, `ILeaderboardRepository`, `JsonLeaderboardRepository` (App), caminho `persistentDataPath/leaderboard.json`, `LeaderboardBootstrap` e registo em Core; restrição de asmdefs (UI/Gameplay sem referência a App); onde corre `TryAdd` (`MatchEnd` vs resultados); limite top **20**; fontes em `GameSessionPrefs` (nome, score, minigame, dificuldade). Completar o diagrama de fluxo de cenas com `Results → 50_Leaderboard → Menu` se ainda não estiver. |
| [`Manual-Game-Designer.md`](../../docs/Manual-Game-Designer.md) | Remover ou ajustar nota de “persistência placeholder” na linha de resultados/ranking; indicar que a lista reflete scores gravados localmente (offline). |
| [`Manual-ScriptableObjects.md`](../../docs/Manual-ScriptableObjects.md) | Só se o bootstrap do ranking exigir GO/componente numa cena concreta — checklist de cena `50_Leaderboard` ou menu com `LeaderboardBootstrap`. |

Critério: um programador novo consegue localizar o ficheiro JSON, estender o repositório e depurar submissão duplicada sem ler o código-fonte.

---

## Critérios de pronto

- Abrir leaderboard mostra entradas persistidas após reiniciar o editor/player.
- Nova partida com score que entra no top aparece na lista; lista respeita limite **20**.
- Navegação existente (resultados → ranking → menu) mantém-se; dados deixam de ser `DemoPlayer`.
- [`Manual-Programador.md`](../../docs/Manual-Programador.md) e [`Manual-Game-Designer.md`](../../docs/Manual-Game-Designer.md) descrevem persistência JSON e API de ranking (ver secção **Documentação**).

---

## Referências

- [fluxo_cenas_mainmenu_b9aec075.plan.md](fluxo_cenas_mainmenu_b9aec075.plan.md) — concluído; fornece score/nome entre cenas.
- [blitz_onomatopoeico_architecture_d4df416d.plan.md](blitz_onomatopoeico_architecture_d4df416d.plan.md) — § RankingSystem, `LeaderboardEntry`, regras de score sugeridas, passo 9 do roadmap.
