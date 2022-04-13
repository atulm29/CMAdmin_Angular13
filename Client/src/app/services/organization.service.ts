import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { OrganizationTableData } from '../models/Organization';
import { environment } from 'src/environments/environment';
const apiUrl = `${environment.apiUrl}`;
   
@Injectable({
  providedIn: 'root',
})

export class OrganizationService {
   headers = new HttpHeaders().set('Content-Type', 'application/json');
   
  constructor(private http: HttpClient, public router: Router) {}

  getAll(params: any): Observable<any> {
    return this.http.get<any>(apiUrl + "api/Organization/GetFiltered", { headers: this.headers, params: params });
  }

  get(id: any): Observable<OrganizationTableData> {
    return this.http.get(`${apiUrl}/${id}`);
  }

  create(data: any): Observable<any> {
    return this.http.post(apiUrl + 'api/Organization/createorganization', data, { headers: this.headers });  
  }

  update(id: any, data: any): Observable<any> {
    return this.http.put(`${apiUrl}/${id}`, data);
  }

  delete(id: any): Observable<any> {
    return this.http.delete(`${apiUrl}/${id}`);
  }

  deleteAll(): Observable<any> {
    return this.http.delete(apiUrl);
  }

  findByTitle(title: any): Observable<OrganizationTableData[]> {
    return this.http.get<OrganizationTableData[]>(`${apiUrl}?title=${title}`);
  }


   getOrganizations(): Observable<any> {
     return this.http.get(apiUrl+ "api/Organization", { headers: this.headers }).pipe(
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

