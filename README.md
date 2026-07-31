# EventManager API

EventManager - учебный ASP.NET Core Web API для управления событиями и бронированиями.

Проект разделён на три микросервиса. Каждый сервис построен по слоям `Domain`, `Application`, `Infrastructure` и `Web`, владеет собственной PostgreSQL-базой данных и взаимодействует с другими сервисами через Kafka.

## Структура проекта

```text
Common/
├── EventManager.Common.Core
└── EventManager.Common.AspNetCore
UserService/
├── UserService.Domain
├── UserService.Application
├── UserService.Infrastructure
└── UserService.Web
EventService/
├── EventService.Domain
├── EventService.Application
├── EventService.Infrastructure
└── EventService.Web
BookingService/
├── BookingService.Domain
├── BookingService.Application
├── BookingService.Infrastructure
└── BookingService.Web
```

## Состав системы

| Сервис | Ответственность | База данных | Порт БД | HTTP / HTTPS |
|---|---|---|---|---|
| `UserService` | Регистрация, вход и выпуск JWT | `users` | `5432` | `5067` / `7113` |
| `EventService` | Управление событиями и учёт доступных мест | `events` | `5433` | `5068` / `7114` |
| `BookingService` | Создание, отмена и фоновая обработка бронирований | `bookings` | `5434` | `5069` / `7115` |

Общие контракты Kafka, JWT-настройки и ASP.NET Core middleware вынесены в каталог `Common`. Для каждого сервиса есть собственные unit- и integration-тесты.

## API
### EventService
- GET /events — получить список событий
- GET /events/{id} — получить событие по id
- POST /events — создать событие
- PUT /events/{id} — обновить событие
- DELETE /events/{id} — удалить событие

### BookingService
- POST /bookings/book — создать бронирование события (асинхронная обработка)
- GET /bookings/{id} — получить бронирование по id
- DELETE /bookings/{id} — отмена бронирования по id

### UserService
POST /Auth/register - регистрация пользователя
POST /Auth/login - авторизация пользователя

### GET /events - фильтрация и пагинация
Поддерживаемые query-параметры:
- `title` (string, optional) — фильтр по заголовку (регистр не важен, поиск по подстроке)
- `from` (date, optional, `yyyy-MM-dd`) — начальная дата (включительно)
- `to` (date, optional, `yyyy-MM-dd`) — конечная дата (включительно)
- `page` (int, optional, default: `1`) — номер страницы, начиная с 1
- `pageSize` (int, optional, default: `10`) — количество элементов на странице

Ограничения валидации:
- `page >= 1`
- `pageSize >= 1`

Примеры запросов:
- `GET /events?title=meetup`
- `GET /events?from=2026-01-01&to=2026-01-31`
- `GET /events?title=workshop&page=2&pageSize=5`

### POST /bookings/book
Создаёт бронирование и помещает его в очередь фоновой обработки `BookingService`.

- При успешном принятии заявки возвращается `202 Accepted`;
- в ответе содержится `Booking` со статусом `Pending`;
- итоговый статус можно получить через `GET /bookings/{id}`.

### GET /bookings/{id}
Возвращает текущее состояние бронирования.

- Если бронирование не найдено: `404 Not Found`
- При успехе: `200 OK`

### JWT-аутентификация

API использует JWT Bearer-аутентификацию. Эндпоинты `POST /Auth/register`, `POST /Auth/login`, `GET /events`, `GET /events/{id}` доступны без токена; все остальные API-эндпоинты требуют авторизации.

### Роли и права доступа

В системе есть две роли: `User` и `Admin`. Роль включается в JWT при входе и применяется при обработке каждого защищённого запроса.

| Операция | User | Admin |
|---|---|---|
| Просмотр событий (`GET /Events`, `GET /Events/{id}`) | Да | Да |
| Бронирование события (`POST /Bookings/book`) | Да | Да |
| Создание, изменение и удаление событий | Нет | Да |
| Просмотр и отмена собственного бронирования | Да | Да |
| Просмотр и отмена чужого бронирования | Нет | Да |

