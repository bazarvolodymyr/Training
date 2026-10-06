# Training — Тренажер іноземної мови (SaaS)

Документ є джерелом правди для команди. Оновлюй цей файл **до** змін у коді, якщо змінюється архітектура або з’являються нові функціональні вимоги.

Пов’язані файли: `docs/tasktracker.md`, `docs/qa.md`, `docs/changelog.md`.

---

## 1. Мета продукту

SaaS для тренування іноземної мови (MVP-контент: українець вивчає англійську, переклад **UA→EN**). Схема **без прив’язки до конкретних мов** (`sourceLang` / `targetLang`): інші мови в перший рік не плануємо, але модель їх не забороняє.

Користувач читає граматичне правило як статтю (WYSIWYG: текст, зображення, таблиці, аудіо) і закріплює вікториною: речення українською → відповідь англійською зі слів/словосполучень або з клавіатури. Прогрес — окремо для кожного рівня складності. Контент і аналітику ведуть Manager/Admin. Повний доступ до вправ — за підпискою Premium.

**Цінність:** безперервні короткі сесії + прозора статистика + контент під контролем, без витоку правильної відповіді до перевірки.

---

## 2. Поточний стан (прототип)

Репозиторій містить вертикальний зріз вікторини без БД, без облікових записів і без статей.

| Шар | Проєкт | Зараз |
|---|---|---|
| API | `Training.API` | ASP.NET Core 9, Swagger, CORS `AllowAll`, `ExerciseController`, маршрути `[controller]/[action]` |
| Application | `Training.Application` | `ExerciseService` + DTO (борг: перейти на CQRS+MediatR) |
| Domain | `Training.Domain` | `Exercise`, `Lesson` (не в потоці), `ExerciseStatus` |
| Persistence | `Training.Persistence` | In-memory заглушка, одне вшите завдання |
| UI | `Training.UI` | Angular 19 standalone, `question-card`: гібрид поле + чіпи |

**Що вже працює**

- GET `Exercise/GetQuestion` — питання без відповіді.
- POST `Exercise/QuestionResponse` — серверна перевірка (trim + lowercase).
- UI збирає відповідь з вводу та підказок-слів.

**Борг відносно узгодженої цілі**

- Немає PostgreSQL, REST `/api/v1`, CQRS+MediatR у Application, BFF/OAuth, ролей і підписки, статей, медіа, трьох режимів з сервера, прогресу, адмінки, GDPR-видалення.
- Прототип UA→EN можна зберегти за змістом; контракт і `question-card` **дозволено ламати** на етапі 4.

Локальні адреси: API `http://localhost:5166`, HTTPS `https://localhost:7014`. Базовий URL UI захардкоджений у `QuestionsService`.

---

## 3. Функціональні вимоги

1. **Стаття правила.** Опубліковані статті доступні всім, зокрема анонімним. Формат — HTML з WYSIWYG (зображення, таблиці, аудіо). Рендер на UI після санітизації.
2. **Вікторина.** Багато вправ на один урок. Source українською, відповідь англійською. Можна проходити без читання статті. Анонімам — демо-вікторина.
3. **Рівні складності.**
   - Легкий: банк лише потрібних токенів (слова або словосполучення) + вільне поле (гібрид: чіп дописує в поле).
   - Середній: до кожного потрібного токена ще 3 дистрактори (вводить Manager) + той самий гібрид.
   - Складний: без банку слів, лише клавіатура.
4. **Реєстрація та автентифікація.** OAuth, сесія через BFF (cookie, токен не в браузері). Підтвердження email до вікторини не потрібне. GDPR (згода + право на видалення даних).
5. **Особиста статистика.** Безперервна випадкова черга питань. Успіх = % правильних з **останніх 100** відповідей. Окремий прогрес на кожен рівень складності. Streak не робимо.
6. **Ролі та доступ.** Standard / Premium / Manager / Admin. Повний доступ до вправ — Premium-підписка. Standard: лише **перші 10 питань** уроку (пулу вправ).
7. **Адмін-частина.** `/admin` у тому ж Angular. Manager: CRUD усіх уроків і вікторин. Admin: ролі + видача Premium на email реєстрації. Платіжний шлюз — після MVP.
8. **Аналітика MVP.** DAU/WAU, нові реєстрації, % успішних спроб по вправі/уроку, кількість Standard і активних Premium. Без воронки «стаття → вікторина» і без часу на відповідь. Manager бачить агрегати контенту, **не ПІБ учнів**.

