import { Routes } from '@angular/router';
// screens
import { LatestUpdates } from './screens/latest-updates';
import { NotFound } from '../../core/components/not-found';

// routes for the module
export const MsUpdatesRoutes: Routes = [
  { path: '', component: LatestUpdates },
    // 404 route (not found)
  { path: '404', component: NotFound }
];