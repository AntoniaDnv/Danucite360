# Danucite360 — demo walkthrough (2–3 min)

**Live:** https://danucite360.azurewebsites.net
**Warm up 3 minutes before:** open the homepage and `/projects/map` once (the free tier sleeps; first load can take ~1 min).

| Time | Where | What to do | What to say |
|---|---|---|---|
| 0:00 | Начало `/` | Point at the headline and the budget card | "Бюджетът е в PDF-и и таблици. Тук всеки вижда план срещу реално изпълнение: събрани са 26% от приходите след 4 от 12 месеца — червената черта е изминалото време." |
| 0:25 | Бюджет `/budget/national` | Scroll to „2. Какво реално се е случило досега“ | "Планът и отчетът са от един и същ обхват (държавния бюджет), с ясен отчетен период. По-широката КФП е отделно, за да не се смесват числа." |
| 0:55 | Проекти `/projects/map` | Choose „Варна“ in the area filter, click the port marker | "Всеки проект: бюджет, изплатено, физически и финансов напредък — двете не са едно и също." Mention: projects are **demo** records (amber dot). |
| 1:25 | Мерки `/measures` | Tick „Родител“ + „Шофьор“ → 5 measures; open „Данъчно облекчение за деца“ | "Гражданинът избира своята ситуация и вижда какво го засяга, на човешки език. Минус означава разход за бюджета, плюс — приход." Measures are illustrative demo. |
| 1:55 | Данни `/open-data` | Click „Изтегли CSV“ on projects; open „JSON API“ on measures | "Не снимки на таблици — отворени данни. Същите числа като на екрана, със статус и източник на всеки ред." |
| 2:20 | Източници `/sources` | Show the status labels | "Всяко число е отбелязано: официално или демонстрационно. Ако документът е официален, но стойността е въведена ръчно — казваме го." |

## Optional extras (only if asked)
- **Региони** `/Map`: compare two regions side by side (demo data).
- **Admin**: `/Identity/Account/Login` → Бюджетни записи → edit a value → the public chart changes. Credentials are in `%USERPROFILE%\danucite360-azure-secrets.txt`.

## If something fails
- Page slow or error on first load → wait 30–60 s and reload (database waking up).
- Map tiles do not load (network) → the project list under the map still works; use it.
- Site down → run locally: start Docker Desktop and `sql1`, then `dotnet run --project Danucite360` → http://localhost:5181.

## Honest framing (say it before they ask)
National budget and execution figures: official (draft State Budget Law 2026, MoF monthly reports Jan–Apr 2026), entered by hand.
Projects, regional values and measures: demonstration data showing how the platform works, labelled on every screen.
