import { lazy } from 'react';

const Wishes = lazy(() => import('pages/Wishes'));
const TrackMood = lazy(() => import('pages/TrackMood'));
const Summary = lazy(() => import('pages/Summary'));

const routes = [
  {
    id: "Wishes",
    component: Wishes,
    title: "Wishes",
    path: "/Wishes",
  },
  {
    id: "TrackMood",
    component: TrackMood,
    title: "TrackMood",
    path: "/TrackMood",
  },
  {
    id: "Summary",
    component: Summary,
    title: "Summary",
    path: "/Summary",
  },
];

export default routes;