import { DiffSegment } from './diff-segment.interface'

export interface AttemptResult {
  exerciseStatus: string
  question: string
  answer: string | null
  correctAnswer: string
  diff: DiffSegment[]
}
