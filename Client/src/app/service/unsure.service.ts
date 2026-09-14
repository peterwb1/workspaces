import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })
  
export class UnsureService {
  
    constructor( private httpClient: HttpClient ) 
    { }
  
    getDetails<t>() {
        return this.httpClient.get<t>("http://localhost:5000/Quote");
    
    }

    GetQuote<t>(data: any) {
      return this.httpClient.post<t>("http://localhost:5000/Quote", data);
  
  }
  }