import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent,HttpInterceptor} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private toast: ToastrService, private route: Router) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          
          switch (error.status) {
            case 400:
              if (error.error.errors) {

                const modalStateErrors = [];
                for (const key in error.error.errors) {

                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key])
                  }
                }
                throw modalStateErrors;
              } else {
                this.toast.error(error.statusText, error.status)  
              }

              break;
            case 401:
              this.toast.error(error.statusText, error.satatus)
              break;
            case 404:
              this.route.navigateByUrl('not-found')
            break;
            case 500:
              const navigateExtras: NavigationExtras = { state: { error: error.error } }
              this.route.navigateByUrl('/server-error', navigateExtras)
              break;
            default:
              this.toast.error('something unexpected went wrong');
              break;
          }
        }

        return throwError(error);
      })
    );
  }
}
