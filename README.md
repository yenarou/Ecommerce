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

## Despliegue en Railway

1. Sube el repositorio a GitHub. No subas `.env` ni secretos; configúralos en Railway.
2. Crea un proyecto y agrega servicios conectados al mismo repositorio:
   - Backend: Root Directory `/Backend`.
   - Frontend: Root Directory `/Frontend-astro`.
   - Agrega PostgreSQL en Railway y una instancia MongoDB (Railway o un proveedor externo).
3. Genera un dominio público para Backend y Frontend. En las variables del servicio Backend configura:

   ```text
   PORT=8080
   ASPNETCORE_ENVIRONMENT=Production
   ApiBaseUrl=https://${{RAILWAY_PUBLIC_DOMAIN}}
   Cors__AllowedOrigins__0=https://${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
   ConnectionStrings__PostgreSQL=Host=${{Postgres.PGHOST}};Port=${{Postgres.PGPORT}};Database=${{Postgres.PGDATABASE}};Username=${{Postgres.PGUSER}};Password=${{Postgres.PGPASSWORD}}
   ConnectionStrings__MongoDB=<URI de conexión MongoDB>
   Jwt__SecretKey=<secreto aleatorio de al menos 32 caracteres>
   AdminUser__Name=<nombre del administrador>
   AdminUser__Email=<correo del administrador>
   AdminUser__Password=<contraseña fuerte>
   ```

   Los nombres `Postgres`, `Backend` y `Frontend` deben coincidir con los nombres de tus servicios en Railway. Si usas Google o Mercado Pago, configura también `Google__ClientId`, `Google__ClientSecret`, `Google__WebRedirectUri`, `Google__ApiRedirectUri`, `MercadoPago__AccessToken` y `MercadoPago__NotificationUrl` como variables del Backend.

4. En Frontend configura:

   ```text
   PUBLIC_API_URL=https://${{Backend.RAILWAY_PUBLIC_DOMAIN}}/graphql
   API_INTERNAL_URL=http://${{Backend.RAILWAY_PRIVATE_DOMAIN}}:8080/graphql
   PUBLIC_MP_PUBLIC_KEY=<clave pública de Mercado Pago>
   ```

   `PUBLIC_API_URL` y `PUBLIC_MP_PUBLIC_KEY` se usan durante la compilación del frontend; vuelve a desplegar Frontend si los cambias.
5. Si habilitas Google, registra como URI de redirección autorizada:
   `https://<dominio-backend>/api/v1/auth/google/callback`.
   Configura `Google__WebRedirectUri` con esa URL y `Google__ApiRedirectUri` como
   `https://<dominio-frontend>/api/auth/callback`. Para pagos, `MercadoPago__NotificationUrl`
   debe apuntar a `https://<dominio-backend>/api/v1/payments/webhook`.
6. Agrega un volumen al servicio Backend con Mount Path `/app/wwwroot/uploads` para conservar las imágenes cargadas desde el panel.
7. Comprueba `https://<dominio-backend>/graphql/schema` y abre el dominio de Frontend.

Genera un `Jwt__SecretKey` nuevo para producción y guarda contraseñas y tokens solo en Variables de Railway. Cambia las credenciales de desarrollo que hayan estado versionadas; quitarlas del archivo no las elimina del historial de Git.

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