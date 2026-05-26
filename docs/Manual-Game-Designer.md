# Manual do Game Designer

Manual para **regras**, **loop de jogo**, **pacing**, **multijogador na UX** e **como pedir features** à equipe técnica. Você não precisa ler C# nem asmdefs; quando precisar de detalhe de implementação, use o [Manual do Programador](Manual-Programador.md).

## 1. Visão do produto (resumo de uma página)

- **Formato:** sessões curtas, estilo *party* educativo.
- **Mesa:** **três** objetos sonoros (slots 0–2). Em cada rodada, **só um** é a resposta correta segundo as regras.
- **Carta:** uma **letra** em foco (`CardLetterId`) + um **par de estímulos** — canal **figura** + canal **som** (onomatopeia / áudio). O jogador olha/ouve e **agarra** o objeto certo; depois há uma fase de **fala** (honor system, sem ASR no protótipo).
- **Pontuação (protótipo):** ganhar a carta incrementa pontos no `RoundController` / `LocalMatchSession` (consulte o código quando precisar de números exatos).

Referência profunda de arquitetura e dados: `.cursor/plans/blitz_onomatopoeico_architecture_d4df416d.plan.md`.

## 2. Loop de partida (jogador)

Fluxo **alvo** (GDD) vs **estado atual** do protótipo:

| Etapa (GDD) | Estado no protótipo |
|-------------|---------------------|
| Menu principal | UXML + `MainMenuView` existem; fluxo de cenas pode ser manual |
| Lobby (até 8 lugares) | `LobbyView` + `LobbyServiceHost` + stub `LobbyServiceStub` (simulação local) |
| Match / rodadas | **Offline:** `LocalMatchSession` + `RoundController` — jogável com cena mínima |
| Resultados / ranking | UXML + views; ranking offline persiste em JSON local (top 20), preenchido ao fim de cada partida offline |

Para o **primeiro play**, siga [Primeiros passos](Getting-Started.md).

## 3. Regras da rodada (linguagem de design)

### Universo fechado

Numa rodada há **três letras** e **três fonemas** “ensinados” para aquela partida, em **bijeção** (cada letra tem um som verdadeiro `T(L)` e os três sons aparecem na mesa numa **permutação** nos slots).

### Modo positivo (“true pair”)

- O **cue** (som escrito/reproduzido na carta) é **igual** ao som verdadeiro da **letra da carta**.
- Regra do jogador: agarre o objeto cuja identidade sonora na mesa **é** esse cue.
- Há **exatamente um** objeto assim se o conteúdo estiver bem formado.

### Modo exclusão (“mismatch”)

- O cue **não** é o som verdadeiro da letra da carta.
- Regra do jogador (intuição): elimine objetos incoerentes com o texto da carta e com o cue; no conjunto fechado de três, a solução deve ser **única**.

### Por que isso importa em sala de aula

Ambiguidade = discussões infinitas e perda de confiança no conteúdo. O **gerador** e o **resolver** foram desenhados para **garantir solução única** quando o modo é exclusão (com verificação; veja os testes).

Ligação conceitual ao código (nomes para falar com programação):

- Regras fechadas: `AnswerResolver` + `CardGenerator` + `CardUniqueness` em `Assets/Scripts/Core/`.
- Testes de sanidade: `Assets/Tests/EditMode/Core/AnswerResolverTests.cs` (“rede de segurança” contra regressões de design ambíguo).

## 4. Fases de uma rodada (`MatchPhase`)

O fluxo offline está em `RoundController` (`Assets/Scripts/Gameplay/Match/RoundController.cs`). Em linguagem de **tela / áudio**:

| Fase (código) | O que o jogador percebe (protótipo) | Risco de UX se falhar |
|-----------------|-------------------------------------|------------------------|
| `MatchInit` | Arranque interno | Transição lenta ou “tela parada” |
| `RoundPrepare` | Nova carta / novo conjunto (pode ser sutil) | Carta antiga visível demais |
| `RoundPresent` | Carta “entra” (~0,35 s no stub) | Leitura insuficiente antes do grab |
| `GrabPhase` | Janela para agarrar (~`MatchRules.GrabWindowSeconds`) | Janela curta demais para crianças |
| `SpeakPhase` | Tempo para falar (~0,25 s no stub) | Hoje é curto — ajustar quando houver UX de fala |
| `ResolveRound` | Feedback de acerto/erro + atualização de pontos | Feedback tardio quebra confiança no input |
| `MatchEnd` | Fim da sessão | Falta de tela de resultados |

