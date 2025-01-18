Шаг №0. Настройка кейклока

1. Скачиваем докер десктоп: 

# Скачайте Docker Desktop с официального сайта
https://www.docker.com/products/docker-desktop/
# Установите и перезагрузите компьютер

2. Запуск Keycloak в Docker

# Создайте папку для Keycloak (например, C:\keycloak)
# Откройте PowerShell и выполните:
docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:latest start-dev

3ю Первичная настройка Keycloak


Откройте браузер и перейдите по адресу: http://localhost:8080
Нажмите "Administration Console"
Войдите с учетными данными:

Username: admin
Password: admin


4ю Создание Realm


В левом верхнем углу нажмите кнопку "Master" и выберите "Create Realm"
Заполните поля:

Realm name: MyAppRealm
Enabled: ✓ (включено)


Нажмите "Create"

5. Создание клиента

В левом меню выберите "Clients"
Нажмите "Create client"
На первом шаге:
Client type: OpenID Connect
Client ID: my-app-client 
Name: My Application (можно оставить пустым)

На втором шаге: 
Client authentication: ON (включить)
Authorization: ON (включить)

На третьем шаге: 
Root URL: http://localhost:5000
Home URL: оставить пустым
Valid redirect URIs: 
  - http://localhost:5000/*
  - http://localhost:5001/*
  - https://localhost:5000/*
  - https://localhost:5001/*
  - http://localhost:7001/*
  - https://localhost:7001/*
Web origins: 
  - http://localhost:5000
  - http://localhost:5001
  - https://localhost:5000
  - https://localhost:5001
  - http://localhost:7001
  - https://localhost:7001


Нажмите "Save"


6. Настройка клиента

Перейдите в созданный клиент my-app-client
Перейдите на вкладку "Credentials"
Скопируйте "Client secret" - он понадобится для настройки API
Перейдите на вкладку "Client Scopes"
В Assigned default client scopes должны быть:
email
profile
roles
web-origins

Если какого-то scope нет, добавьте через "Add client scope"


7. Создание роли POWER_USER

В левом меню выберите "Realm roles"
Нажмите "Create role"
Заполните:

Role name: POWER_USER
Description: Role for power users

Нажмите "Save"


8. Создание тестового пользователя

В левом меню выберите "Users"
Нажмите "Add user"
Заполните поля:

Username: poweruser
Email: poweruser@example.com
First name: Power
Last name: User

Email Verified: ✓ (включить)
Нажмите "Save"
Перейдите на вкладку "Credentials"
Нажмите "Set password"


Password: poweruser123
Password confirmation: poweruser123
Temporary: OFF (выключить)

Нажмите "Save"
Перейдите на вкладку "Role mapping"
Нажмите "Assign role"
Выберите "Filter by realm roles"
Найдите и выберите роль "POWER_USER"
Нажмите "Assign"

9. Проверка настроек

# Получение токена через Postman или curl:
POST http://localhost:8080/realms/MyAppRealm/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

client_id=my-app-client
&client_secret=ваш-client-secret
&grant_type=password
&username=poweruser
&password=poweruser123

10. Настройка appsettings.json в API проекте

{
  "Keycloak": {
    "realm": "MyAppRealm",
    "auth-server-url": "http://localhost:8080/",
    "ssl-required": "none",
    "resource": "my-app-client",
    "verify-token-audience": true,
    "credentials": {
      "secret": "ваш-client-secret-который-вы-скопировали"
    },
    "confidential-port": 0
  }
}

11. Настройка appsettings.json в UI проекте

{
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/MyAppRealm",
    "ClientId": "my-app-client",
    "ClientSecret": "ваш-client-secret-который-вы-скопировали"
  }
}