При отсутствии или недействительности токена API возвращает `401 Unauthorized`; при попытке выполнить операцию, запрещённую ролью, — `403 Forbidden`.

## Error response format
При ошибках API возвращает объект формата:

{
    ProblemDetails ErrorDetails,
    DateTime Created,
    string Message
}


Примеры ответов с ошибками:

`400 Bad Request` (некорректный `page`):
```json
{
  "errorDetails": {
    "title": "One or more validation errors occurred.",
    "status": 400,
    "detail": "$: JSON deserialization for type 'EventManager.Models.EventDto' was missing required properties including: 'title'., eventDto: The eventDto field is required."
  },
  "created": "2026-04-16T08:17:19.7257238Z",
  "message": "Validation issues. See Error Details"
}
```

`404 Not Found` (событие не найдено):
```json
{
  "errorDetails": {
    "status": 404,
    "detail": "Event 3fa85f64-5717-4562-b3fc-2c963f66afa6 is not found"
  },
  "created": "2026-04-16T08:23:09.1735054Z",
  "message": "Exception occured. See Error Details"
}
```
## Event model
`Event` описывает событие и содержит:

- `id` (`Guid`) — идентификатор события
- `Title` (`string`) — заголовок события
- `Description` (`string?`) — описание события (не обязательно)
- `StartAt` (`DateTime`) — дата/время начала события (UTC)
- `EndAt` (`DateTime`) — время завершения события (UTC)
- `TotalSeats` (`int`) — общее количество мест для бронирования
- `AvailableSeats` (`int`) — количество свободных мест для бронирования

## Booking model
`Booking` описывает бронирование события и содержит:

- `id` (`Guid`) — идентификатор бронирования
- `eventId` (`Guid`) — идентификатор события
- `status` (`BookingStatus`) — текущий статус обработки
- `createdAt` (`DateTime`) — время создания бронирования (UTC)
- `processedAt` (`DateTime?`) — время завершения обработки (UTC), `null` пока не обработано
- `bookedSeatsAmount` (`int`) — количество мест для брони

Статусы `BookingStatus`:

- `Pending` — заявка создана и ожидает обработки
- `Confirmed` — заявка успешно подтверждена фоновой обработкой
- `Rejected` — заявка отклонена 

## Логика фоновой обработки бронирований
После `POST /bookings/book` бронирование не подтверждается мгновенно. `BookingService` сохраняет заявку, помещает её в in-memory очередь, а `BookingBackgroundService` обрабатывает её в фоне: переводит в `Confirmed` либо `Rejected` и создаёт сообщение в outbox. Итоговый статус нужно проверять через `GET /bookings/{id}`.

## Обмен событиями через Kafka

`BookingService` публикует события из таблицы outbox, а `EventService` подписан на оба топика в consumer group `eventsGroup`. `EventService` использует таблицу inbox для учёта полученных сообщений.

### `BookingConfirmedMsg`

Топик: `booking-confirmed`.

1. После фоновой обработки `BookingService` переводит бронь в `Confirmed` и добавляет `BookingConfirmedMsg` в outbox.
2. `OutboxPublisherService` публикует сообщение в Kafka с ключом `EventId`.
3. `BookingConsumerService` в `EventService` получает сообщение и передаёт его `BookingConfirmedMsgHandler`.
4. Обработчик загружает событие и резервирует указанное число мест. Если событие уже началось или мест недостаточно, доступное число мест не изменяется, а ошибка записывается в лог.
5. После обработки consumer коммитит Kafka offset.

### `BookingCancelledMsg`

Топик: `booking-cancelled`.

1. При отмене или отклонении `BookingService` добавляет `BookingCancelledMsg` в outbox.
2. `OutboxPublisherService` публикует его в Kafka с ключом `EventId`.
3. `BookingConsumerService` передаёт сообщение в `BookingCancelledMsgHandler`.
4. Обработчик освобождает указанное число мест; значение `AvailableSeats` не может превысить `TotalSeats`.
5. После обработки consumer коммитит Kafka offset.

## Примитивы синхронизации
В проекте используются примитивы синхронизации, чтобы безопасно обрабатывать параллельные запросы на бронирование:

1. `Channel<T>` (`InMemoryTaskQueue`)  
   Используется как потокобезопасная очередь задач между API и фоновым обработчиком.
   - API добавляет `Booking` в очередь (`EnqueueAsync`)
   - `BookingBackgroundService` читает заявки через `ReadAllAsync`
   Это разделяет быстрый HTTP-ответ (`202 Accepted`) и более долгую обработку.

2. `SemaphoreSlim` + `ConcurrentDictionary<Guid, SemaphoreSlim>` (`EventBookingLockProvider`)  
   Используется для блокировки по `eventId`:
   - для одного и того же события одновременно выполняется только одна операция создания бронирования в экземпляре `BookingService`
   - для разных событий операции могут выполняться параллельно
   Это сериализует обработку одновременных запросов внутри экземпляра `BookingService`.

## Пример использования сценария бронирований
1. Создать событие:
   - `POST /events`
2. Получить `id` созданного события из ответа
3. Создать бронирование:
   - `POST /bookings/book`
4. Получить `bookingId` из ответа (`status = Pending`)
5. Через несколько секунд проверить статус:
   - `GET /bookings/{bookingId}`
6. Ожидаемый результат после обработки:
   - `status = Confirmed`, `processedAt != null`

## Пример сценария с овербукингом
Допустим, есть событие с `TotalSeats = 1`.

1. Клиент A отправляет `POST /bookings/book`
   - получает `202 Accepted`, бронирование `A` со статусом `Pending`
2. Клиент B также отправляет `POST /bookings/book` и получает `202 Accepted`.
3. После фоновой обработки `BookingService` публикует для обеих заявок `BookingConfirmedMsg`.
4. `EventService` последовательно обрабатывает сообщения с одним `EventId`: для первой заявки уменьшает `AvailableSeats` до `0`, а для второй не резервирует место и записывает ошибку в лог.

Итог: проверка доступности мест выполняется асинхронно в `EventService`, а HTTP-запрос на создание бронирования всегда возвращает `202 Accepted` после принятия заявки.

## Run
### Требования
- .NET SDK 9.0
- Docker Desktop или Docker Engine с Docker Compose
- свободные порты `5432`–`5434`, `9092`, `5067`–`5069` и `7113`–`7115`

### Первый запуск после клонирования

```bash
git clone <repository-url>
cd practicum

dotnet restore Common/EventManager.Common.sln
dotnet restore UserService/UserService.sln
dotnet restore EventService/EventService.sln
dotnet restore BookingService/BookingService.sln
```

При необходимости убедитесь, что проекты собираются:

```bash
dotnet build UserService/UserService.sln --no-restore
dotnet build EventService/EventService.sln --no-restore
dotnet build BookingService/BookingService.sln --no-restore
```

Затем запустите инфраструктуру и сервисы по инструкциям ниже. `dotnet restore` необходим, потому что после чистого клона в репозитории нет восстановленных NuGet-пакетов и артефактов сборки.

### Настройка JWT-секрета

Параметры JWT находятся в разделе `Jwt` файлов `UserService/UserService.Web/appsettings.json`, `EventService/EventService.Web/appsettings.json` и `BookingService/BookingService.Web/appsettings.json`. Помимо `Issuer`, `Audience` и `JwtTokenStoreMinutes` необходимо задать секрет подписи:

```json
{
  "Jwt": {
    "Issuer": "EventManagerApi",
    "Audience": "EventManagerApi",
    "JwtTokenStoreMinutes": 60,
    "Secret": "local-development-secret-change-me"
  }
}
```
В production не храните секрет в репозитории и не используйте пример выше. Передавайте его через переменную окружения `Jwt__Secret` или безопасное хранилище секретов. Используйте уникальное криптографически случайное значение длиной не менее 32 байт и регулярно меняйте его при необходимости.

## Актуальная структура секретов для JWT

В текущем локальном проекте используется механизм **User Secrets** для безопасного хранения конфиденциальных данных на локальной машине разработчика. 

Секреты привязаны к проекту через идентификатор в <UserSecretsId> в настройках проекта.
Они хранятся вне папки проекта, поэтому **никогда не попадут в Git**.