Відкритими лишаються хостинг, сховище медіа та кілька уточнень — див. `docs/qa.md` (статус `Відкрите`).

---

## 4. Нефункціональні вимоги

### 4.1 Безпека

- HTTPS поза Development, HSTS. CORS лише на origin UI, з credentials (BFF-cookie). Не `AllowAll` у проді.
- **BFF:** access/refresh не зберігаються в JS. Сесійна cookie: `HttpOnly` + `Secure` + `SameSite`. Refresh **30 днів**. CSRF: antiforgery / SameSite + перевірка origin.
- Вхід через **OAuth** (провайдер — див. відкрите B1 у `qa.md`). Identity лишається store користувача, ролей і зовнішніх логінів.
- Lockout на локальні спроби (якщо з’явиться парольний fallback). Email confirmation **не** блокує вікторину.
- Policy-based authorization на захищених endpoint. Перевірка відповіді **тільки на сервері**. На складному рівні банк слів не віддається. На легкому/середньому — перемішані токени без позначки «правильне».
- Статті: HTML з адмінки **санітизувати** на записі (XSS, javascript: URL, небезпечні теги). Аудіо/зображення — лише з нашого сховища.
- Адмін-дії — audit log. Мінімум **один** Admin; зняти останнього Admin не можна.
- GDPR: згода на обробку, право на видалення персональних даних (акаунт, спроби, прогрес, сесії). Контент уроків не персональний.
- Secrets — User Secrets / env / Key Vault. Rate limit: OAuth callback, logout, submit вікторини.
- Мінімальні права ролі БД у проді (без DDL).

### 4.2 Швидкодія

- Публічні read (каталог, стаття) — p95 < 200 мс після прогріву кешу.
- Видача наступного питання (випадковий weighted pick) і submit — p95 < 150 мс; аналітику не рахувати в цьому запиті.
- Індекси: спроби `(UserId, CreatedAt)`, `(UserId, ExerciseId, Difficulty)`, уроки `IsPublished`, `SortOrder`.
- Кеш: `IMemoryCache`, Redis — коли з’явиться друге API-інстанс. Інвалідація після publish уроку.
- Пагінація списків. Агрегати аналітики — фон / snapshot.
- Angular: lazy `features/admin` і `features/analytics`. UI українською, без i18n-фреймворка в етапі 3.

### 4.3 Надійність і підтримка

- Health: `/health/live`, `/health/ready` (PostgreSQL).
- Структуровані логи + `Correlation-Id`.
- Єдина зміна схеми — EF Core міграції.
- Backup щодня, у проді — PITR.
- API: REST `/api/v1/...`. Ломаючі зміни — нова версія префікса або новий контракт.

---

## 5. Архітектурні рішення

Рішення з `qa.md` (2026-09-21) і CQRS+MediatR (2026-10-06). Зміна пункту = оновлення цього розділу + запис в історії документа.

