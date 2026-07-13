import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DataService {
      // api endpoint
	private apiEndpoint = `${environment.apiEndpoint}/api/ms-updates`;

      // ctor
      constructor(private http: HttpClient) {}

      // get all updates
      getAllUpdates(source:string,pageIndex: number, pageSize: number): Observable<any> {
            return this.http.get(`${this.apiEndpoint}?source=${source}&pageIndex=${pageIndex}&pageSize=${pageSize}`);
      }
}
