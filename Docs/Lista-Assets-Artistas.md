# Lista de assets — equipe de arte

Checklist de produção visual e áudio para **Blitz Onomatopoeico**, alinhada ao que o projeto **já consome** no código (`OnomatopoeiaDefinition`, mesa com 3 slots, UI Toolkit) e ao que está **planejado** no blueprint (`GameplayFeedbackBus`, feedback de hover, `FigureVisualId`).

**Pastas do projeto:** `Assets/Art/`, `Assets/Audio/`, `Assets/UI Toolkit/`, `Assets/Prefabs/Table/`, `Assets/Prefabs/Minigames/`.

Índice geral: [README dos Docs](README.md). Montagem no Unity: [Manual do Designer](Manual-Designer.md). Dados de onomatopeia: [Manual de ScriptableObjects](Manual-ScriptableObjects.md).

**Neste documento:** [§7 Convenções de imagem](#7-convenções-de-imagem-tamanho-dpi-e-formato) · [§9 Checklist de produção](#9-checklist-de-produção) · minijogo **Fantasma Ladrão** marcado como *opcional* em todo o texto.

**Integração no Unity (programação):** [Cronograma de integração — programador](Cronograma-Integracao-Programador.md) (hooks `IMinigame`, fases A–C, checklist I1–I16).

---

## 1. Props (onomatopeias e mesa)

### 1.1 Conteúdo pedagógico (por onomatopeia no catálogo)

Cada entrada do catálogo é **uma unidade**: letra + figura + som. No repositório existem 3 placeholders (C, F, J) com sprite e **sem áudio**.

| Asset | Formato sugerido | Uso no jogo | Critérios |
|--------|------------------|-------------|-----------|
| **Figura da onomatopeia** | Sprite 2D (PNG, fundo transparente) ou mesh 3D + textura | **Um único** `FigureSprite` por definição: props na mesa (`SoundObjectInstance`) **e** imagem na carta do HUD (`card-figure`, 96×96 px no layout) | **Único por definição** no trio; **não** criar PNG separado “só para o HUD”; silhueta legível na mesa e reduzida na UI |
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

### 1.3 Variante Fantasma Ladrão (`32_Minigame_Fantasma`) — *opcional*

Minijogo secundário (P2). Não bloqueia playtest do Blitz Onomatopoeico.

| Asset | Descrição | Obrigatório |
|--------|-----------|-------------|
| **3 objetos-letra 3D** | Letras coloridas clicáveis (fantasia “ladrão de sons”); colliders dedicados | Não |
| **Mesa / ambiente Fantasma** | Visual distinto do Blitz; mesma lógica de 3 alvos → slots 0–2 via adaptador | Não |
| **Prefabs** | `Assets/Prefabs/Minigames/FantasmaLadraoSons/` | Não |

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
| **HUD (partida)** | `HUD/HUD.uxml` | Moldura da **carta** (`card-mount`); figura do estímulo vem do catálogo (`card-figure`); fundo do timer/fase, pontuação, tipografia para letra + cue + prompt |
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

Durante a partida, o painel `card-mount` no HUD (`HUD.uxml`) combina **conteúdo dinâmico** (vindo do catálogo) e **arte de UI** (moldura/estilo):

| Elemento no HUD | Origem do asset | Quem entrega |
|-----------------|-----------------|--------------|
| Letra da carta (`card-letter`) | `LetterDisplay` da definição | Design / tipografia (§1.1) |
| **Figura** (`card-figure`, 96×96) | **`FigureSprite` da onomatopeia do estímulo** — mesmo arquivo da mesa | Arte §1.1 (`HudPresenter` aplica o sprite em runtime) |
| Texto do som (`card-cue`) | `WrittenLabel` | Design §1.1 |
| Prompt de regra | Texto gerado pelo jogo | N/A (cópia em código) |
| **Moldura / fundo da carta** | Sprite 9-slice ou USS (`blitz-card`) | Arte §2.2 / §2.3 |

**Não confundir:** a “figura na carta” **não** é um item extra na checklist — é a mesma figura do catálogo, só exibida menor na UI. O item de produção separado aqui é o **template visual da carta** (borda, fundo, hierarquia), não um segundo PNG por onomatopeia.

**Entregar (UI):** moldura/frente da carta, estados **revelação** (~0,35 s em `RoundPresent`) e **grab aberto**, e opcional flash acerto/erro (ligado ao `GameplayFeedbackBus` no futuro).

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

### 3.4 Fantasma Ladrão — *opcional*

Mesmos SFX de feedback do Blitz; **camada temática** (sussurro, “fantasma”) é opcional e não substitui os sons pedagógicos das onomatopeias.

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

### 4.4 Fantasma Ladrão — *opcional*

| Efeito | Descrição | Obrigatório |
|--------|-----------|-------------|
| Aura / rastro nas letras 3D | Identidade do minijogo | Não |
| “Roubo” do som | Reforço da fantasia em cartas mismatch | Não |

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
| **3D letras (Fantasma)** *(opcional)* | Cor por slot + outline no hover |

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
3. **Moldura da carta no HUD** (figuras = item 1; validar legibilidade em 96×96)
4. **SFX:** som por onomatopeia + acerto/erro + click UI

### P1 — polish de partida

5. VFX hover, grab, acerto/erro
6. Shaders outline + flash
7. SFX timer e entrada de carta
8. Menu + lobby com identidade visual

### P2 — escala e segundo minijogo *(opcional)*

9. *(Opcional)* Cena + props **Fantasma Ladrão** (letras 3D)
10. Expansão do catálogo (biblioteca > 3)
11. Resultados + ranking ilustrados
12. Música ambiente

---

## 7. Convenções de imagem (tamanho, DPI e formato)

Referência para exportação da equipe de arte. O Unity importa por **pixels**; o DPI abaixo é só convenção em ferramentas (Figma, Photoshop, Illustrator) — use **72 DPI** na base e **144 DPI** quando exportar `@2x`.

| Tipo de imagem | Dimensões (px) | DPI (export) | Formato | Observações |
|----------------|----------------|--------------|---------|-------------|
| **Figura da onomatopeia** | 512×512 (arte fonte); exibida ~96×96 no HUD e maior na mesa | 72 / 144 @2x | PNG RGBA | **Um PNG por item** (`Ono_*_Figure.png`); usado na mesa **e** em `card-figure` — não há entrega “sprite só HUD” |
| **Moldura / template da carta (HUD)** | Largura do painel `card-mount`; 9-slice se aplicável | 72 | PNG RGBA | Fundo/borda da carta; **não** inclui as figuras do catálogo |
| **Ícones de UI** | 64×64 e 128×128 (@2x) | 144 | PNG RGBA | Botões, “Pronto”, estados de lobby |
| **Backgrounds de tela** | 1920×1080 (safe 16:9) ou maior com safe area | 72 | PNG / JPG | JPG só se não precisar de transparência; UI Toolkit aceita 9-slice |
| **Painéis 9-slice** | Bordas ≥ 32 px; centro repetível | 72 | PNG RGBA | Definir borders no import Unity |
| **Logo / título** | Largura máx. ~800 px (altura proporcional) | 144 | PNG RGBA | Versão compacta para HUD se necessário |
| **Avatar / assento (lobby)** | 128×128 (@2x de 64×64) | 144 | PNG RGBA | Placeholder e molduras separadas se aplicável |
| **Medalhas / ranking** | 96×96 a 256×256 conforme destaque | 144 | PNG RGBA | Top 3 podem ter arte maior |
| **Texturas 3D (Fantasma)** *(opcional)* | Potência de 2 (512, 1024) | 72 | PNG RGBA | Albedo + máscara; evitar NPOT |
| **LUT / grading** *(opcional)* | 1024×32 | 72 | PNG | Pós-processamento URP |
| **Fontes** | N/A (vetor) | N/A | TTF / OTF | Licenciadas; não rasterizar UI principal |

**Regras gerais**

- Preferir **PNG com alpha** para UI e figuras; **JPG** apenas para fotos/fundos sem transparência.
- Exportar sempre a **versão @2x** quando o elemento aparecer em telas de alta densidade (tablet/desktop).
- Nomear conforme a seção [Convenções de handoff](#8-convenções-de-handoff); evitar espaços e acentos nos nomes de arquivo.
- **HUD `card-figure`:** validar legibilidade da figura do catálogo em **96×96 px** na tela; export @2x (192 px) é referência de nitidez, não um segundo arquivo obrigatório.

---

## 8. Convenções de handoff

| Item | Convenção |
|------|-----------|
| Sprites de figura | `Assets/Art/Onomatopoeia/Ono_<Id>_<Nome>_Figure.png` |
| Áudio | `Assets/Audio/Onomatopoeia/Ono_<Id>_<Nome>.wav` |
| Props | `Assets/Prefabs/Table/SoundObject_Base.prefab` + variantes por tema |
| UI | Sprites em `Assets/Art/UI/`; preservar nomes UXML (`card-letter`, `card-figure`, etc.) |
| `FigureVisualId` | Planilha compartilhada com design (`ushort` ↔ asset) |

---

## 9. Checklist de produção

Preencher **Responsável** e **Data de entrega** à medida que os itens forem comprometidos. Marcar **Entregue** quando o asset estiver no repositório e referenciado no Unity (ou planilha `FigureVisualId`, quando aplicável).

| # | Asset | Seção | Prioridade | Responsável | Data de entrega | Entregue |
|---|--------|-------|------------|-------------|-----------------|----------|
| 1 | Figura da onomatopeia (lote MVP: 3+ itens; mesa + HUD `card-figure`) | §1.1 | P0 | | | ☐ |
| 2 | Som da onomatopeia (lote MVP: 3+ clips) | §3.1 | P0 | | | ☐ |
| 3 | Rótulo escrito + letra de exibição (design por item) | §1.1 | P0 | | | ☐ |
| 4 | Prefab prop `SoundObject_Base` + colliders | §1.1 | P0 | | | ☐ |
| 5 | Mesa / tabuleiro Blitz | §1.2 | P0 | | | ☐ |
| 6 | Âncoras `Slot_0`–`Slot_2` (+ opcional carta) | §1.2 | P0 | | | ☐ |
| 7 | Prefab `TableLayoutRoot` | §1.2 | P0 | | | ☐ |
| 8 | Moldura / template da carta no HUD (sem figuras do catálogo) | §2.3 | P0 | | | ☐ |
| 9 | SFX acerto / erro / clique UI | §3.2–3.3 | P0 | | | ☐ |
| 10 | Cenário `31_Minigame_Blitz` (luz, fundo, decor) | §1.2 | P1 | | | ☐ |
| 11 | VFX hover, grab, acerto, erro (mesa) | §4.1 | P1 | | | ☐ |
| 12 | VFX carta, timer, transição de rodada | §4.2 | P1 | | | ☐ |
| 13 | Shaders outline + flash (props) | §5.1 | P1 | | | ☐ |
| 14 | SFX timer, entrada de carta, transição de fase | §3.2–3.3 | P1 | | | ☐ |
| 15 | Arte menu principal | §2.1 | P1 | | | ☐ |
| 16 | Arte lobby (assentos, avatares, estados) | §2.1 | P1 | | | ☐ |
| 17 | Theme USS expandido + fontes | §2.2 | P1 | | | ☐ |
| 18 | Sprites UI (9-slice, ícones) | §2.2 | P1 | | | ☐ |
| 19 | VFX flash carta + toast / +1 ponto | §4.3 | P1 | | | ☐ |
| 20 | Arte resultados + ranking | §2.1 | P2 | | | ☐ |
| 21 | Expansão catálogo (biblioteca > 3) | §1.1 | P2 | | | ☐ |
| 22 | Música ambiente (menu / gameplay / resultados) | §3.3 | P2 | | | ☐ |
| 23 | *(Opcional)* 3 objetos-letra 3D — Fantasma | §1.3 | P2 | | | ☐ |
| 24 | *(Opcional)* Mesa / ambiente Fantasma | §1.3 | P2 | | | ☐ |
| 25 | *(Opcional)* Prefabs `FantasmaLadraoSons/` | §1.3 | P2 | | | ☐ |
| 26 | *(Opcional)* SFX camada temática Fantasma | §3.4 | P2 | | | ☐ |
| 27 | *(Opcional)* VFX aura / rastro letras Fantasma | §4.4 | P2 | | | ☐ |
| 28 | *(Opcional)* Shader 3D letras Fantasma | §5.1 | P2 | | | ☐ |
| 29 | *(Opcional)* Tabela `FigureVisualId` | §1.4 | — | | | ☐ |
| 30 | *(Opcional)* Variantes de figura (dificuldade) | §1.4 | — | | | ☐ |

---

## 10. Lacunas atuais no repositório

- `Assets/Audio/` — vazio (só `.gitkeep`); clips das 3 letras **não** ligados nos ScriptableObjects
- `Assets/Prefabs/*` — pastas reservadas, **sem** prefabs de mesa/props
- `Assets/Art/` — PNGs placeholder apenas
- VFX / shaders de feedback — **planejados** no blueprint, **não** implementados no código

---

## 11. Referências

- [Manual do Designer](Manual-Designer.md) — pastas, mesa 3 slots, UI Toolkit
- [Manual de ScriptableObjects](Manual-ScriptableObjects.md) — `OnomatopoeiaDefinition`, catálogo, requisitos de sprite/áudio distintos
- [Manual do Game Designer](Manual-Game-Designer.md) — fases da rodada e regras positivo/exclusão
- Blueprint: `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`
