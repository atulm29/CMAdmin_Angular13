import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
const apiUrl = `${environment.apiUrl}`;
      
@Injectable({
  providedIn: 'root',
})

export class FacilitiesService {
   headers = new HttpHeaders().set('Content-Type', 'application/json');

  constructor(private http: HttpClient, public router: Router) {}
  
  getAll(params: any): Observable<any> {
    return this.http.get<any>(apiUrl + "api/Facilities/GetFiltered", { headers: this.headers, params: params });
  }

  getFacilityDropDownData(params: any): Observable<any> {
    return this.http.get<any>(apiUrl + "api/Facilities/GetRoleInstitute", { headers: this.headers, params: params });
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