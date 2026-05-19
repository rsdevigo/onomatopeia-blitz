# Lista de assets — equipe de arte

Checklist de produção visual e áudio para **Blitz Onomatopoeico**, alinhada ao que o projeto **já consome** no código (`OnomatopoeiaDefinition`, mesa com 3 slots, UI Toolkit) e ao que está **planejado** no blueprint (`GameplayFeedbackBus`, feedback de hover, `FigureVisualId`).

**Pastas do projeto:** `Assets/Art/`, `Assets/Audio/`, `Assets/UI Toolkit/`, `Assets/Prefabs/Table/`, `Assets/Prefabs/Minigames/`.

Índice geral: [README dos Docs](README.md). Montagem no Unity: [Manual do Designer](Manual-Designer.md). Dados de onomatopeia: [Manual de ScriptableObjects](Manual-ScriptableObjects.md).

---

## 1. Props (onomatopeias e mesa)

### 1.1 Conteúdo pedagógico (por onomatopeia no catálogo)

Cada entrada do catálogo é **uma unidade**: letra + figura + som. No repositório existem 3 placeholders (C, F, J) com sprite e **sem áudio**.

| Asset | Formato sugerido | Uso no jogo | Critérios |
|--------|------------------|-------------|-----------|
| **Figura da onomatopeia** | Sprite 2D (PNG, fundo transparente) ou mesh 3D + textura | `FigureSprite` na mesa (`SoundObjectInstance`) e na carta do HUD (`card-figure`) | **Único por definição** no trio; silhueta legível em ~96×96 UI e na mesa |
| **Prop físico (slot 0–2)** | Prefab `SoundObject_Base` | Collider + `SoundObjectInstance`; figura aplicada em runtime | 3 instâncias por mesa; collider alinhado ao clique; **não** embutir letra fixa no mesh |
| **Rótulo escrito** | Texto / tipografia (design) | `WrittenLabel` (ex.: “Toc”, “Pum”) | Curto, legível para crianças; coerente com o áudio |
| **Letra de exibição** | Glyph ou arte de letra | `LetterDisplay` | Uma letra distinta por definição no pool da partida |

**Escala de conteúdo**

- **MVP jogável:** mínimo **3** onomatopeias (3 letras distintas) — piso do `OnomatopoeiaCatalog`.
- **Catálogo real:** biblioteca maior (ex.: 12–24+), pois cada partida sorteia **3 sem repetição**; sprites e clips **não podem repetir** entre as 3 sorteadas (`OnomatopoeiaMatchSampler`).

### 1.2 Mesa e ambiente (Blitz Onomatopoeico)

| Asset | Descrição |
|--------|-----------|
| **Mesa / tabuleiro** | Superfície onde ficam os 3 props; leitura clara na câmera |
| **Âncoras** | Marcadores visuais ou vazios: `Slot_0`, `Slot_1`, `Slot_2`, opcional `CardMount` / `CardAnchor` |
| **Cenário aditivo** `31_Minigame_Blitz` | Iluminação URP, fundo, props decorativos (não interativos) |
| **Prefab** `TableLayoutRoot` | Layout reutilizável em `Assets/Prefabs/Table/` |

### 1.3 Variante Fantasma Ladrão (`32_Minigame_Fantasma`)

| Asset | Descrição |
|--------|-----------|
| **3 objetos-letra 3D** | Letras coloridas clicáveis (fantasia “ladrão de sons”); colliders dedicados |
| **Mesa / ambiente Fantasma** | Visual distinto do Blitz; mesma lógica de 3 alvos → slots 0–2 via adaptador |
| **Prefabs** | `Assets/Prefabs/Minigames/FantasmaLadraoSons/` |

### 1.4 Opcional (roadmap de dados)

| Asset | Descrição |
|--------|-----------|
| **ID visual (`FigureVisualId`)** | Tabela de correspondência arte ↔ `ushort` para VFX/UI futuros |
| **Variantes de figura** | Fácil / médio / difícil (blueprint); não obrigatório no protótipo |

---

## 2. UI (UI Toolkit)

Estrutura em `Assets/UI Toolkit/UXML/`. Hoje é **layout funcional** com `Theme.uss` (cores/tokens), **sem** ícones, fundos ilustrados nem fonte de marca.

### 2.1 Telas (UXML existentes)

