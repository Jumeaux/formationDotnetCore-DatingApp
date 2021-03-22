import { User } from 'src/app/_models/user';
import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';


@Component({
  selector: 'app-role-modal',
  templateUrl: './role-modal.component.html',
  styleUrls: ['./role-modal.component.css']
})
export class RoleModalComponent implements OnInit {
  
  @Input() public updateSelectedRoles = new EventEmitter<any>();
  
  user:User;
  roles:any[];

  constructor( public bsModalRef:BsModalRef) { }

  ngOnInit(): void {
    
  }

  updateRole(){
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
