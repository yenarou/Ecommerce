require('dotenv').config()
const { ApolloServer } = require('@apollo/server')
const { startStandaloneServer } = require('@apollo/server/standalone')

const typeDefs = require('./typeDefs')
const resolvers = require('./resolvers')

const server = new ApolloServer({ typeDefs, resolvers })

const PORT = process.env.PORT || 4000
const HOST = process.env.HOST || '0.0.0.0'

async function main() {
  const { url } = await startStandaloneServer(server, {
    listen: { port: PORT, host: HOST },
  })
  console.log(`Servidor GraphQL listo en ${url}`)
}

main()
