import { Component, OnInit } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "src/app/core";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { permission } from "src/app/models/permission";
import { InstituteRoleDropDownData } from "src/app/models/tabledatamodel";
import { UserRoles } from "src/app/models/userroles";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";
import { UserRolesService } from "src/app/services/userroles.service";
import Validation from "src/app/shared/validation";


@Component({
    selector: 'app-admin-addadminusers',
    templateUrl: './users.add.component.html',
    styleUrls: ['./users.add.component.css']
})

export class AddAdminUsersComponent implements OnInit 
{
    orgNameModel = "";
    facilityNameRole ="";
    roleNameModel = "";
    roleName = "";
    submitted = false;
    addRoleError = false;
    OrganizationDropDownList: OrganizationDropDownData[] = [];
    FacilityDropDownList: InstituteRoleDropDownData[] = [];
    UserRolesList: UserRoles[] = [];
    collegeId = 0;
    groupId = 0;
    errorMessage ='';
    emailRegex = '^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$'; 
    
    AddNewUserForm = this.fb.group({
        organizationName:  ['0', [Validators.required]],
        facilityName:  ['0', [Validators.required]],
        firstName: ['', [Validators.required]],
        lastName: ['', [Validators.required]],
        loginName: ['', [Validators.required, Validators.pattern(this.emailRegex)]],
        password: ['', [Validators.required]],
        confirmPassword : ['', [Validators.required]],
        userRoleName:  ['0', [Validators.required]]
    },
    {
        validators: [Validation.match('password', 'confirmPassword')]
      })
    
    get f(): { [key: string]: AbstractControl } {
        return this.AddNewUserForm.controls;
    }
    
    constructor( public authService: AuthService, public organizationService: OrganizationService, 
        public userRoleService: UserRolesService, public facilityService: FacilitiesService,
        public fb: FormBuilder, private router: Router){  }

    ngOnInit(): void 
    { 
        this.getOrganizationDropDownListData();       
        this.getFacilityDropDownListData();    
        this.getUserRolesData();
    }    

    getOrganizationDropDownListData(): void 
    {
      this.organizationService.getOrganizations()
        .subscribe({ next: (data) => { 
            this.OrganizationDropDownList = data.result;                 
          },
          error: (err) => { console.log(err); }
        });
    }
    getFacilityDropDownListData(): void 
    {
      let paramValue = {groupId: this.groupId, collegeId: this.collegeId};
      this.facilityService.getFacilityDropDownData(paramValue)
        .subscribe({ next: (data) => {        
            this.FacilityDropDownList = data.result;               
          },
          error: (err) => { console.log(err); }
        });
    }
    getUserRolesData(): void
    {
      let paramclgValue = {collegeId: this.collegeId};    
      console.log("this.collegeId :" + paramclgValue);
      this.userRoleService.getAll(paramclgValue)
      .subscribe({
        next: (data) => {
           this.UserRolesList = data.result;       
        },
        error: (err) => { console.log(err);   }
      });
    }
    chngeOrgDropDown(e: any) {
        console.log(e.target.value);
         if(e.target.value == "") { this.groupId = 0} else { this.groupId =  e.target.value};
        this.getFacilityDropDownListData();
    }
    
    changeFacilityDropDown(e: any) {
        console.log(e.target.value);
        if(e.target.value == "") { this.collegeId = 0} else { this.collegeId =  e.target.value};
        this.getUserRolesData();
    }

    submitForm() 
    {
        this.submitted = true;
        if (this.AddNewUserForm.invalid) { return; }    
    }
  
    cancel(){
        this.router.navigate(['adminusers']); 
    }

    alphaNumberOnly (e: any) {  // Accept only alpha numerics, not special characters 
        var regex = new RegExp("^[a-zA-Z0-9 ]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
    
        e.preventDefault();
        return false;
      }
    
      onPaste(e: any) {
        e.preventDefault();
        return false;
      }
}