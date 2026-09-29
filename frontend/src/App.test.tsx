import { render, screen, waitFor } from '@testing-library/react'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'
import * as api from './api'

describe('App', () => {
  beforeEach(() => {
    vi.spyOn(api, 'fetchMembers').mockResolvedValue([
      { id: 1, fullName: 'Ada Lovelace', email: 'ada@example.com', joinedOnUtc: '2024-01-01', isActive: true },
    ])
    vi.spyOn(api, 'fetchClasses').mockResolvedValue([
      { id: 1, title: 'Spin', instructor: 'Jo', startsAtUtc: '2024-01-01', capacity: 10, bookedCount: 3 },
    ])
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('renders members and classes once loaded', async () => {
    render(<App />)

    await waitFor(() => {
      expect(screen.getByText(/Ada Lovelace/)).toBeInTheDocument()
    })

    expect(screen.getByText(/Spin with Jo/)).toBeInTheDocument()
  })

  it('shows an error message when loading fails', async () => {
    vi.spyOn(api, 'fetchMembers').mockRejectedValue(new Error('network down'))

    render(<App />)

    await waitFor(() => {
      expect(screen.getByRole('alert')).toHaveTextContent('network down')
    })
  })
})
