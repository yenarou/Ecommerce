# WebProyecto con Docker

## Arranque

Desde esta carpeta ejecuta:

```bash
docker compose up --build
```

Abre `http://localhost:15173` para usar la aplicación. La API GraphQL queda disponible en `http://localhost:14000/`.

La base SQLite se guarda en el volumen Docker `webproyecto_sqlite-data` y se inicializa automáticamente la primera vez.

## Puertos

Los puertos publicados por defecto son `15173` para el frontend y `14000` para GraphQL. Se pueden cambiar en PowerShell:

```powershell
$env:BACKEND_PORT="24000"
$env:FRONTEND_PORT="25173"
$env:VITE_API_URL="http://localhost:24000/"
docker compose up --build
```

Para regenerar la base con los datos semilla, elimina el volumen:

```bash
docker compose down -v
docker compose up --build
```
