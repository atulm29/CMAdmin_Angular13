import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
const apiUrl = `${environment.apiUrl}`;

@Injectable({
  providedIn: 'root',
})

export class DashboardService {
   headers = new HttpHeaders().set('Content-Type', 'application/json');
   adminTiles: any = {};

  constructor(private http: HttpClient, public router: Router) {}
 
  // Dashboard data
  getDashboardData(): Observable<any> {
    return this.http.get(apiUrl+'api/Dashboard/', { headers: this.headers }).pipe(
      map((res) => {
        return res || {};
      }),
      catchError(this.handleError)
    );
  }

  // Error
  handleError(error: HttpErrorResponse) {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }
}
function httpOptions(arg0: string, data: any, httpOptions: any): Observable<any> {
  throw new Error('Function not implemented.');
}