Для корректной работы аутентификации на локальной машине при необходимости переопределите настройку JWT `Secret`:
### Вариант 1: Через терминал (.NET CLI)

```bash
dotnet user-secrets set "Jwt:Secret" "ВАШ_СУПЕР_СЕКРЕТНЫЙ_КЛЮЧ_ДЛИНОЙ_ОТ_32_СИМВОЛОВ"
```
*Полезные команды CLI:*
* **Просмотреть все секреты:** `dotnet user-secrets list`
* **Очистить все секреты:** `dotnet user-secrets clear`


### Вариант 2: Через Visual Studio (Windows / Mac)

1. В окне **Solution Explorer** нажмите правой кнопкой мыши по проекту.
2. Выберите **Manage User Secrets** (Управление секретами).
3. В открывшийся файл `secrets.json` вставьте блок JSON из раздела «Актуальная структура секретов для JWT» выше и сохраните файл.
## Где физически хранятся эти файлы?
Если вам нужно найти файл `secrets.json` вручную на диске:

* **Windows:** `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
* **Linux / macOS:** `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

### Запуск всего стенда в Docker
Из корня репозитория соберите образы и запустите Kafka, три PostgreSQL-базы и три API:

```bash
docker compose up --build -d
docker compose ps
```

Kafka доступна на порту `9092`; базы `users`, `events` и `bookings` — на портах `5432`, `5433` и `5434`. API будут доступны по адресам:

- `http://localhost:5067` — `UserService`;
- `http://localhost:5068` — `EventService`;
- `http://localhost:5069` — `BookingService`.

Для просмотра логов конкретного сервиса используйте, например:

```bash
docker compose logs -f booking-service
```

Остановить стенд можно командой:

```bash
docker compose down
```

### Запуск сервисов через `dotnet run`
Этот вариант подходит для отладки API на хосте. Сначала поднимите только Kafka и базы данных:

```bash
docker compose up -d zookeeper kafka users-db events-db bookings-db
```

Откройте три терминала в корне репозитория и выполните:

```bash
dotnet run --project UserService/UserService.Web --launch-profile http
```

```bash
dotnet run --project EventService/EventService.Web --launch-profile http
```

```bash
dotnet run --project BookingService/BookingService.Web --launch-profile http
```

Миграции EF Core применяются автоматически при запуске каждого Web-проекта. Все три сервиса должны использовать одинаковые значения `Jwt:Issuer`, `Jwt:Audience` и `Jwt:Secret`.

### Схема БД и миграции EF Core
Схема базы данных управляется миграциями EF Core. Файлы миграций находятся в инфрастуктурном слое сервисов в папке `/Migrations`.

При запуске API неопубликованные миграции применяются автоматически (`db.Database.Migrate()` в `Program.cs`). 

Создать новую миграцию после изменения моделей:

```bash
dotnet ef migrations add <MigrationName> -p <ServiceName>.Infrastructure -s <ServiceName>.Web
```

## Run tests
Для запуска unit-тестов:

```bash
dotnet test BookingService/BookingService.UnitTests/BookingService.UnitTests.csproj
dotnet test EventService/EventService.UnitTests/EventService.UnitTests.csproj
dotnet test UserService/UserService.UnitTests/UserService.UnitTests.csproj
```

Интеграционные тесты Booking и Event Service используют Testcontainers и требуют доступного Docker daemon.

## Swagger
`http://localhost:5067/swagger`, `http://localhost:5068/swagger`, `http://localhost:5069/swagger`

Получить и использовать JWT токен через Swagger можно так:

1. Откройте Swagger по адресу выше и выполните `POST /Auth/register`, если пользователя ещё нет. В теле запроса передайте `login`, `password` и при необходимости `role`; роль по умолчанию — `User`.
2. Выполните `POST /Auth/login` с теми же `login` и `password`.
3. Скопируйте значение поля `data` из успешного ответа — это JWT-токен.
4. Нажмите **Authorize** в Swagger, вставьте только скопированный токен в поле Bearer и подтвердите. Swagger добавит заголовок `Authorization: Bearer <token>` к защищённым запросам.
