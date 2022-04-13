
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';


const apiUrl = 'https://localhost:44359/api/UsersRoles';
      

@Injectable({
    providedIn: 'root',
  })

  export class UserRolesService {
    headers = new HttpHeaders().set('Content-Type', 'application/json');
   
    constructor(private http: HttpClient, public router: Router) {}
  
    getAll(params: any): Observable<any> {
      return this.http.get<any>(apiUrl + "/GetAllRole", { headers: this.headers, params: params });
    }

    getPermission(params: any): Observable<any> {
      return this.http.get<any>(apiUrl + "/GetPermission", { headers: this.headers, params: params });
    }

    createRole(data: any): Observable<any> {
      return this.http.post(apiUrl + '/SaveRoles', data, { headers: this.headers });  
    }

    getRolePermissionById(id: any): Observable<any> {
      return this.http.get(`${apiUrl}/${id}`, {headers: this.headers} );
    }
  }