**Nota de design:** os tempos curtos no `RoundController` são para **iteração técnica**; no GDD você deve propor valores finais (e justificar por idade / turma).

## 5. Dificuldade e conteúdo (roadmap)

### Já existe como parâmetro simples

- `MatchRules`: **número de rodadas** e **duração da janela de grab** (`Assets/Scripts/Core/MatchModels.cs` + uso em `LocalMatchSession` / `OfflineQuickStart`).

### No blueprint, ainda como conceito / SO

- Perfil de dificuldade (bots, tier de distrator, tamanho de “baralho”), curadoria de letras/fonemas, políticas de geração de cartas.

### Como pedir uma feature (modelo)

Copie para o seu ticket / issue:

1. **User story:** “Como [papel], quero [ação], para [benefício].”
2. **Contexto de mesa:** número de slots, idioma, restrições pedagógicas.
3. **Regra de ouro:** a solução tem de ser **única** por rodada; descreva casos limite.
4. **Critérios de aceitação:** passos reproduzíveis + comportamento esperado + telemetria/playtest se aplicável.
5. **Referência técnica:** aponte para seção do blueprint ou para `AnswerResolver` / `RoundController` se já souber onde bate.

## 6. Multijogador (experiência do jogador)

### Autoridade

- **Servidor** (conceito NGO): valida **grab**, **fase**, **pontuação** e progressão de rodada.
- **Cliente:** envia **intenção** (ex.: “cliquei no slot X”), mostra UI e animações **após** confirmação quando o jogo estiver completo.

### Estado do protótipo de rede

- `NetMatchSession` (`Assets/Scripts/Networking/NGO/NetMatchSession.cs`) replica **fase** e **pontuação** e expõe um RPC de grab para validação no servidor.
- Limitação explícita para alinhar expectativas: **a carta completa no cliente** pode não estar replicada byte a byte neste stub — o GD deve tratar o MP como “**servidor manda na verdade**”, UI como “**espelho**”.

## 7. Minijogos (visão de systems)

- **Blitz onomatopoeico:** mesa clássica com carta e três objetos sonoros.
- **Fantasma ladrão de sons:** mesma “cabeça” de resolução, com **adaptador** (`ISolutionSpaceAdapter`) para mapear “o que foi agarrado no mundo” → slot canônico 0–2 — útil quando o alvo visual não é o mesmo prefab base.

Fantasia e assets: [Manual do Designer](Manual-Designer.md).  
Contratos e extensão: [Manual do Programador](Manual-Programador.md).

## 8. Playtest (roteiro curto, 5–10 min)

1. **Objetivo da sessão:** validar leitura da carta + grab + sensação de justiça.
2. **Coortes:** idade / letramento; anote se leem sozinhos ou em voz alta.
3. **Tarefas:** 5 rodadas alternando **positivo** e **exclusão** (se o gerador permitir na build).
4. **Métricas simples:** taxa de primeiro grab correto, tempo até o primeiro grab, confusões verbalizadas (“há dois certos?”).
5. **Registro de bug:** passos + captura de tela + versão Unity (`ProjectSettings/ProjectVersion.txt`) + seed da partida se disponível.

## 9. Glossário (consistente com o blueprint)

| Termo | Significado |
|-------|-------------|
| **Letra da carta** | Letra pedagógica em foco na carta (`CardLetterId`). |
| **Cue / fonema do cue** | Som escrito/ouvido na carta que entra nas regras positivo/exclusão. |
| **Som verdadeiro da letra** | `T(L)` — o fonema ensinado para aquela letra nesta partida. |
| **Slot** | Índice 0–2 do objeto físico na mesa. |
| **Rodada** | Uma carta + janela de grab + fala + resolução. |
| **Match** | Sequência de rodadas até condição de fim (ex.: número de rodadas). |
| **Lobby** | Sala de espera até 8 lugares (stub local). |

## 10. Onde aprofundar

- [Primeiros passos](Getting-Started.md)  
- [Manual do Designer](Manual-Designer.md)  
- [Manual do Programador](Manual-Programador.md)  
- [README dos Docs](README.md)
