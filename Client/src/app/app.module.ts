import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SigninComponent } from './components/signin/signin.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
//import { AuthInterceptor } from './shared/auth.interceptor';
import { PageContentComponent } from './layout/page-content/page-content.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DashBoardLayoutTopComponent } from './layout/dashboard-layout/dashboard-layout-top/dashboard-layout-top.component';
import { DashBoardLayoutFooterComponent } from './layout/dashboard-layout/dashboard-layout-footer/dashboard-layout-footer.component';
import { DashBoardLayoutMiddleComponent } from './layout/dashboard-layout/dashboard-layout-middle/dashboard-layout-middle.component';
import { AuthorizedLayoutTopComponent } from './layout/authorized/authorized-layout-top/authorized-layout-top.component';
import { AuthorizedLayoutLeftComponent } from './layout/authorized/authorized-layout-left/authorized-layout-left.component';
import { AuthorizedLayoutMiddleComponent } from './layout/authorized/authorized-layout-middle/authorized-layout.component';
import { AuthorizedLayoutFooterComponent } from './layout/authorized/authorized-layout-footer/authorized-layout-footer.component';

import { NgxPaginationModule } from 'ngx-pagination';
import { CommonModule } from '@angular/common';
import { Organizations } from './components/organizations/organizations.component';
import { Facilities } from './components/facilities/facilities.component';
import { MembersComponent } from './components/members/members.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UsersComponent } from './components/adminusers/users/users.list.component';
import { UserImportsComponent } from './components/adminusers/import/imports.component';
import { CoreModule } from './core/core.module';
import { UserRoleListComponent } from './components/adminusers/roles/role.list.component';
import { UserAddNewRoleComponent } from './components/adminusers/roles/role.add.component';
import { UserEditRoleComponent } from './components/adminusers/roles/role.edit.component';
import { AddAdminUsersComponent } from './components/adminusers/users/users.add.component';
import { EditAdminUsersComponent } from './components/adminusers/users/users.edit.component';


@NgModule({
  declarations: [
    AppComponent,
    PageContentComponent,
    DashBoardLayoutTopComponent,
    DashBoardLayoutFooterComponent,
    DashBoardLayoutMiddleComponent,
    DashboardComponent,  
    AuthorizedLayoutTopComponent,
    AuthorizedLayoutLeftComponent,  
    AuthorizedLayoutMiddleComponent,
    AuthorizedLayoutFooterComponent,
    SigninComponent,    
    Organizations,
    Facilities,
    UserRoleListComponent,
    UserAddNewRoleComponent,
    UserEditRoleComponent,
    MembersComponent,  
    UsersComponent,
    AddAdminUsersComponent,
    EditAdminUsersComponent,
    UserImportsComponent    
  ],
  imports: [
    BrowserModule,
    NgbModule, 
    CommonModule,   
    AppRoutingModule,
    CoreModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    NgxPaginationModule
  ],
  providers: [
   
  ],
  bootstrap: [AppComponent],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AppModule { }
