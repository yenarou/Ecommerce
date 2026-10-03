# Hilo y Alma

E-commerce de crochet con front en Astro y backend en C#.

## Requisitos

Docker Desktop.

## Arranque

Desde esta carpeta ejecuta:

```bash
docker compose up --build
```

La primera vez tarda varios minutos. Después abre `http://localhost:15174`. La API GraphQL queda en `http://localhost:14001/graphql`.

## Puertos

Los puertos por defecto son `15174` para Astro y `14001` para GraphQL. Se pueden cambiar en PowerShell:

```powershell
$env:BACKEND_PORT="24001"
$env:FRONTEND_PORT="25174"
$env:PUBLIC_API_URL="http://localhost:24001/graphql"
docker compose up --build
```

## Borrar los datos

Para eliminar los datos de las bases (PostgreSQL y MongoDB):

```bash
docker compose down -v
docker compose up --build
```