FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG PUBLIC_GRAPHQL_URL=http://localhost:14001/graphql
ARG PUBLIC_API_URL=http://localhost:14001
ARG PUBLIC_AUTH_URL=http://localhost:14001
ENV PUBLIC_GRAPHQL_URL=$PUBLIC_GRAPHQL_URL \
    PUBLIC_API_URL=$PUBLIC_API_URL \
    PUBLIC_AUTH_URL=$PUBLIC_AUTH_URL
RUN npm run build

FROM node:22-alpine AS runtime
WORKDIR /app
ENV HOST=0.0.0.0 PORT=15175
COPY --from=build /app/dist ./dist
COPY --from=build /app/node_modules ./node_modules
EXPOSE 15175
CMD ["node", "./dist/server/entry.mjs"]