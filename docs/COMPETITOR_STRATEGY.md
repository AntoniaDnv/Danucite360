# Danucite360 vs СИГМА Бюджет — strategy (2026-11-18)

Reviewed live: `/`, `/danaci`, `/izpalnenie`, `/obshtini`, `/vedomstva`, `/merki`, `/byudzhet-2027`, `/api`, `/proverka` on sigma-budget.midt.bg.

## The honest read
СИГМА has already won "the national budget explained". Its data runs to **30.09.2026** (ours: April), it covers the 2027 draft,
all 265 municipalities (transfers), 47 ministries, measures transcribed from official tables with excerpts from the motives,
a salary/tax calculator, CSV + an MCP server for AI assistants, and a reproducible release with SHA-256 for every input file.
Matching that is a feature-parity trap. Whatever we copy, they already do better and with official data.

What СИГМА does **not** do: it stops at the level of the budget line. The 226 investment projects from Annex 2 appear as one
table section of the 2027 page: amounts per ministry, no page per project, no location, no contract, no payments, no progress.
Nobody follows a single project from "money allocated in the law" to "what was built".

## 1. Competitive comparison
| Capability | СИГМА | Danucite360 | Verdict |
|---|---|---|---|
| National budget in plain language | Strong, official, to Sep 2026 | Good, official, to Apr 2026 | Do not compete; update our data only |
| In-year execution | Monthly, 2022–2026, vs prior year | Plan vs actual vs time elapsed | Keep ours small; no race |
| Ministries / municipalities | 47 ministries, 265 municipalities | 6 demo regions | Do not copy; link out if useful |
| Budget measures | Official, full list, motives | Demo, but **filtered by who you are** | Our angle is personalization; data must become official |
| Tax calculator | Yes | No | Not worth copying |
| Open data / API | CSV + MCP, provenance hashes | CSV + JSON | Adequate for us |
| Data provenance | Release manifest, SHA-256 | Status label + source per figure | Ours is per figure; good enough |
| **Projects: map, page per project** | No | **Yes** (demo) | **Differentiator** |
| **Project execution (paid vs built)** | No | **Yes** (demo) | **Differentiator** |
| **Evidence trail per project** | No | No | **Build next** |
| Shareable project report | No | No | Cheap add-on to the trail |

## 2. Positioning
For Bulgarian citizens and local journalists who want to know what actually happened with public money in their town,
Danucite360 follows **individual public projects** from the line in the budget law to the contract, the payments and the
finished work, with a document behind every step. Unlike budget dashboards that stop at the budget line, it answers
"what was this money spent on, where, and can I check it?"

## 3. Top three differentiators (scored)
| Idea | Citizen | Journalist | Different | Data availability | Effort | Demo | Long-term |
|---|---|---|---|---|---|---|---|
| **Source-to-project evidence trail** (+ shareable report) | High | **Very high** | **Very high** | Medium: Annex 2 of the budget law (allocation), ЦАИС ЕОП (contracts), ИСУН 2020 (EU-funded contracts and payments) are public | Medium | **Yes** | **Very high** |
| Project explorer + map (have) | High | Medium | High | Same as above | Done | Yes | High |
| Citizen-personalized measures (have) | **Very high** | Medium | Medium (СИГМА lists, we filter) | High: MoF tables II-7/II-8 are public | Low to make official | Yes | Medium |
| Project-level execution | High | High | High | Medium (payments in ИСУН/ЕОП) | Folded into the trail | Yes | High |
| Monitoring / notifications | Medium | High | High | Needs the trail first | High | No | High |

**Pick:** the evidence trail. It turns the projects we already have into the thing nobody else does, and project-level execution
falls out of it for free: payments are evidence, so paid-to-date is the sum of the payment documents.

Riskiest assumption: that real project-level documents can be linked reliably. Cheapest test after the demo: take 3 real
EU-funded projects from ИСУН and trace them by hand (allocation → contract → payments). If that takes under an hour each, the model holds.

## 4. Roadmap
- **P0 (today):** evidence trail on project pages with demo documents, clearly labelled; paid-to-date equals the sum of payment
  evidence; printable/shareable project report; evidence in the project JSON.
- **P1:** replace demo projects with 5–10 real ones (Annex 2 + ИСУН/ЕОП, entered by hand, verified); make measures official from
  MoF tables II-7/II-8; refresh execution to the latest MoF month.
- **P2:** admin CRUD for evidence; monitoring and e-mail alerts on project changes; importer for ИСУН exports.
- **Do not build:** tax calculator, 265-municipality tables, ministry budgets, MCP server — СИГМА covers them.

## 5. Minimum data and API for the evidence trail
`ProjectEvidence` (new table, additive migration): `Id`, `FundedProjectId` (FK, cascade), `Stage`
(allocation | procurement | contract | payment | progress), `Title`, `OccurredOn`, `Amount` (thousand EUR, nullable),
`DocumentTitle`, `DocumentUrl`, `IsDemo`, `SortOrder`.
Rule: for a project with payment evidence, `Spent` = sum of payment amounts (tested).
API: `GET /api/projects/{slug}` → project + ordered evidence. Pages: `/projects/{slug}` timeline, `/projects/{slug}/report` printable.

## 6. Demo acceptance criteria
Open a project → see the chain allocation → procurement → contract → payments → progress, each with date, amount, document and
status dot; paid-to-date on the page equals the sum of payments; "Сподели / печат" opens a clean one-page report with a permanent
link; `/api/projects/{slug}` returns the same evidence; every demo document is labelled demo; Regions page stays map-free;
`/projects/map` unchanged.

## 7. Sequence
1. Entity + migration + seed (deterministic, consistent with Spent) + test for the Spent invariant.
2. Service + JSON endpoint. 3. Timeline on the project page. 4. Report page with print CSS. 5. Build, tests, deploy, verify live.
