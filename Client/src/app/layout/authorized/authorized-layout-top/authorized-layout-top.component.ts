import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core';


@Component({
  selector: 'app-authorized-layout-top',
  templateUrl: './authorized-layout-top.component.html',
  styleUrls: ['./authorized-layout-top.component.css']
})
export class AuthorizedLayoutTopComponent implements OnInit {
  currentUser: any;
  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.currentUser = this.authService.user$; 
    console.log("current user authorized: " + JSON.stringify(this.currentUser.username));
  }
  
  logout(): void {
      this.authService.logout();
      window.location.reload();
  }
}