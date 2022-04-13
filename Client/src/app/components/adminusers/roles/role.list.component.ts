import { Component, OnInit } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/core";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { InstituteRoleDropDownData } from "src/app/models/tabledatamodel";
import { UserRoles } from "src/app/models/userroles";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";
import { UserRolesService } from "src/app/services/userroles.service";

@Component({
  selector: 'app-admin-usersroleslist',
  templateUrl: './role.list.component.html',
  styleUrls: ['./role.list.component.css']
})
export class UserRoleListComponent implements OnInit {
  organizationNametitle = '0';
  facilityNametitle = '0';
  OrganizationDropDownList: OrganizationDropDownData[] = [];
  FacilityDropDownList: InstituteRoleDropDownData[] = [];
  collegeId = 0;
  groupId = 0;
  UserRolesList: UserRoles[] = [];
  OrgDropDownListForm = this.fb.group({
    organizationName: [''],
    facilityName: ['']
  })
  constructor( public authService: AuthService,
               public organizationService: OrganizationService, 
               public userRoleService: UserRolesService,
               public facilityService: FacilitiesService,
               public fb: FormBuilder, private router: Router,) {  }
  ngOnInit(): void 
  { 
    this.getOrganizationDropDownListData();
    this.getUserRolesData();
    this.getFacilityDropDownListData();
  }

  getUserRolesData(): void
  {
    let paramclgValue = {collegeId: this.facilityNametitle};    
    console.log("this.collegeId :" + paramclgValue);
    this.userRoleService.getAll(paramclgValue)
    .subscribe({
      next: (data) => {
         this.UserRolesList = data.result;       
      },
      error: (err) => { console.log(err);   }
    });
  }
  getOrganizationDropDownListData(): void 
  {
    this.organizationService.getOrganizations()
      .subscribe({
        next: (data) => { 
          this.OrganizationDropDownList = data.result; 
        },
        error: (err) => {console.log(err); }
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
        error: (err) => {console.log(err); }
      });
  }
  onSubmit() {
     this.getUserRolesData();
  }

  chngeOrgDropDown(e: any) {
     console.log(e.target.value);
     this.groupId = e.target.value;
    this.getFacilityDropDownListData();
  }

  AddNewRole()
  {
    this.router.navigate(['addnewrole']); 
  }
}