| Тема | Рішення |
|---|---|
| Модель SaaS | B2C, один продукт, багато користувачів, RBAC. Без tenant / шкіл |
| Мови | Поля `sourceLang` / `targetLang` у контенті. MVP-контент: UA→EN, один напрямок. Інших мов у перший рік немає |
| БД | PostgreSQL, dev — Docker |
| Auth | BFF + cookie-сесія 30 днів, OAuth, Identity як store |
| Статті | WYSIWYG HTML у БД, рендер і санітизація на UI; медіа (зображення, таблиці, аудіо) |
| Перевірка | Нормалізація + ігнор пунктуації + синоніми; апостроф значущий; після помилки — diff; регістр зберігається в канонічній формі |
| Видача питань | Безперервно, випадково, без ліміту спроб. Після **правильної** відповіді знижується ймовірність цього ж питання |
| Прогрес | % успіху з останніх 100 спроб, **окремо** для кожного рівня складності. Перегляд статті не трекаємо. Streak немає |
| Standard | Перші 10 питань пулу вправ уроку; статті без обмежень |
| Premium | Повний пул вправ; сутність підписки (`ExpiresAt`). У MVP видає Admin на email. Білінг — після MVP |
| Аналітика | DAU/WAU, реєстрації, % успіху по вправі/уроку, лічильники Standard і активних Premium. Без воронки та time-on-answer |
| API | REST `/api/v1` |
| Application | **CQRS + MediatR**, одна PostgreSQL. Окремі Command/Query, без другої БД і без event sourcing |
| Доступ до даних | **`ITrainingDbContext` + хендлери**. Generic `I{Name}Repository` не розмножуємо. Вузький доступ (напр. `IQuestionPicker`) — лише якщо запит повторюваний або важкий |
| Адмінка | `/admin` у тому ж Angular, окремий SPA не потрібен |
| Процес | Підтвердження **кожної задачі**, не етапу цілком |

Ще відкриті: провайдер OAuth, хостинг, сховище файлів і ліміти, мердж анонімних спроб, строк зберігання сирих спроб — `qa.md`.

### 5.1 Структура рішення

Clean Architecture. Нові проєкти не додаємо, доки шар не роздувається.

```
Training.API            HTTP REST, BFF-cookie, CORS+CSRF, rate limit, health, OAuth callback
Training.Application    CQRS (Commands/Queries) + MediatR, ITrainingDbContext, контракти, політики
Training.Domain         моделі, enum (без DTO, без I*Service і без I*Repository «на кожну сутність»)
Training.Persistence    TrainingDbContext : ITrainingDbContext, EF Core + PostgreSQL, Identity
Training.UI             Angular 19 standalone (учень + /admin)
docker-compose          PostgreSQL для dev
```

Залежності: `API` → `Application` / `Persistence` → `Domain`.

### 5.1.1 Application: CQRS + MediatR (одна БД)

Логічний CQRS у `Training.Application`. Фізичний CQRS (окрема read-БД, черга подій, eventual consistency) **не робимо**.

- **Query** — читання без зміни стану. Не віддає канонічну відповідь (`GetNextQuestion`, каталог уроків, прогрес, аналітика).
- **Command** — зміна стану. `SubmitAttempt` у **одній транзакції** пише спробу й оновлює прогрес рівня (інакше безперервна вікторина й GDPR-erase ламаються).
- **MediatR** — `IRequest` / `IRequestHandler`, `IMediator.Send` з тонкого контролера. Pipeline: валідація, logging. Нових `I{Name}Service` у Domain не додаємо; поточний `IExerciseService` замінюємо handlers.
- **Один** `TrainingDbContext` (PostgreSQL) за інтерфейсом `ITrainingDbContext`. Query: `AsNoTracking` де можливо. Аналітика пізніше — таблиця snapshot у тій самій БД, не окремий store.
- Структура за фічами, не плоский `Services/`:

```
Training.Application/Features/{Feature}/
  Queries/{Name}/{Name}Query.cs
  Queries/{Name}/{Name}QueryHandler.cs
  Queries/{Name}/{Name}Response.cs
  Commands/{Name}/{Name}Command.cs
  Commands/{Name}/{Name}CommandHandler.cs
```

Приклади фіч: `Lessons`, `Quiz`, `Progress`, `Auth`, `Admin`, `Analytics`, `Privacy`.  
Контролер не містить бізнес-логіки: мапить HTTP → request, request → `IActionResult`.

### 5.1.2 Доступ до даних: `ITrainingDbContext`, не шар репозиторіїв

EF Core уже є Unit of Work і змінює кілька сутностей в одній транзакції. Другий шар `IRepository<T>` поверх `DbSet<T>` для цього продукту не потрібен.

