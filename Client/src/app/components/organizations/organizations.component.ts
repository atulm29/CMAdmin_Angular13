import { Component, Injectable, OnInit } from "@angular/core";
import { AbstractControl, FormBuilder, FormGroup, Validators } from "@angular/forms";
import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';
import { OrganizationDropDownData, OrganizationTableData, OrganizationTableHeader } from "src/app/models/Organization";
import { PagingAccess } from "src/app/models/pagingaccess";
import { OrganizationService } from "src/app/services/organization.service";

@Component({
  templateUrl: './organizations.component.html',
  styleUrls: ['./organizations.component.css']
})
export class Organizations implements OnInit {
  THData: OrganizationTableHeader = {};
  OrganizationDropDownList: OrganizationDropDownData[] = [];
  OrganizationList: OrganizationTableData[] = [];
  currentOrganization: OrganizationTableData = {};
  currentIndex = -1;
  title = '0';
  totalRecordsText: string ='';
  page = 1;
  count = 0;
  pageSize = 10;
  pageSizes = [10, 25, 50, 100, 200, 500, 1000];
  pagingAccessList : PagingAccess[] = [];
  selectedPageAccess = 1;
   constructor(private organizationService:OrganizationService, public fb: FormBuilder, public modalService: NgbModal) 
   {
    this.orgnizationForm = this.fb.group({
      groupName: ['', [Validators.required]],
      organizationType: [''],      
    });
   }
     
   ngOnInit() {
    this.retrieveOrganizationData();
    this.getOrganizationDropDownListData();
    this.organizationSucessMsg ='';
  }
  getRequestParams(searchTitle: string, page: number, pageSize: number): any {
    let params: any = {};

    if (searchTitle) {
      params[`SearchTitle`] = searchTitle;
    }

    if (page) {
      params[`PageNumber`] = page;
    }

    if (pageSize) {
      params[`PageSize`] = pageSize;
    }

    return params;
  }
  
  retrieveOrganizationData(setPageAccessList: boolean = true): void 
  {

    const params = this.getRequestParams(this.title, this.page, this.pageSize);

    this.organizationService.getAll(params)
      .subscribe({
        next: (data) => {
          //console.log(data.result);
         // console.log('pagingAccessList: ' + setPageAccessList);
          this.OrganizationList = data.result.rowResults;
          this.count = data.paging.totalItems;  
          if(setPageAccessList == true)
          {
            this.pagingAccessList = data.paging.pageList;
          }
          this.totalRecordsText = data.paging.totalRecordsText;
          this.THData.srNo = data.result.srNo;
          this.THData.groupConfigLabel =  data.result.groupConfigLabel;  
          this.THData.instituteLabelConfig =  data.result.instituteLabelConfig;  
          this.THData.instructorLabelConfig =  data.result.instructorLabelConfig;  
          this.THData.subScribeStudentLabelConfig =  data.result.subScribeStudentLabelConfig; 
          this.THData.trialStudentLabelConfig =  data.result.trialStudentLabelConfig;    
        },
        error: (err) => {
          console.log(err);
        }
      });
  }
  
  handlePageChange(event: number): void {
    this.page = event;
    this.selectedPageAccess = event;
    this.retrieveOrganizationData(false);
  }

  onChange(e: any) {
    console.log(JSON.stringify(this.selectedPageAccess));
    this.page = e.target.value;
    this.retrieveOrganizationData(false);
  }
  
  handlePageSizeChange(event: any): void {
    this.pageSize = event.target.value;
    this.page = 1;
    this.retrieveOrganizationData();
  }

  refreshList(): void {
    this.retrieveOrganizationData();
    this.currentOrganization = {};
    this.currentIndex = -1;
  }

  setActiveOrganization(organization: OrganizationTableData, index: number): void {
    this.currentOrganization = organization;
    this.currentIndex = index;
  }

  removeAllOrganizations(): void {
    this.organizationService.deleteAll()
      .subscribe({
        next: res => {
          console.log(res);
          this.refreshList();
        },
        error: err => {
          console.log(err);
        }
      });

  }

  searchTitle(): void {
    this.page = 1;
    this.retrieveOrganizationData();
  }

  getOrganizationDropDownListData(): void 
  {

    this.organizationService.getOrganizations()
      .subscribe({
        next: (data) => {
          //console.log(data.result);
          this.OrganizationDropDownList = data.result;   
        },
        error: (err) => {
          console.log(err);
        }
      });
  }


  OrgDropDownListForm = this.fb.group({
    name: ['']
  })
  onSubmit() {
    this.page = 1;
    this.retrieveOrganizationData();
  }

  
  closeResult: string = '';
  open(content:any) 
  {
    this.isOrganizationFailed = false;
    this.submitted = false;
    this.orgnizationForm.patchValue({
      groupName:'',
      organizationType: ''        
    });
    let options: any = {
      size: "dialog-centered",
      ariaLabelledBy: 'modal-basic-title'
    };
    this.modalService.open(content, options).result.then((result) => {
      this.closeResult = `Closed with: ${result}`;
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  } 

  closePupUp()
  {
    this.isOrganizationFailed = false;
    this.submitted = false;
    this.modalService.dismissAll('Cross click');
  }
  
  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return  `with: ${reason}`;
    }
  }

  /* Add Organization */
  onCheckboxChange(e: any) {    
    if (e.target.checked) {
      this.orgnizationForm.patchValue({
        organizationType: 'Indesign'        
      });
    } else {
      this.orgnizationForm.patchValue({
        organizationType: ''        
      });
    }
  }
  get orgF(): { [key: string]: AbstractControl } {
    return this.orgnizationForm.controls;
  }
  orgnizationForm: FormGroup;
  submitted = false;
  errorMessage = '';
  organizationSucessMsg = '';
  isOrganizationFailed = false;
  addOrganization() {
    this.submitted = true;
    if (this.orgnizationForm.invalid) {
        return;
    }
    //alert('organization Input: ' + JSON.stringify(this.orgnizationForm.value)); 
    this.organizationService.create(this.orgnizationForm.value).subscribe({
      next: data => {
        console.log('New Organization added:', JSON.stringify(data));       
        if(data.status == 0)
        {
          this.isOrganizationFailed = true;         
          this.page = 1;
          this.retrieveOrganizationData();
          this.organizationSucessMsg = data.message;
          this.modalService.dismissAll('Cross click');
        }
        else if (data.status == -1 || data.status == -2)
        {
          this.errorMessage = data.Error.Message;
        }
      },
      error: err => {
        this.errorMessage = 'Add Organization Failed.';        
      }
    });
  }
}