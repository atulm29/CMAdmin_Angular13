import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
const apiUrl = `${environment.apiUrl}`;

//const apiUrl = 'https://localhost:44359/api/Organization';
      
@Injectable({
  providedIn: 'root',
})

export class LeftNavService {
    headers = new HttpHeaders().set('Content-Type', 'application/json');
    
   constructor(private http: HttpClient, public router: Router) {}
 
   getleftNavMenus() {
     return this.leftMenus;
   }
 
   leftMenus: any[] = [
    {
      "tabName": "GroupManagement",
      "url": "organizations",
      "alias": "Organizations",
      "leftCssClass": "nav-link pt-3  pb-3 Organizanav text-center"
    },
    {
      "tabName": "InstituteManagement",
      "url": "facilities",
      "alias": "Facilities",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Institutenav"
    },
    {
      "tabName": "InstructorManagement",
      "url": "adminroles",
      "alias": "Admin Users",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Instructnav"
    },
    {
      "tabName": "CourseManagement",
      "url": "researchreports",
      "alias": "Research Reports",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Coursenav"
    },
    {
      "tabName": "StudentManagement",
      "url": "members",
      "alias": "Members",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Studentnav"
    },
    {
      "tabName": "SwitchToScoreCard",
      "url": "scorecard",
      "alias": "Scorecard",
      "leftCssClass": "nav-link pt-3 pb-3 text-center switchtoscorecardnav"
    },
    {
      "tabName": "LicenseCourseMetaMaster",
      "url": "audience",
      "alias": "License &amp; Course Meta Master",
      "leftCssClass": "nav-link pt-3 pb-3 text-center licenseCourseMetaMasterNav"
    },
    {
      "tabName": "Events",
      "url": "events",
      "alias": "Events",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Eventnav"
    },
    {
      "tabName": "GroupManagement",
      "url": "community",
      "alias": "Community",
      "leftCssClass": "nav-link pt-3 pb-3 text-center Communitynav"
    }
  ];
 
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
 