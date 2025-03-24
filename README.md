<h1>Рабочий backend на c#/.NET</h1>

<p><b>.NET 8.0:</b> Проект создан с использованием .NET 8.0</p>

<p><b>Entity Framework Core:</b> Entity Framework Core используется для управления операциями с базой данных. Он предоставляет объектно-реляционный маппер (ORM), позволяющий разработчикам работать с базой данных с использованием объектов C#</p>

<p><b>Microsoft.AspNetCore.Mvc</b>(на основе <b>Web Api</b> с поддержкой <b>OpenApi</b>): Этот фреймворк используется для создания веб-приложений в .NET. Он предоставляет набор компонентов и функций для обработки HTTP-запросов, маршрутизации, привязки моделей и других вещей.</p>

<p><b>Microsoft.AspNetCore.Authentication.JwtBearer:</b> Этот пакет предоставляет проверку подлинности JWT (JSON Web Token) для приложений ASP.NET Core. Он позволяет безопасную проверку подлинности и авторизацию для конечных точек API.</p>

<p><b>Microsoft.EntityFrameworkCore.Design:</b> Этот пакет требуется для операций во время разработки в Entity Framework Core, таких как миграции и обновления базы данных.</p>

<h3>Структура проекта:</h3>

- Работет Auth
- Проверка на токен с ролью и ошибки/повторы, тд..
- Создание, изменение, удаление продкута только для Админа
---
- Так же планируется другой функционал, о нем потом))
```
Backend/
    │
    ├───Controllers/
    │   ├───AuthController.cs
    │   └───ProductController.cs
    │
    ├───DTOs/
    │   ├───AuthDto.cs
    │   ├───ProdDto.cs
    │   └───UpdateUserDto.cs
    │
    ├───Database/
    │   └───AppDbContext.cs
    │
    ├───Identity/
    │   └───IdentityData.cs
    │
    ├───Migrations/
    │   ├───20230101000000_InitialCreate.cs
    │   └───20230101000000_InitialCreate.Designer.cs
    │
    ├───Models/
    │   ├───Image.cs
    │   ├───Product.cs
    │   └───User.cs
    │
    ├───Properties/
    │   └───launchSettings.json
    │
    ├───Services/
    │   └───JwtService.cs
    │
    ├───Swagger/
    │   └───ConfigureSwaggerOptions.cs
    │
    │───Utils/..
    │   Backend.sln
    │   Backend.csproj
    │───appsettings.Development.json
    └───appsettings.json
```

## Обязательно

Перед запуском заполнить в `appsettings.json`

Эту строчку `"DbConnectionString": "Host=localhost;Port=Port;Database=DatabaseName;Username=postgres;Password=Password"`

<h3>Запуск проекта:</h3>

Для запуска проекта в `Visual Studio` просто нажмите кнопку http/https/IIS Express ![image](https://github.com/user-attachments/assets/43bd3748-01cc-4ef3-a269-92513c94d7f0)


### Иначе, если вы делаете через консоль

- Откройте каталог MerchMarket в терминале или командной строке
- Перейдите в каталог Backend с помощью команды `cd`
- Выполните следующую команду для восстановления зависимостей проекта: `dotnet restore`
- Выполните следующую команду для сборки проекта: `dotnet build`
- Выполните следующую команду для запуска сервера бэкенда: `dotnet run`

### После запуска вас сразу же перекинет на локальный хост:
![image](https://github.com/user-attachments/assets/dad46854-b9a2-403c-a55d-af5be5adf6aa)