- Хендлер залежить від **`ITrainingDbContext`** (у Application). `TrainingDbContext` у Persistence **реалізує** цей інтерфейс. Application може посилатися на EF Core заради `DbSet` / `AsNoTracking` — прийнятий компроміс.
- Domain містить лише моделі та enum. **Не** додавати `I{Name}Repository` як правило. Поточний `IExerciseRepository` — борг прототипу (in-memory), прибрати після підключення EF.
- Generic CRUD-репозиторій заборонений як стандарт.
- Вузька абстракція **за змістом, не `*Repository`** (`IQuestionPicker`, `IProgressReader`) — лише якщо той самий важкий запит потрібен двом хендлерам або LINQ у хендлері стає нечитабельним. Кандидат: weighted random + ліміт 10 без канону.
- Звичайні сценарії (стаття, submit + прогрес, адмін CRUD, GDPR erase, аналітика) — LINQ у хендлері через `ITrainingDbContext` і один `SaveChangesAsync`.

### 5.2 Доменна модель (цільова)

- `User` — профіль + Identity + зовнішні логіни OAuth. Email — ключ для видачі Premium.
- `Role` — `Standard`, `Premium`, `Manager`, `Admin` (права на матеріали та адмінку).
- `Subscription` — `UserId`, `ExpiresAt`, хто видав, активна якщо `ExpiresAt > UtcNow`. Визначає повний доступ до вправ.
- `Lesson` — назва, **HTML** статті, `sourceLang`, `targetLang`, `IsPublished`, порядок. Розширюємо існуючу модель, не перейменовуємо.
- `Exercise` — `LessonId`, source-текст, канонічна відповідь, **синоніми**, токени (слово або словосполучення), дистрактори (3 на токен, вводить Manager), дозволені режими.
- `MediaAsset` — файл статті (image/audio), метадані; фізичне сховище — після відповіді C4.
- `QuizAttempt` — користувач (nullable для демо), вправа, рівень, відповідь, результат, час.
- `UserLessonProgress` — агрегат по уроку **і** рівню: % з останніх 100 спроб цього рівня.
- `AuditEntry` — актор, дія, сутність, час.
- `AnalyticsSnapshot` — DAU/WAU, реєстрації, % успіху, лічильники тарифів.

### 5.3 Ролі та доступ (матриця MVP)

| Можливість | Анонім | Standard | Premium (активна підписка) | Manager | Admin |
|---|---|---|---|---|---|
| Читати опубліковані статті | так | так | так | так | так |
| Демо-вікторина | так | — | — | — | — |
| Вікторина уроку, перші 10 питань, усі 3 рівні | ні | так | так | так | так |
| Вікторина уроку, повний пул | ні | ні | так | так | так |
| Особиста статистика | ні | так | так | так | так |
| CRUD усіх уроків і вправ | ні | ні | ні | так | так |
| Бачити ПІБ учнів | ні | ні | ні | ні | так (керування акаунтами) |
| Видати/продовжити Premium на email | ні | ні | ні | ні | так |
| Призначити ролі, мінімум 1 Admin | ні | ні | ні | ні | так |
| Аналітика контенту / продукту | ні | ні | ні | агрегати без ПІБ | повна + ПІБ у списку користувачів |

Manager бачить **усі** уроки. Ліміт Standard — кількість питань пулу, не рівень складності.

### 5.4 API (REST `/api/v1`)

Поточні `Exercise/GetQuestion` і `Exercise/QuestionResponse` замінюємо на етапах 1–4 (узгоджено: ламати прототип можна).

| Група | Приклади | Доступ |
|---|---|---|
| Auth | `POST /api/v1/auth/oauth/{provider}`, callback, `POST /api/v1/auth/logout`, `GET /api/v1/auth/me` | анонім / власник сесії |
| Lessons | `GET /api/v1/lessons`, `GET /api/v1/lessons/{id}` | публічно (опубліковані) |
| Exercises | `GET /api/v1/lessons/{id}/questions/next?difficulty=`, `POST /api/v1/exercises/{id}/attempts` | демо / Standard / Premium за правилами |
| Progress | `GET /api/v1/me/progress` | власник |
| Privacy | `DELETE /api/v1/me` | власник (GDPR erase) |
| Admin | `POST/PUT /api/v1/admin/lessons`, exercises, media | Manager/Admin |
| AdminUsers | `GET /api/v1/admin/users`, `POST /api/v1/admin/users/{id}/roles`, `POST /api/v1/admin/subscriptions` | Admin |
| Analytics | `GET /api/v1/analytics/overview` | Manager (без ПІБ) / Admin |
| Health | `/health/live`, `/health/ready` | інфра |

