import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/core';

@Component({
  selector: 'app-admin-userimports',
  templateUrl: './imports.component.html',
  styleUrls: ['./imports.component.css']
})
export class UserImportsComponent implements OnInit {



  constructor( public authService: AuthService, private actRoute: ActivatedRoute) 
  {
   
  }



  ngOnInit(): void { }
}