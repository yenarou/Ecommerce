# Hilo y Alma

E-commerce de crochet con front en Astro y backend en C#.

## Requisitos

Docker Desktop.

## Configuración

Crea un archivo `.env` en la rama principal con:

    ADMIN_PASSWORD=...
    MERCADOPAGO_ACCESS_TOKEN=... (credencial de PRUEBA)
    PUBLIC_MP_PUBLIC_KEY=... (clave pública de PRUEBA, de la misma app)

Si cambias las claves, reconstruye: `docker compose up -d --build`.

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

## Pruebas
- Tienda: http://localhost:15174
- Admin: http://localhost:15174/admin (usuario admin23@gmail.com)
- Tarjetas de prueba: nombre del titular APRO (aprueba), FUND (fondos insuficientes), SECU (código de seguridad inválido).