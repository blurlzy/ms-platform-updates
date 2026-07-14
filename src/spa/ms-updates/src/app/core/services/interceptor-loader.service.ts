import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpStatusCode } from '@angular/common/http';
import { Observable, finalize } from 'rxjs';

// services
import { Loader } from './loader.service';

// show loader on http request, hide once all the requests are completed
@Injectable({ providedIn: 'root' })
export class LoaderInterceptor implements HttpInterceptor {
   // handle multiple parallel http requests
   private count = 0;
   //ctor
   constructor(public loader: Loader) {

   }

   // show loader on http request
   intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      // avoid error: NG0100: Expression has changed after it was checked
      setTimeout(() => {
         this.loader.isLoading.next(true);
       });

      this.count++;

      return next.handle(req).pipe(
         finalize(() => {
            this.count--;
            // hide loader when the last request is completed
            if (this.count < 1) {
               this.loader.isLoading.next(false);
            }
         })
      );
   }
}