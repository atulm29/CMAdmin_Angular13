
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';


const apiUrl = 'https://localhost:44359/api/Users';
      

@Injectable({
    providedIn: 'root',
  })

  export class UserService {
    headers = new HttpHeaders().set('Content-Type', 'application/json');
   
    constructor(private http: HttpClient, public router: Router) {}
  
    getUsers(params: any): Observable<any> {
      return this.http.get<any>(apiUrl + "/GetUsers", { headers: this.headers, params: params });
    }

  }