Контракти — `record` поруч із Query/Command фічі (за потреби спільні типи в `Training.Application/Contracts`). Без Domain-сутностей у відповідях. Контролер викликає `IMediator.Send`, не Application-сервіс.

### 5.5 UI

- `common-ui/` — стаття (безпечний HTML), картка вікторини, чіпи, diff відповіді.
- `data/services/`, `data/interfaces/`.
- `features/auth`, `features/learn`, `features/progress`, `features/admin`, `features/analytics`.
- Адмінка: lazy route `/admin`, той самий додаток.
- `withCredentials` для BFF. UI українською.
- Легкий/середній: гібрид (чіп → поле). Складний: лише поле.
- Після відповіді одразу наступне випадкове питання, без повернення на статтю.

### 5.6 Рушій вікторини

1. Сервер обирає наступну вправу з пулу уроку (для Standard — лише перші 10 за стабільним порядком `SortOrder`/`Id`). Випадково, **безперервно**. Вага питання падає після його правильної відповіді цим користувачем.
2. Відповідь клієнта: текст (і `difficulty`). Не «id правильних чіпів».
3. Нормалізація: trim, стиснення пробілів, **видалення пунктуації**; апостроф у скороченнях зберігається. Порівняння з каноном **і синонімами**. Канонічний рядок зберігає регістр для показу.
4. Легкий/середній: токени можуть бути словосполученнями; дистрактори — 3 на токен від Manager, віддаються перемішаними.
5. Command `SubmitAttempt` пише `QuizAttempt` і в тій же транзакції перераховує % з останніх 100 спроб цього користувача / уроку / рівня. Аналітичний snapshot у цьому запиті не рахуємо.
6. У відповіді command: статус, канон **після** спроби, **diff** якщо помилка. Query `GetNextQuestion` канон ніколи не віддає.

Анонім: лише зафіксована демо-вікторина. Мердж демо-спроб після реєстрації — ще не узгоджено (B4).

---

## 6. Технології та стандарти

| Шар | Стек | Стандарт |
|---|---|---|
| Backend | .NET 9, ASP.NET Core | nullable, тонкі контролери, REST `/api/v1` |
| Application | **MediatR** + логічний CQRS | Command/Query handlers; одна БД; без event sourcing |
| Дані | EF Core + **PostgreSQL**, міграції | `ITrainingDbContext` + один `TrainingDbContext`; Fluent API, індекси |
| Dev infra | Docker Compose (PostgreSQL) | рядок підключення в secrets |
| Identity | ASP.NET Core Identity + OAuth + BFF cookie | ролі + policies + `Subscription` |
| API docs | Swagger | Development / внутрішній контур |
| Frontend | Angular 19, RxJS 7, TypeScript 5.7 | standalone; UI укр.; без i18n у етапі 3 |
| Статті | WYSIWYG (адмінка) + sanitize на UI | HTML, не Markdown |
| Стиль C# | `.cursor/rules/backend.mdc` | MediatR handlers + `ITrainingDbContext`; без generic-репозиторіїв |
| Стиль UI | `.cursor/rules/frontend.mdc` | HTTP у сервісах, типи в `data/` |
| Процес | `.cursor/rules/general.mdc` | документація → підтвердження **задачі** → код |
| Час | UTC у БД | |
| JSON | camelCase, enum рядком | |

Планові пакети (не ставити до підтвердження задачі): `MediatR`, `Npgsql.EntityFrameworkCore.PostgreSQL`, Identity, auth cookie/BFF, OAuth providers, rate limiting.

Хостинг і blob — після C2/C4.

---

## 7. Етапи розробки

