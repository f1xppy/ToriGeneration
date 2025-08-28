import { Torus } from './torus';

export type TorusResponse = {
  torusList: Torus[],
  totalCount: number,
  concentration: number,
  elapsedTime: string
}
