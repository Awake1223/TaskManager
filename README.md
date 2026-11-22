# TaskManager

Менеджер задач: проекты, задачи, пользователи.

**Backend:** ASP.NET Core 9 + EF Core + PostgreSQL + Identity + JWT.  
**Frontend:** Angular 20 (`NgModule`, `standalone: false`).  
**Инфраструктура:** Docker Compose.

---

## Стек технологий

| Слой | Технологии |
|---|---|
| Backend | C# / .NET 9, ASP.NET Core, EF Core 9, ASP.NET Identity, JWT |
| Хранилище | PostgreSQL 16 |
| Frontend | Angular 20, RxJS, Reactive Forms |
| Инфраструктура | Docker, Docker Compose |

---

## Архитектура

```text
Domain → Application → Infrastructure → API
                                          ↑
                                    Angular (client)
```

- **Domain** — сущности (`UserModel`, `ProjectModel`, `TaskItemModel`), enum'ы.
- **Application** — DTO, интерфейсы сервисов и репозиториев, бизнес-логика.
- **Infrastructure** — `TaskManagerDbContext`, EF-конфигурации, репозитории, JWT-генератор, Identity.
- **API** — контроллеры, DI, middleware, `Program.cs`.
- **Angular** — SPA с вкладками «Проекты / Задачи / Пользователи» + логин/регистрация.

---

## Требования

- **Docker Desktop** (Windows/macOS/Linux) с включённым WSL2 (Windows).
- Свободные порты на хосте:
  - `8081` — Angular.
  - `5041` — API.
  - `5433` — PostgreSQL (снаружи).

Для локальной разработки без Docker (опционально):

- .NET 9 SDK.
- Node.js 22+.
- Angular CLI 20.

---

## Быстрый запуск (Docker)

Из корня репозитория (где находится `docker-compose.yml`) выполните:

```bash
docker compose up -d --build
```

### Что произойдёт

1. Соберётся образ `api` (ASP.NET Core).
2. Соберётся образ `client` (Angular → nginx).
3. Поднимется `postgres:16` с БД `taskmanager`.
4. `api` дождётся готовности PostgreSQL, применит EF-миграции и запустится.
5. `client` (nginx) раздаст собранный Angular и будет проксировать `/api/*` в `api:8080`.

### Ожидаемый вывод

```text
[+] Running 3/3
 ✔ Container taskmanager-postgres  Healthy
 ✔ Container taskmanager-api       Started
 ✔ Container taskmanager-client    Started
```

### Проверка состояния контейнеров

```bash
docker compose ps
```

Все три контейнера должны иметь статус `Up` / `Up (healthy)`.

### Доступы после запуска

| Сервис | URL / Подключение |
|---|---|
| Angular (UI) | http://localhost:8081 |
| Swagger (API) | http://localhost:5041/swagger |
| PostgreSQL (снаружи) | `localhost:5433` |
| Пользователь PostgreSQL | `postgres` |
| Пароль PostgreSQL | `postgres` |
| База данных | `taskmanager` |

### Остановка

```bash
# Остановить контейнеры, сохранив данные
docker compose down

# Остановить контейнеры и удалить volume с данными
docker compose down -v
```

---

## Использование

