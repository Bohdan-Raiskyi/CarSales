Лабораторна робота №1 з предмету ІСтТП. Предметна область - сайт з продажу вживаних автомобілів. БД - SQL Server. 
Виконані етапи 1.1 - 1.5
# Pipeline системи MVC (ASP.NET Core)

Цей репозиторій демонструє стандартний життєвий цикл HTTP-запиту у веб-додатку на основі патерну **Model-View-Controller (MVC)**.

## 🔄 Послідовність взаємодії

1. **Користувач** надсилає HTTP-запит.
2. **Routing** визначає, який контролер і метод будуть обробляти запит.
3. **Controller** викликає відповідну **Model** для обробки логіки та доступу до БД.
4. **Model** взаємодіє з **Database** (наприклад, через Entity Framework).
5. **Controller** отримує дані та передає їх у **View**.
6. **View** генерує HTML-відповідь, яка повертається **користувачу**.

## 📊 Сіквенс-діаграма

```mermaid
sequenceDiagram
    participant User
    participant Routing
    participant Controller
    participant Model
    participant Database
    participant View

    User->>Routing: HTTP-запит
    Routing->>Controller: Визначення контролера та дії
    Controller->>Model: Запит на дані
    Model->>Database: Звернення до БД (EF/сервіси)
    Database-->>Model: Дані з БД
    Model-->>Controller: Оброблені дані
    Controller->>View: Передача даних для відображення
    View-->>User: HTML-відповідь