| Tela | Arquivo | Assets visuais necessários |
|------|---------|----------------------------|
| **Menu principal** | `MainMenu/MainMenu.uxml` | Logo/título, fundo, ícones nos botões, estados hover/disabled do “Continuar”, arte para dropdowns (dificuldade, minijogo) |
| **Lobby (8 lugares)** | `Lobby/Lobby.uxml` + `LobbySeatRow.uxml` | Fundo de sala, avatar placeholder, moldura de assento, ícone “Pronto”, estados vazio/ocupado/host |
| **HUD (partida)** | `HUD/HUD.uxml` | Moldura da **carta** (`card-mount`), fundo do timer/fase, barra ou ícone de pontuação, tipografia para letra + cue + prompt |
| **Resultados** | `Results/Results.uxml` | Medalhas/colocação, painel de resumo, botões “Ver ranking” / “Menu” |
| **Ranking** | `Leaderboard/Leaderboard.uxml` | Cabeçalho, linha de entrada (top 20), destaque top 3 |

### 2.2 Sistema visual (USS)

| Asset | Descrição |
|--------|-----------|
| **Theme expandido** | Paleta final, espaçamentos, radius — base em `Theme.uss` |
| **Componentes** | `ButtonPrimary.uss` + futuros: secundário, toggle, card, timer urgente |
| **Fontes** | TTF/OTF licenciadas; tamanhos para título, corpo, HUD compacto |
| **Sprites para UI Toolkit** | Backgrounds 9-slice, ícones PNG importados como `Sprite` / `VectorImage` |
| **Panel Settings** | Background opcional, escala DPI (tablet/desktop) |

### 2.3 Carta no HUD (prioridade alta)

O HUD mostra: letra da carta, imagem da figura (`card-figure`, 96×96 no UXML), texto do som (`WrittenLabel`), prompt de regra (positivo / exclusão).

**Entregar:** template visual de carta (frente), estados **revelação** (~0,35 s em `RoundPresent`) e **grab aberto**, e opcional flash acerto/erro (ligado ao `GameplayFeedbackBus` no futuro).

### 2.4 Prefabs de UI

Hosts de `UIDocument` em `Assets/Prefabs/UI/` — útil para UI world-space ou navegação por prefab.

---

## 3. SFX

Pasta alvo: `Assets/Audio/`. O código toca **`AudioClip` por onomatopeia** na cue da carta (`LocalMatchSession`, `SessionAudioDirector`).

### 3.1 Por onomatopeia (obrigatório para catálogo válido)

| Clip | Descrição | Formato |
|------|-----------|---------|
| **Som da onomatopeia** | Vocalização / efeito fiel ao `WrittenLabel` | WAV/OGG, mono, duração curta (~0,3–1,5 s) |

**Regra técnica:** os 3 clips de um trio sorteado devem ser **distintos** (mesma regra dos sprites).

### 3.2 UI e fluxo

| Clip | Momento |
|------|---------|
| Clique botão (menu, lobby, resultados) | Navegação |
| Toggle “Pronto” | Lobby |
| Contagem / tick do timer de grab | `GrabPhase` (janela 2,5–3 s conforme dificuldade) |
| Aviso “últimos segundos” | Final da janela de grab |
| Transição de fase | `RoundPresent` → `GrabPhase` → `SpeakPhase` → `ResolveRound` |

### 3.3 Gameplay

| Clip | Momento |
|------|---------|
| Carta entra / flip | `RoundPresent` |
| Hover / highlight no prop | Interação (planejado) |
| Grab registrado | Clique válido na mesa |
| Grab rejeitado / fora de fase | Multiplayer futuro |
| Acerto na rodada | `ResolveRound` + pontuação |
| Erro na rodada | Mesma fase |
| Fim de partida | `MatchEnd` |
| Música de menu / gameplay / resultados (opcional) | Loops discretos |

### 3.4 Fantasma

Mesmos SFX de feedback; opcional **camada temática** (sussurro, “fantasma”) sem substituir os sons pedagógicos das onomatopeias.

---

## 4. VFX

Ainda **não há** prefabs de partículas no repositório; o blueprint prevê pool + `GameplayFeedbackBus` (hoje só `ToastRequested`).

### 4.1 Mesa e interação

| Efeito | Momento |
|--------|---------|
| **Hover / outline** no prop | Pré-grab |
| **Press / seleção** | Clique durante `GrabPhase` |
| **Highlight do slot correto** (opcional, modo fácil) | Ensino |
| **Pulso no prop** | Início da janela de grab |

### 4.2 Carta e rodada

