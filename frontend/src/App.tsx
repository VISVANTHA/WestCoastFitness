import { useEffect, useState } from 'react'
import { fetchClasses, fetchMembers, type ClassSession, type Member } from './api'
import './App.css'

function App() {
  const [members, setMembers] = useState<Member[]>([])
  const [classes, setClasses] = useState<ClassSession[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    Promise.all([fetchMembers(), fetchClasses()])
      .then(([memberData, classData]) => {
        setMembers(memberData)
        setClasses(classData)
      })
      .catch((err: unknown) => {
        setError(err instanceof Error ? err.message : 'Failed to load data')
      })
  }, [])

  return (
    <main>
      <h1>West Coast Fitness Club</h1>
      {error && <p role="alert">{error}</p>}

      <section aria-labelledby="members-heading">
        <h2 id="members-heading">Members</h2>
        <ul>
          {members.map((member) => (
            <li key={member.id}>
              {member.fullName} ({member.email}) — {member.isActive ? 'active' : 'inactive'}
            </li>
          ))}
        </ul>
      </section>

      <section aria-labelledby="classes-heading">
        <h2 id="classes-heading">Upcoming Classes</h2>
        <ul>
          {classes.map((classSession) => (
            <li key={classSession.id}>
              {classSession.title} with {classSession.instructor} — {classSession.bookedCount}/
              {classSession.capacity} booked
            </li>
          ))}
        </ul>
      </section>
    </main>
  )
}

export default App
