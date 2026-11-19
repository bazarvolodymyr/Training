export interface AuthUser {
  isAuthenticated: boolean
  email: string | null
  name: string | null
  roles: string[]
  premiumActive: boolean
}
