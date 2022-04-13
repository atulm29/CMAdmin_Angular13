import { Component, OnInit } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/core";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { permission } from "src/app/models/permission";
import { InstituteRoleDropDownData } from "src/app/models/tabledatamodel";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";
import { UserRolesService } from "src/app/services/userroles.service";


@Component({
    selector: 'app-admin-usereditrole',
    templateUrl: './role.edit.component.html',
    styleUrls: ['./role.edit.component.css']
  })

export class UserEditRoleComponent implements OnInit 
{
   constructor( public authService: AuthService, public organizationService: OrganizationService, 
        public userRoleService: UserRolesService, public facilityService: FacilitiesService,
        public fb: FormBuilder, private router: Router, private activatedRoute: ActivatedRoute,) { }
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
    editRoleForm = this.fb.group({
        organizationName:  ['0', [Validators.required]],
        facilityName:  ['0', [Validators.required]],
        roleName: ['', [Validators.required]],
        name: this.fb.array([], [Validators.required])
      })

    get f(): { [key: string]: AbstractControl } {return this.editRoleForm.controls; }
    ngOnInit(): void 
    { 
        this.getOrganizationDropDownListData();       
        this.getFacilityDropDownListData(); 
        //this.getPermissionCheckBoxList();       
        this.getEditRoleData();
    }

    onChange(name: number, e: any) {
          
       const index = this.PermissionData.findIndex(x => x.adminDashBoardId === name);
        if (e.target.checked) {
            this.PermissionData[index].isSelected = true;
        } else {
          
            this.PermissionData[index].isSelected = false;
        }
    }  

    getEditRoleData(): void
    {
        var getId = this.activatedRoute.snapshot.paramMap.get('id');
        this.userRoleService.getRolePermissionById(getId).subscribe({
            next: data => {
              console.log('New Organization get:', JSON.stringify(data));       
              if(data.status == 0)
              {
                this.editRoleForm.setValue({
                    organizationName: data.result.groupId,
                    facilityName: data.result.collegeId,
                    roleName: data.result.roleName,
                    name: []
                  });
                  this.PermissionData = data.result.permissions;
                  this.editRoleForm.controls['name'].patchValue(data.result.permissions);
              }
              else if (data.status == -1 || data.status == -2)
              {
                this.errorMessage = data.Error.Message;
              }
            },
            error: err => {
              this.errorMessage = 'get RolePermission Failed.';        
            }
          });
    }

    getPermissionCheckBoxList(): void 
    {
      let paramValue = {collegeId: this.collegeId};
      this.userRoleService.getPermission(paramValue)
        .subscribe({ next: (data) => {        
            this.PermissionData = data.result;   
            console.log('getPermissionCheckBoxList:', JSON.stringify(this.PermissionData)); 
            this.editRoleForm.controls['name'].patchValue(this.PermissionData);   
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
          },
          error: (err) => { console.log(err); }
        });
    }

    chngeOrgDropDown(e: any) {
        console.log(e.target.value);
         if(e.target.value == "") { this.groupId = 0} else { this.groupId =  e.target.value};
        this.getFacilityDropDownListData();
    }

    chngeFacilityDropDown(e: any) {
        console.log(e.target.value);
        if(e.target.value == "") { this.collegeId = 0} else { this.collegeId =  e.target.value};

        this.getPermissionCheckBoxList();
    }

    submitForm() {
        this.submitted = true;
        const permlist = (this.editRoleForm.get('name') as FormArray); 
      
        this.PermissionData.forEach((value :any, index : any) => {
           if (value.isSelected) {
                permlist.push(new FormControl(value));
           }
         })
        if (this.editRoleForm.invalid) { return; }
 
         var param = {CollegeId: this.editRoleForm.value.facilityName,
                    RoleId: this.activatedRoute.snapshot.paramMap.get('id'), 
                    RoleName: this.editRoleForm.value.roleName,
                    Permissions: this.editRoleForm.value.name
                   }
        console.log(param);
        this.userRoleService.createRole(param).subscribe({
            next: data => {
              console.log('Roles Edited:', JSON.stringify(data));       
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