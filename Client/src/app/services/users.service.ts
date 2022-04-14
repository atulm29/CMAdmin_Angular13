
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { User } from '../models/user';



const apiUrl = 'https://localhost:44359/api/Users';
      

@Injectable({
    providedIn: 'root',
  })

  export class UserService {
    headers = new HttpHeaders().set('Content-Type', 'application/json');
   
    constructor(private http: HttpClient, public router: Router) {}

    get(id: any): Observable<any> {
      return this.http.get(`${apiUrl}/${id}`, {headers: this.headers} );
    }
    getUsers(params: any): Observable<any> {
      return this.http.get<any>(apiUrl + "/GetUsers", { headers: this.headers, params: params });
    }

    createUser(data: any): Observable<any> {
      return this.http.post(apiUrl + '/SaveUser', data, { headers: this.headers });  
    }    

    updateUser(data: any): Observable<any> {
      return this.http.post(apiUrl + '/UpdateUser', data, { headers: this.headers });  
    }  
  }