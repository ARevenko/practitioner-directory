# Practitioner Directory

Тестовое задание: бэкенд справочника врачей.

## Стек

- .NET 8, ASP.NET Core Web API, EF Core 8 (Code First, миграции)
- MS SQL Server 2022
- Kafka (в докере это Redpanda)
- Hl7.Fhir.R5 (Firely SDK)
- Docker, docker-compose

## Запуск

Нужен только Docker Desktop. Из корня решения (где лежит docker-compose.yml):

```bash
# собирает образы и поднимает все сервисы (БД, брокер, UI, API, консьюмер)
docker compose up --build
```

При первом запуске:

- поднимается MS SQL, накатываются миграции, база наполняется 10 тестовыми докторами;
- поднимается Redpanda и Kafka UI;
- стартует API и консьюмер топика.

URLs доступа к ресурсам после запуска докера:

- API + Swagger: http://localhost:5000/swagger
- Kafka UI: http://localhost:8080

Остановка:

```bash
docker compose down        # остановить всё
docker compose down -v     # остановить и удалить данные БД (чистый старт с нуля)
```

Получить статус/логи:

```bash
docker compose ps                      # статус контейнеров
docker compose logs api --tail 50      # логи API
docker compose logs worker --tail 50   # логи консьюмера
```

## Проверка консьюмера Kafka

Консьюмер слушает топик `practitioners-import` и добавляет/изменяет доктора в базе.

Создание топика:

```bash
docker compose exec redpanda rpk topic create practitioners-import -p 1 -r 1
```

Пример создания тестового сообщения в топике:

```json
{"Id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","FirstName":"Тест","LastName":"Кафков","Speciality":"Терапевт","Email":"t.kafkov@clinic.example","IsActive":true}
```

Проверяем, что врач появился в базе: GET http://localhost:5000/practitioners/3fa85f64-5717-4562-b3fc-2c963f66afa6 (или через Swagger).

Если отправить то же сообщение повторно:
 - дубликат не создастся
 - запись просто обновится.

## API

Обычный REST:

| Метод | Роут | Что делает |
|---|---|---|
| GET | /practitioners?page=1&pageSize=50 | список с пагинацией |
| GET | /practitioners/{id} | врач по Id |
| GET | /practitioners/search?firstName=&lastName=&resultSize=20 | поиск по имени/фамилии (по части строки) |
| POST | /practitioners | создать |
| PUT | /practitioners | обновить |
| DELETE | /practitioners/{id} | удалить (мягкое удаление) |

FHIR R5 (Content-Type: application/fhir+json):

| Метод | Роут | Что делает |
|---|---|---|
| GET | /fhir/practitioners/{id} | read по стандарту, в ответе ETag с meta.versionId и Last-Modified |
| GET | /fhir/practitioners?family=&given=&_count=20 | search, возвращает Bundle |

## Принятые решения

- **Идемпотентность.** Upsert: запись сначала ищется по Id (сообщения из Kafka приходят с готовым Id), при создании через API — по email. На email висит уникальный индекс в БД, гонка «два одинаковых запроса одновременно» ловится по SqlException 2627/2601 и превращается в 409 Conflict.
- **Конкурентность.** RowVersion ([Timestamp] в EF Core) — оптимистичная блокировка из коробки.
- **Мягкое удаление.** Записи физически не удаляются, проставляется DeletedOn, во всех выборках они фильтруются.
- **Консьюмер.** Ручной коммит оффсета только после успешной обработки. При ошибке обработки сообщения оффсет не двигается, сообщение не теряется, через 5 секунд повтор. На каждое сообщение создаётся отдельный DI-scope, потому что DbContext у нас scoped, а воркер — синглтон.
- **Конфигурация.** В appsettings оставлены значения для локальной разработки.
- **Особенность с Redpanda.** Брокеру обязательно нужен --advertise-kafka-addr: без него он сообщает клиентам свой адрес как 127.0.0.1, и клиенты из других контейнеров подключиться не могут (каждый идёт на свой собственный localhost).

## Что не сделано из опционального

- Кэширование GET (Redis) — не реализовано.
- Полнотекстовый поиск сделан простым LIKE по firstName/lastName, без отдельного поискового движка.
- DLQ для битых сообщений нет: сейчас проблемное сообщение ретраится бесконечно с паузой. В проде добавил бы счётчик попыток и отправку в отдельный топик после N неудач.

## Структура решения

- **PractitionerDirectory.API** — Web API: контроллеры (REST + FHIR), мапперы, middleware глобальной обработки ошибок.
- **PractitionerDirectory.Integrations** — Worker Service, консьюмер Kafka.
- **PractitionerDirectory.Services** — бизнес-логика, DTO, кастомные исключения.
- **PractitionerDirectory.Repositories** — EF Core: контекст, сущность, репозиторий, миграции, сидер.

Коллекция запросов для Postman — PractitionerDirectory.postman_collection.json в корне репозитория.
