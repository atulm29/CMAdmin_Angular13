import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core';


@Component({
  selector: 'app-dashboard-layout-top',
  templateUrl: './dashboard-layout-top.component.html',
  styleUrls: ['./dashboard-layout-top.component.css']
})
export class DashBoardLayoutTopComponent implements OnInit {

  currentUser: any ={};
  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.currentUser = this.authService.getUser();   
    console.log(this.authService.getUser());
  }

  logout(): void
  {
     this.authService.logout();
  }

}
