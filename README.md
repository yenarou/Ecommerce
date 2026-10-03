# WebProyecto con Docker

## Arranque

Desde esta carpeta ejecuta:

```bash
docker compose up --build
```

Abre `http://localhost:15174` para usar la aplicación. La API GraphQL queda disponible en `http://localhost:14001/graphql`.

PostgreSQL y MongoDB se ejecutan como servicios y conservan sus datos en volúmenes Docker.

## Puertos

Los puertos publicados por defecto son `15174` para Astro y `14001` para GraphQL. Se pueden cambiar en PowerShell:

```powershell
$env:BACKEND_PORT="24001"
$env:FRONTEND_PORT="25174"
$env:PUBLIC_API_URL="http://localhost:24001/graphql"
docker compose up --build
```

Para eliminar los datos de ambas bases, elimina los volúmenes:

```bash
docker compose down -v
docker compose up --build
```
