# Task Management System

Микросервисное приложение для управления задачами, реализованное с использованием ASP.NET Core, PostgreSQL и SignalR.

## Состав системы

- **AuthService** — регистрация и аутентификация пользователей
- **TaskService** — управление задачами
- **NotificationService** — уведомления в реальном времени через SignalR
- **Gateway** — YARP reverse proxy для проксирования запросов

---

## API коллекция
Postman коллекция с основными запросами:
- [TaskManagement.postman_collection.json](./docs/TaskManagementSystem.postman_collection.json)

---

## Примеры запросов (cURL)

### Регистрация пользователя

```bash
curl -X POST http://localhost:5125/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Qwerty123!",
    "fullName": "Test User"
  }'
```

### Авторизация (получение токена)

```bash
curl -X POST http://localhost:5125/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Qwerty123!"
  }'
```

### Создание новой задачи

```bash
curl -X POST http://localhost:5125/api/tasks \
  -H "Authorization: Bearer <токен>" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Новая задача",
    "description": "Описание задачи",
    "assigneeId": "guid_пользователя"
  }'
```