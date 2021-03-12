import { Observable } from 'rxjs';
import { ConfirmModalComponent } from './../modals/confirm-modal/confirm-modal.component';
import { Message } from 'src/app/_models/message';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  bsModalRef: BsModalRef
  constructor(private modalService: BsModalService) { }

  confirm(title = 'Confirmation', message = 'Are you sure, you want to do this ?',
    btnOktext = 'OK',
    btnCancelText = 'Cancel'): Observable<boolean> {

    const config = {
      initialState: {
        title,
        message,
        btnOktext,
        btnCancelText,
      }
    }
    this.bsModalRef = this.modalService.show(ConfirmModalComponent, config);

    return new Observable<boolean>(this.getResult());
  }

  private getResult() {
    return (observer) => {

      const subscription = this.bsModalRef.onHidden.subscribe(() => {
        observer.next(this.bsModalRef.content.result);
        observer.complete();
      })

      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }
}
