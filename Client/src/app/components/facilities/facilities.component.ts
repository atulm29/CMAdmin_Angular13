import { Component, Injectable, OnInit } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { OrganizationDropDownData } from "src/app/models/Organization";
import { PagingAccess } from "src/app/models/pagingaccess";
import { tableHeader, tableRowData } from "src/app/models/tabledatamodel";
import { FacilitiesService } from "src/app/services/facilities.service";
import { OrganizationService } from "src/app/services/organization.service";


@Component({
    templateUrl: './facilities.component.html',
    styleUrls: ['./facilities.component.css']
})

export class Facilities implements OnInit {
    pageTilte: String = 'Facilities';
    OrganizationDropDownList: OrganizationDropDownData[] = [];
    TableHeaderData: tableHeader[] = [];  
    TableRowData: tableRowData[] = []; 
    currentIndex = -1;
    title = '0';
    totalRecordsText: string ='';
    page = 1;
    count = 0;
    pageSize = 10;
    pageSizes = [10, 25, 50, 100, 200, 500, 1000];
    pagingAccessList : PagingAccess[] = [];
    selectedPageAccess = 1;
     constructor(private facilitiesService:FacilitiesService, private organizationService:OrganizationService, public fb: FormBuilder) {}
       
     ngOnInit() {
      this.retrieveFacilitiesData();
      this.getFacilityDropDownListData();
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
    
    retrieveFacilitiesData(setPageAccessList: boolean = true): void 
    {
  
      const params = this.getRequestParams(this.title, this.page, this.pageSize);
  
      this.facilitiesService.getAll(params)
        .subscribe({
          next: (data) => {
            console.log("Facilities Page: " + data.result);
           // console.log('pagingAccessList: ' + setPageAccessList);
            this.TableHeaderData = data.result.tableData.tableHeader;
            this.TableRowData = data.result.tableData.tableRowData;
            this.count = data.paging.totalItems;  
            if(setPageAccessList == true)
            {
              this.pagingAccessList = data.paging.pageList;
            }
            this.totalRecordsText = data.paging.totalRecordsText;   
          },
          error: (err) => {
            console.log(err);
          }
        });
    }
    
    handlePageChange(event: number): void {
      this.page = event;
      this.selectedPageAccess = event;
      this.retrieveFacilitiesData(false);
    }
  
    onChange(e: any) {
      console.log(JSON.stringify(this.selectedPageAccess));
      this.page = e.target.value;
      this.retrieveFacilitiesData(false);
    }
    
    handlePageSizeChange(event: any): void {
      this.pageSize = event.target.value;
      this.page = 1;
      this.retrieveFacilitiesData();
    } 
  
    searchTitle(): void {
      this.page = 1;
      this.retrieveFacilitiesData();
    }
  
    getFacilityDropDownListData(): void 
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
      this.retrieveFacilitiesData();
    }
}