Старт **кожної задачі** — лише після підтвердження. Статуси: `docs/tasktracker.md`.

| Етап | Назва | Результат |
|---|---|---|
| 0 | Документація | Вимоги та рішення зафіксовані |
| 1 | Фундамент даних | PostgreSQL (Docker), EF Core, каркас CQRS+MediatR, `Lesson`/`Exercise` у БД |
| 2 | Безпека доступу | BFF + OAuth, ролі, підписка, CORS/CSRF, GDPR erase |
| 3 | Контент учня | Публічний каталог і стаття (HTML + медіа) |
| 4 | Рушій вікторини | REST, 3 режими, гібрид/клавіатура, weighted random, ліміт 10, демо, diff |
| 5 | Прогрес | Спроби, % з 100, окремо по рівнях |
| 6 | Адмінка | `/admin` CRUD, WYSIWYG, дистрактори, видача Premium, audit |
| 7 | Аналітика | DAU/WAU, реєстрації, % успіху, лічильники тарифів |
| 8 | Hardening | health, кеш, correlation id, бекапи |

Етап 0 (пакет + перенос відповідей qa) — виконано.

---

## 8. Консистентність і підтримка

1. Документація перша: `Project.md` → `tasktracker.md` → код → `changelog.md`.
2. Не перейменовувати проєкти, namespace і папки шарів.
3. Мінімальний диф; підтверджуємо **задачу**, не весь етап.
4. Нові API — REST `/api/v1`; UI-контракти в тому ж кроці.
5. `AddApplication()` реєструє MediatR. `AddPersistence()` реєструє `ITrainingDbContext` → `TrainingDbContext` (`AddScoped`). Нові сценарії — Command/Query через контекст, не `I{Name}Service` і не `I{Name}Repository` у Domain.
6. Банк слів і перевірка — лише сервер (command); клієнтські `anagrams` — борг до етапу 4.
7. Документація українською, ідентифікатори коду англійською; UI — українською.
8. Не стартувати задачу, що залежить від ще відкритого питання в `qa.md`.
9. Інциденти: `Correlation-Id`, health, `IsPublished` без деплою. Видалення акаунта не чіпає чужі уроки.

---

## 9. Критерії готовності MVP

- OAuth + BFF-сесія; статті публічні; демо-вікторина без акаунта.
- Зареєстрований Standard проходить 10 питань уроку на будь-якому рівні; Premium (ручна підписка) — увесь пул.
- Випадкова безперервна черга; після успіху питання випадає рідше; після помилки є diff.
- Прогрес: % з останніх 100, окремо по рівнях.
- Manager редагує всі уроки (WYSIWYG + медіа + дистрактори). Admin видає Premium на email і не може зняти останнього Admin.
- Аналітика без ПІБ для Manager; GDPR-видалення персональних даних.
- REST `/api/v1`, PostgreSQL, CQRS+MediatR і `ITrainingDbContext` (одна БД, без шару репозиторіїв), без правильної відповіді в GET next.
- Документація синхронна з кодом.

---

## 10. Як оновлювати цей файл

1. Коротко опиши зміну в «Історія документа».
2. Виправ розділ (вимоги, рішення, модель, етап).
3. Онови статуси / нові питання в `qa.md`.
4. Додай або скоригуй задачі в `tasktracker.md`.
5. Запис у `changelog.md`.

### Історія документа

| Дата | Зміна |
|---|---|
| 2026-09-18 | Перша версія: рамка SaaS і 8 вимог |
| 2026-09-21 | Перенесено відповіді з `qa.md`: UA→EN, мово-агностична модель, PostgreSQL/Docker, BFF+OAuth, WYSIWYG+медіа, REST, підписка, ліміт 10, прогрес по 100 і рівнях, weighted random, GDPR, `/admin` |
| 2026-10-06 | Application: логічний CQRS + MediatR, одна PostgreSQL; без окремої read-БД і event sourcing |
| 2026-10-06 | Доступ до даних: `ITrainingDbContext` + хендлери; `I{Name}Repository` не є правилом, лише вузькі абстракції за змістом |
