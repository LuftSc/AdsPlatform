# Тестовое задание для EffectiveApps

## Для работы программы необходимо запустить её в Docker. Перейдите в папку TestTask-AdsPlatform и выполните последовательно следующие команды:

```bash
docker build -t ads-platform .
```

```bash
docker run -d -p 8080:8080 --name ads-platform-container ads-platform
```

После успешного запуска программный интерфейс приложения (API) будет доступно по адресу:
http://localhost:8080/swagger
