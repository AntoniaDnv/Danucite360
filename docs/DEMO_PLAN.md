# Danucite360 — Media demo plan (deadline 2026-11-18)

Brief: make a credible, working demo of the six "Бюджет на светло" capabilities.
Bulgarian is the primary demo language. Working > pretty. Never present demo data as official.

## Verified state (2026-11-17, branch feat/projects-investments)
- Build: succeeded, 0 errors (16 warnings). Tests: 24/24 pass.
- Migrations: InitialCreate, AllowNegativeBudgetAmounts, AddFundedProjects, AddDebtSnapshots.
- Entities: Region, BudgetCategory, BudgetRecord, BudgetSource, ApplicationUser,
  FundedProject, ProjectFunding, DebtSnapshot (+ breakdown/trend/auction parts).
- Public controllers: Home, Budget, Map (regions), Regions, Projects, Funding, Debt, Sources. Admin area: Dashboard, BudgetRecords, Regions CRUD.
- **Docker/SQL was DOWN** — start Docker Desktop and container `sql1` before anything (app migrates on startup). Connection string in Danucite360/appsettings.json.
- UI is **English** today. Design system: Public Sans + JetBrains Mono, navy/teal tokens in wwwroot/css/site.css, shared wwwroot/js/chart-theme.js.

## Six-capability gap analysis
| Cap | Status | Smallest demo version |
|---|---|---|
| A Unified portal | Mostly done (7-tab nav, all routes 200) | Tighten nav into a clear IA, homepage that guides into the features |
| B Citizen budget | Mostly done (KPIs, plain-language boxes, sources) | **Translate demo path to Bulgarian**; "what this means for you" lines |
| C Open data | **Missing** | CSV export + JSON endpoint from DB (national records, projects, measures) with unit/period/source/status columns |
| D Project map | **Placeholder** | Leaflet map, markers at city-level coords for FundedProject, popup -> /projects/{slug}; fallback to list if tiles fail |
| E Measures register | **Missing** | New BudgetMeasure entity + seed (demo-labelled), /measures page, filter by age + role (parent, driver, pensioner...) |
| F Execution tracking | Partly done (CFP + state-budget execution on /budget/national, project completion %) | Clear planned vs actual vs % vs reporting period; physical vs financial progress on projects |

## Plan
P0 (must work): start DB; Measures register (E); project map (D); CSV + JSON (C); Bulgarian on demo path (nav, home, national, projects/map, measures, sources); homepage ties it together; all routes no 500s.
P1: execution panel polish, source links on every chart, mobile pass, admin edit -> public update demo.
P2 (after demo): PDF import, full municipal data, accounts, procurement/SIGMA integration, multi-tenant.

Migrations to add: `AddBudgetMeasures` (BudgetMeasure table + audience tags), `AddProjectCoordinates` (Latitude/Longitude/City on FundedProject). Non-destructive, idempotent seed.

## Progress
- 2026-11-17: Docker + sql1 up, all routes 200. **E done**: BudgetMeasure entity + migration AddBudgetMeasures (additive),
  9 demo-labelled measures seeded, IMeasureService (roles OR'd + "everyone", AND age), /measures + /measures/{slug} in Bulgarian,
  "Мерки" nav link. Tests 30/30 (6 new in MeasureServiceTests).

- 2026-11-17 (later): **D** project map /projects/map (Leaflet + OSM tiles, list fallback, physical vs financial progress; migration AddProjectLocationAndSpending).
  **C** /open-data + CSV (budget-records, projects, measures; UTF-8 BOM) + JSON /api/budget-records|projects|measures. **F** plan vs actual vs % vs period on /budget/national#execution.
  **Bulgarian**: all public pages, nav (7 sections), money format "30,37 млрд. €", DB text via SeedData.Localization (only rows still in English).
  Removed unverifiable claims/fake controls (pipeline story, 65/35 bars, fake search/live/lang switch, real-looking procedure codes).
  Admin password moved out of source -> config `Seed:AdminPassword`. Tests 36/36.
- **Deployed**: https://danucite360.azurewebsites.net (rg-danucite360, Sweden Central, App Service F1 + Azure SQL free offer). Redeploy: `deploy/azure-deploy.ps1`. Secrets: %USERPROFILE%\danucite360-azure-secrets.txt.

- 2026-11-18: **Design pass (unslop)** live: ink-grey ramp, single accent #2563EB (wwwroot/css/civic.css, loaded after site.css),
  weights <=600, no uppercase labels, emoji icons removed, home hero = plan-vs-actual ledger with time-elapsed marker,
  contents list instead of tiles, spending donut -> ranked bars, citation line (source · period · status dot) under figures.
  Deploy package slimmed to ~9 MB (no maps/pdb/win runtimes); SQL CommandTimeout 120s for free-tier resume.

## Next action
1. (done) Redesign pass.
2. Admin journey: Identity login page is still English (default UI); verify admin edit -> public update.
3. Run graphify on the repo (requested). Commit work on branch.
4. Before demo: open the site 2-3 min early (F1 sleeps, free SQL auto-pauses ~1 min resume).

## Definition of done
Builds, tests pass, app starts from a cold Docker start; every nav link works; map shows projects and opens details; measures filter returns results; CSV downloads and opens; JSON endpoint returns data matching the dashboard; every demo value carries a status label; demo path in Bulgarian; admin can edit a record and see it public.

## Demo walkthrough (2-3 min)
Home -> national budget (planned vs actual) -> project map -> measures ("Родител" + "Шофьор") -> execution + sources -> download CSV / open JSON.
