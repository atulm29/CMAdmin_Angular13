import { Component, OnInit } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "src/app/core";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { permission } from "src/app/models/permission";
import { InstituteRoleDropDownData } from "src/app/models/tabledatamodel";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";
import { UserRolesService } from "src/app/services/userroles.service";


@Component({
    selector: 'app-admin-useraddnewrole',
    templateUrl: './role.add.component.html',
    styleUrls: ['./role.add.component.css']
  })

export class UserAddNewRoleComponent implements OnInit 
{
    orgNameModel = "";
    facilityNameRole ="";
    submitted = false;
    addRoleError = false;
    OrganizationDropDownList: OrganizationDropDownData[] = [];
    FacilityDropDownList: InstituteRoleDropDownData[] = [];
    collegeId = 0;
    groupId = 0;
    PermissionData: permission[] = [];
    errorMessage ='';
      
    AddNewRoleForm = this.fb.group({
        organizationName:  ['0', [Validators.required]],
        facilityName:  ['0', [Validators.required]],
        roleName: ['', [Validators.required]],
        name: this.fb.array([], [Validators.required])
      })

    get f(): { [key: string]: AbstractControl } {
        return this.AddNewRoleForm.controls;
      }
    constructor( public authService: AuthService, public organizationService: OrganizationService, 
        public userRoleService: UserRolesService, public facilityService: FacilitiesService,
        public fb: FormBuilder, private router: Router){ }

    onChange(name: number, e: any) 
    {
        const cartoons = (this.AddNewRoleForm.get('name') as FormArray);        
        if (e.target.checked) {
          cartoons.push(new FormControl(name));
        } else {
          const index = cartoons.controls.findIndex(x => x.value === name);
          cartoons.removeAt(index);
        }
    }
    ngOnInit(): void 
    { 
        this.getOrganizationDropDownListData();       
        this.getFacilityDropDownListData(); 
        this.getPermissionCheckBoxList();       
    }
    getPermissionCheckBoxList(): void 
    {
      let paramValue = {collegeId: this.collegeId};
      this.userRoleService.getPermission(paramValue)
        .subscribe({ next: (data) => {        
            this.PermissionData = data.result;            
          },
          error: (err) => { console.log(err); }
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
           // this.facilityNametitle = '0';  
          },
          error: (err) => { console.log(err); }
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
        this.getPermissionCheckBoxList();
    }
      

    submitForm() {
      this.submitted = true;
      if (this.AddNewRoleForm.invalid) { return; }
      var tempPermissionData = [];

      for(let j=0; j < this.AddNewRoleForm.value.name.length; j++)
      {
        var filter_array = this.PermissionData.find(x => x.adminDashBoardId.toString() == this.AddNewRoleForm.value.name[j]);
        if(filter_array != null)
        {
          tempPermissionData.push(filter_array);
        }
      }
       var param = {CollegeId: this.AddNewRoleForm.value.facilityName,
                  RoleId: "0", 
                  RoleName: this.AddNewRoleForm.value.roleName,
                  Permissions: tempPermissionData
                 }
      console.log(param);
      this.userRoleService.createRole(param).subscribe({
        next: data => {
          console.log('New Roles added:', JSON.stringify(data));       
          if(data.status == 0)
          {
            this.router.navigate(['adminroles']); 
          }
          else if (data.status == -1 || data.status == -2)
          {
            this.addRoleError = true;
            this.errorMessage = data.Error.Message;
          }
        },
        error: err => {
          this.addRoleError = true;
          this.errorMessage = 'Add New Role Failed.';        
        }
      });

    }

    cancel(){
      this.router.navigate(['adminroles']); 
  }
}