import { BsModalRef } from 'ngx-bootstrap/modal';
import { Message } from 'src/app/_models/message';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.css']
})
export class ConfirmModalComponent implements OnInit {

  title:string;
  message:string;
  btnOktext:string;
  btnCancelText:string;
  result:boolean;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }
  confirm(){
    this.result=true;
    this.bsModalRef.hide();
  }

  cancel(){
    this.result=false;
    this.bsModalRef.hide();
  }

}