Откройте [http://localhost:8081](http://localhost:8081).

После открытия произойдёт автоматический редирект на `/login`.

### Регистрация и авторизация

1. Нажмите «Зарегистрироваться».
2. Заполните форму регистрации.
3. После регистрации JWT-токен сохранится в `localStorage`.
4. Вы автоматически попадёте на вкладку «Проекты».

### Вкладка «Проекты»

- **«+ Проект»** — создание проекта (имя обязательно, описание опционально).
- **«Изменить» / «Удалить»** — редактирование и удаление проекта.
- Поиск по имени.
- Пагинация.

### Вкладка «Задачи»

- **«+ Задача»** — создание задачи:
  - имя;
  - описание;
  - проект (обязательно);
  - исполнитель (опционально).
- **«Подробнее»** — просмотр карточки задачи.
- **«Изменить»** — изменение имени, описания и исполнителя (проект у существующей задачи не меняется).
- **Смена статуса** — выпадающий список непосредственно в таблице:
  - `New`;
  - `InProgress`;
  - `OnHold`;
  - `Done`;
  - `Cancelled`.
- Поиск по имени.
- Пагинация.
- Удаление задач.

### Вкладка «Пользователи»

- **«+ Пользователь»** — создание нового пользователя:
  - email;
  - пароль;
  - имя;
  - фамилия.
- **«Удалить»** — Soft Delete: пользователь помечается `IsDeleted = true` и пропадает из списка, но физически из БД не удаляется.
- Поиск по имени/email.
- Пагинация.

### Выход из аккаунта

Кнопка **«Выйти»** в шапке приложения очищает `localStorage`.

---

## Локальная разработка (без Docker)

### Backend (API)

Для работы требуется локальный PostgreSQL на `localhost:5432`.

При необходимости можно переключить строку подключения на Docker PostgreSQL (`localhost:5433`).

В `TaskManager.API/appsettings.json` измените `DefaultConnection`.

Запуск API:

```bash
dotnet run --project TaskManager.API
```

API поднимется на:

```text
https://localhost:7160
```

Порт берётся из `launchSettings.json`.

EF-миграции применяются автоматически.

Swagger доступен по адресу:

```text
https://localhost:7160/swagger
```

### Frontend (Angular)

Перейдите в папку Angular-проекта:

```bash
cd TaskManager.Angular
```

Установите зависимости:

```bash
npm install
```

Запустите приложение:

```bash
npm start
```

UI откроется по адресу:

```text
http://localhost:61680
```

`environment.ts` уже настроен на `http://localhost:5041` (Docker API).

Если запускаете API локально, измените `apiUrl` на:

```text
https://localhost:7160
```

CORS на API для `http://localhost:61680` включён.

---

## Структура репозитория

```text
.
├── TaskManager.Domain/          # Сущности, enum'ы
├── TaskManager.Application/     # DTO, интерфейсы, сервисы
├── TaskManager.Infrastructure/  # DbContext, EF-конфиги, репозитории, JWT
├── TaskManager.API/             # Контроллеры, Program.cs, Dockerfile
├── TaskManager.Angular/         # Angular-клиент, Dockerfile, nginx.conf
├── docker-compose.yml
└── README.md
```

---

## API

### Авторизация

| Метод | Endpoint | Описание |
|---|---|---|
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Авторизация (JWT) |

### Проекты

| Метод | Endpoint | Описание |
|---|---|---|
| GET | `/api/projects?page=1&pageSize=20&search=` | Получить список проектов |
| GET | `/api/projects/{id}` | Получить проект по ID |
| POST | `/api/projects` | Создать проект |
| PUT | `/api/projects/{id}` | Обновить проект |
| DELETE | `/api/projects/{id}` | Удалить проект |

### Задачи

| Метод | Endpoint | Описание |
|---|---|---|
| GET | `/api/tasks?page=1&pageSize=20&search=` | Получить список задач |
| GET | `/api/tasks/{id}` | Получить задачу по ID |
| POST | `/api/tasks` | Создать задачу |
| PUT | `/api/tasks/{id}` | Обновить задачу |
| PATCH | `/api/tasks/{id}/status` | Изменить статус задачи |
| DELETE | `/api/tasks/{id}` | Удалить задачу |

Пример тела запроса для изменения статуса:

```json
{
  "status": "InProgress"
}
```

### Пользователи

| Метод | Endpoint | Описание |
|---|---|---|
| GET | `/api/users?page=1&pageSize=20&search=` | Получить список пользователей |
| POST | `/api/users` | Создать пользователя |
| POST | `/api/users/{id}/delete` | Soft Delete (`IsDeleted = true`) |

### Авторизация запросов

Все API-эндпоинты, кроме `/api/auth/*`, требуют JWT-токен.

Передавайте токен в HTTP-заголовке:

```http
Authorization: Bearer <token>
```
