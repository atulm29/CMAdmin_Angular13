import { Component } from "@angular/core";



@Component({
    templateUrl: './members.component.html',
    styleUrls: ['./members.component.css'] 
})

export class MembersComponent {
    leftnav:string = 'leftnav';

    changeStyle(e: any){
        this.leftnav = e.type == 'mouseover' ? 'leftnav leftnavhover' : 'leftnav';
    }
}