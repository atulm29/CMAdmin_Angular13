import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DashboardService } from 'src/app/services/dashboard.service';


@Component({
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  adminTiles: any;

  constructor( public dashboardService: DashboardService,  private actRoute: ActivatedRoute) 
  {
    this.dashboardService.getDashboardData().subscribe((res) => {
      console.log('get DashboardData:', JSON.stringify(res.result));
      this.adminTiles = res.result;
    });
  }



  ngOnInit(): void { 
  }
}
