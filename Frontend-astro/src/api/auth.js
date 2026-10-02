import { gql } from './client'

// poner USE_MOCK en false 
const USE_MOCK = true

const AUTH_FIELDS = 'token userId username'

function mockAuth(username) {
  return {
    token: 'token-de-prueba',
    userId: '11111111-1111-1111-1111-111111111111', 
    username,
  }
}

export async function login(email, password) {
  if (USE_MOCK) return mockAuth(email.split('@')[0])
  const data = await gql(
    `mutation($email: String!, $password: String!) {
      login(email: $email, password: $password) { ${AUTH_FIELDS} }
    }`,
    { email, password }
  )
  return data.login
}

export async function register(username, email, password) {
  if (USE_MOCK) return mockAuth(username)
  const data = await gql(
    `mutation($username: String!, $email: String!, $password: String!) {
      register(username: $username, email: $email, password: $password) { ${AUTH_FIELDS} }
    }`,
    { username, email, password }
  )
  return data.register
}

export async function loginWithGoogle(code, redirectUri) {
  if (USE_MOCK) return mockAuth('usuario-google')
  const data = await gql(
    `mutation($code: String!, $redirectUri: String) {
      loginWithGoogle(code: $code, redirectUri: $redirectUri) { ${AUTH_FIELDS} }
    }`,
    { code, redirectUri }
  )
  return data.loginWithGoogle
}