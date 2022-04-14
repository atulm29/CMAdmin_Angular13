import { Component, OnInit } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/core";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { permission } from "src/app/models/permission";
import { InstituteRoleDropDownData } from "src/app/models/tabledatamodel";
import { UserRoles } from "src/app/models/userroles";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";
import { UserRolesService } from "src/app/services/userroles.service";
import { UserService } from "src/app/services/users.service";
import Validation from "src/app/shared/validation";


@Component({
    selector: 'app-admin-editadminusers',
    templateUrl: './users.edit.component.html',
    styleUrls: ['./users.edit.component.css']
})

export class EditAdminUsersComponent implements OnInit 
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
    
    EditUserForm = this.fb.group({
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
        return this.EditUserForm.controls;
    }
    
    constructor( public authService: AuthService, public organizationService: OrganizationService, 
        public userRoleService: UserRolesService, public facilityService: FacilitiesService, 
        public userService:  UserService,
        public fb: FormBuilder, private router: Router, private activatedRoute: ActivatedRoute){  }

    ngOnInit(): void 
    { 
        this.getOrganizationDropDownListData();       
        this.getFacilityDropDownListData();    
       // this.getUserRolesData();
        this.getEditUserData();
    }    
    getEditUserData(): void
    {
        var getId = this.activatedRoute.snapshot.paramMap.get('id');
        this.userService.get(getId).subscribe({
            next: data => {
              console.log('Edit User data get:', JSON.stringify(data));       
              if(data.status == 0)
              {
                this.collegeId = data.result.collegeId;
                  this.getUserRolesData(); 
                this.EditUserForm.setValue({
                    organizationName: data.result.groupId,
                    facilityName: data.result.collegeId,
                    firstName: data.result.firstName,
                    lastName: data.result.lastName,
                    loginName: data.result.loginName,
                    password: data.result.password,
                    confirmPassword: data.result.password,
                    userRoleName: data.result.roleId
                  });  
                               
              }
              else if (data.status == -1 || data.status == -2)
              {
                this.errorMessage = data.Error.Message;
              }
              
            },
            error: err => {
              this.errorMessage = 'get edit user data failed.';        
            }
          });
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
         if(e.target.value == "") { this.groupId = 0} 
         else { this.groupId =  e.target.value; this.collegeId = 0; this.facilityNameRole ="";};
        this.getFacilityDropDownListData();
    }
    
    changeFacilityDropDown(e: any) {
        console.log(e.target.value);
        if(e.target.value == "") 
        { this.collegeId = 0} 
        else { this.collegeId =  e.target.value; this.roleNameModel = "";};
        this.getUserRolesData();
    }

    submitForm() 
    {
        this.submitted = true;
        if (this.EditUserForm.invalid) { return; }  
        //console.log("Add New Form :" + JSON.stringify(this.EditUserForm.value));  
        var param = 
        {
          AdminUserId: this.activatedRoute.snapshot.paramMap.get('id'),
          LoginName: this.EditUserForm.value.loginName,
          Password: this.EditUserForm.value.password,
          FirstName: this.EditUserForm.value.firstName,
          LastName: this.EditUserForm.value.lastName,
          CollegeId: this.EditUserForm.value.facilityName,
          UserType: '',
          RoleId:this.EditUserForm.value.userRoleName
        }
        console.log(param);

       this.userService.updateUser(param).subscribe({
          next: data => {
            console.log('Edit user :', JSON.stringify(data));       
            if(data.status == 0)
            {
              this.router.navigate(['adminusers']); 
            }
            else if (data.status == -1 || data.status == -2)
            {
              this.addRoleError = true;
              this.errorMessage = data.Error.Message;
            }
          },
          error: err => {
            this.addRoleError = true;
            this.errorMessage = 'Edit user Failed.';        
          }
        });
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