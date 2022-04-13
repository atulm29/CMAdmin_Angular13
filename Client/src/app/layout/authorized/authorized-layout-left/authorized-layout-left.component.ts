
import { Component, OnInit } from "@angular/core";
import { AuthService } from 'src/app/core';
import { LeftNavMenus } from "src/app/models/leftnavmenus";
import { LeftNavService } from "src/app/services/leftnav.service";

@Component({
    selector: 'app-authorized-layout-left',
    templateUrl: './authorized-layout-left.component.html',
    styleUrls: ['./authorized-layout-left.component.css'] 
})

export class AuthorizedLayoutLeftComponent implements OnInit {
    leftnav:string = 'leftnav';
    leftNavMenuList: LeftNavMenus[] = [];

    constructor(private leftnavservice: LeftNavService, private authService : AuthService) {}
    ngOnInit() {
        this.GetLeftMenusList();
    }
    
    GetLeftMenusList() {
        this.leftNavMenuList = this.leftnavservice.getleftNavMenus();
    }

    changeStyle(e: any){
        this.leftnav = e.type == 'mouseover' ? 'leftnav leftnavhover' : 'leftnav';
    }
    logout(): void
    {
       this.authService.logout();
      
    }
}