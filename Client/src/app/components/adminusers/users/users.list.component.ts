import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/core';
import { OrganizationDropDownData } from 'src/app/models/Organization';
import { InstituteRoleDropDownData } from 'src/app/models/tabledatamodel';
import { Users } from 'src/app/models/users';
import { FacilitiesService } from 'src/app/services/facilities.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { UserService } from 'src/app/services/users.service';



@Component({
  selector: 'app-admin-userslist',
  templateUrl: './users.list.component.html',
  styleUrls: ['./users.list.component.css']
})
export class UsersComponent implements OnInit 
{
  organizationNametitle = '0';
  facilityNametitle = '0';
  OrganizationDropDownList: OrganizationDropDownData[] = [];
  FacilityDropDownList: InstituteRoleDropDownData[] = [];
  collegeId = 0;
  groupId = 0;
  UserList: Users[] = [];
  OrgDropDownListForm = this.fb.group({
    organizationName: [''],
    facilityName: ['']
  })

  constructor( public authService: AuthService,
    public organizationService: OrganizationService, 
    public userService: UserService,
    public facilityService: FacilitiesService,
    public fb: FormBuilder, private router: Router,) {  }
    
    ngOnInit(): void 
    { 
      this.getOrganizationDropDownListData();   
      this.getFacilityDropDownListData();
      this.getUsersData();
    }

    getOrganizationDropDownListData(): void 
    {
       this.organizationService.getOrganizations()
        .subscribe({
          next: (data) => {
            this.OrganizationDropDownList = data.result;             
          },
          error: (err) => { console.log(err);  }
        });
    }
    getFacilityDropDownListData(): void 
    {
      let paramValue = {groupId: this.groupId, collegeId: this.collegeId};
      this.facilityService.getFacilityDropDownData(paramValue)
        .subscribe({
          next: (data) => {
            this.FacilityDropDownList = data.result; 
            this.facilityNametitle = '0';  
          },
          error: (err) => { console.log(err);  }
        });
    }
    getUsersData(): void
    {
      let paramValue = {collegeId: this.facilityNametitle};    
      console.log("this.collegeId :" + paramValue);
      this.userService.getUsers(paramValue)
      .subscribe({
        next: (data) => {
           this.UserList = data.result;       
        },
        error: (err) => { console.log(err);   }
      });
    }

    chngeOrgDropDown(e: any) {
      console.log(e.target.value);
      this.groupId = e.target.value;
     this.getFacilityDropDownListData();
   }
  onSubmit() {
    this.getUsersData();
  }
  AddNewUser()
  {
    this.router.navigate(['adminadduser']);
  }
}
