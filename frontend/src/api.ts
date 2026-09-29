const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

export interface Member {
  id: number
  fullName: string
  email: string
  joinedOnUtc: string
  isActive: boolean
}

export interface ClassSession {
  id: number
  title: string
  instructor: string
  startsAtUtc: string
  capacity: number
  bookedCount: number
}

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`)
  if (!response.ok) {
    throw new Error(`Request to ${path} failed with status ${response.status}`)
  }
  return (await response.json()) as T
}

export function fetchMembers(): Promise<Member[]> {
  return getJson<Member[]>('/api/members')
}

export function fetchClasses(): Promise<ClassSession[]> {
  return getJson<ClassSession[]>('/api/classes')
}