| Efeito | Momento |
|--------|---------|
| **Entrada da carta** | `RoundPresent` (~0,35 s) |
| **Burst de cue** | Quando o áudio da onomatopeia toca |
| **Timer crítico** | Últimos ~0,5 s da janela |
| **Acerto** | `ResolveRound` |
| **Erro** | `ResolveRound` |
| **Transição entre rodadas** | `RoundPrepare` → próxima carta |

### 4.3 UI

| Efeito | Momento |
|--------|---------|
| Flash no `card-mount` | Acerto/erro |
| +1 ponto | Atualização de score no HUD |
| Toast / banner | `GameplayFeedbackBus.RaiseToast` |

### 4.4 Fantasma

| Efeito | Descrição |
|--------|-----------|
| Aura / rastro nas letras 3D | Identidade do minijogo |
| “Roubo” do som (opcional) | Reforço da fantasia em cartas mismatch |

### 4.5 Organização

Prefabs em `Assets/Art/VFX/` ou subpastas por minijogo; versões **leves** se o alvo for WebGL/mobile.

---

## 5. Shaders (URP)

### 5.1 Props e mesa

| Shader / material | Uso |
|-------------------|-----|
| **Sprite lit / unlit** | Figuras 2D na mesa |
| **Toon / estilizado** (opcional) | Direção de arte party educativo |
| **Outline / hover** | `InteractableFeedback` — contorno no raycast |
| **Dissolve / flash** | Feedback de acerto/erro no prop |
| **3D letras (Fantasma)** | Cor por slot + outline no hover |

### 5.2 Carta e HUD world-space (se usar mesh/plane na cena)

| Shader | Uso |
|--------|-----|
| **Card flip / reveal** | Animação de apresentação da carta 3D |
| **Unlit UI-on-mesh** | Texto/figura na mesa |

### 5.3 Pós e ambiente (opcional)

| Shader | Uso |
|--------|-----|
| **Bloom suave** | Destaques de acerto |
| **Color grading LUT** | Por cena (menu vs Blitz vs Fantasma) |
| **Vignette leve** | Foco nos 3 props |

### 5.4 UI Toolkit

Em geral **USS + sprites**; shaders só se houver elementos 3D/UI híbridos ou `RenderTexture` para carta no mundo.

---

## 6. Prioridades de entrega

### P0 — desbloqueia playtest com conteúdo real

1. **3+ onomatopeias completas** (sprite + áudio + labels únicos)
2. **Prefab mesa Blitz** (3 props + colliders + `TableLayoutRoot`)
3. **Arte da carta no HUD** (moldura + legibilidade)
4. **SFX:** som por onomatopeia + acerto/erro + click UI

### P1 — polish de partida

5. VFX hover, grab, acerto/erro
6. Shaders outline + flash
7. SFX timer e entrada de carta
8. Menu + lobby com identidade visual

### P2 — segundo minijogo e escala

9. Cena + props **Fantasma** (letras 3D)
10. Expansão do catálogo (biblioteca > 3)
11. Resultados + ranking ilustrados
12. Música ambiente

---

## 7. Convenções de handoff

| Item | Convenção |
|------|-----------|
| Sprites de figura | `Assets/Art/Onomatopoeia/Ono_<Id>_<Nome>_Figure.png` |
| Áudio | `Assets/Audio/Onomatopoeia/Ono_<Id>_<Nome>.wav` |
| Props | `Assets/Prefabs/Table/SoundObject_Base.prefab` + variantes por tema |
| UI | Sprites em `Assets/Art/UI/`; preservar nomes UXML (`card-letter`, `card-figure`, etc.) |
| `FigureVisualId` | Planilha compartilhada com design (`ushort` ↔ asset) |

---

## 8. Lacunas atuais no repositório

- `Assets/Audio/` — vazio (só `.gitkeep`); clips das 3 letras **não** ligados nos ScriptableObjects
- `Assets/Prefabs/*` — pastas reservadas, **sem** prefabs de mesa/props
- `Assets/Art/` — PNGs placeholder apenas
- VFX / shaders de feedback — **planejados** no blueprint, **não** implementados no código

---

## 9. Referências

- [Manual do Designer](Manual-Designer.md) — pastas, mesa 3 slots, UI Toolkit
- [Manual de ScriptableObjects](Manual-ScriptableObjects.md) — `OnomatopoeiaDefinition`, catálogo, requisitos de sprite/áudio distintos
- [Manual do Game Designer](Manual-Game-Designer.md) — fases da rodada e regras positivo/exclusão
- Blueprint: `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`
