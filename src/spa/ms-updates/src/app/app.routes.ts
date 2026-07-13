import { Routes } from '@angular/router';
// layouts
import { MainLayout } from './layouts/main-layout';

export const routes: Routes = [
    {
		path: '',
		component: MainLayout,
		children: [
			{
				// blog module
				path: '', loadChildren: () => import('./modules/ms-updates/ms-updates.route').then(m => m.MsUpdatesRoutes)
			}
		]
	},
    { path: '**',   redirectTo: '404', pathMatch: 'full' } // redirect to 404 screen (not found) in the default (blog) module
];
