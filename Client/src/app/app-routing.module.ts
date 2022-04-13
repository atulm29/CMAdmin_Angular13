import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserRoleListComponent } from './components/adminusers/roles/role.list.component';
import { UserImportsComponent } from './components/adminusers/import/imports.component';

import { UsersComponent } from './components/adminusers/users/users.list.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { Facilities } from './components/facilities/facilities.component';
import { MembersComponent } from './components/members/members.component';
import { Organizations } from './components/organizations/organizations.component';
import { SigninComponent } from './components/signin/signin.component';
import { AuthGuard } from './core';
import { AuthorizedLayoutMiddleComponent } from './layout/authorized/authorized-layout-middle/authorized-layout.component';
import { DashBoardLayoutMiddleComponent } from './layout/dashboard-layout/dashboard-layout-middle/dashboard-layout-middle.component';
import { UserAddNewRoleComponent } from './components/adminusers/roles/role.add.component';
import { UserEditRoleComponent } from './components/adminusers/roles/role.edit.component';
import { AddAdminUsersComponent } from './components/adminusers/users/users.add.component';
//import { AuthGuard } from './shared/auth.guard';


const routes: Routes = [
  { path: 'members', component: MembersComponent }, 
  { path: 'login', component: SigninComponent },  
  {
    path: '',
    component: DashBoardLayoutMiddleComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard]  },
      { path: '', redirectTo: '/login', pathMatch: 'full' },    
    ]
  }, 
  {
    path: '',
    component: AuthorizedLayoutMiddleComponent,
    children: [
      { path: 'organizations', component: Organizations, canActivate: [AuthGuard]  }, 
      { path: 'facilities', component: Facilities, canActivate: [AuthGuard]  }, 
      { path: 'adminusers', component: UsersComponent, canActivate: [AuthGuard] },
      { path: 'adminadduser', component: AddAdminUsersComponent, canActivate: [AuthGuard] },
      { path: 'adminroles', component: UserRoleListComponent, canActivate: [AuthGuard] },
      { path: 'addnewrole', component: UserAddNewRoleComponent, canActivate: [AuthGuard] },
      { path: 'editrole/:id', component: UserEditRoleComponent, canActivate: [AuthGuard] },
      { path: 'adminuserimports', component: UserImportsComponent, canActivate: [AuthGuard] },
      { path: '**', redirectTo: '/login', pathMatch:'full'}
    ]
  },  
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login', pathMatch:'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
