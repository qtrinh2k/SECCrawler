import { Component, Inject, Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { CIKInfo, SECFilingInfo } from "../../../app.shared.module";
@Injectable()

export class ClientSearchService {
    endPoint: string;
    result: string;
    public companies: CIKInfo[];
    public stockResults: string[];


    searchClick(searchInput: string): Observable<any[]> {
        var results = this.http.get(this.baseUrl + 'api/SECReport/SearchTicker?term=' + searchInput)
            .map((r: Response) => {
                return r.json()
            });

        return results;
    }

    search(term: string): Observable<any[]> {
        var ClientList = this.http.get(this.endPoint + '?term=' + term)
            .map((r: Response) => { return (r.json().length != 0 ? r.json() : [{ "ClientId": 0, "ClientName": "No Record Found" }]) as any[] });
        return ClientList;
    }

    constructor(public http: Http, @Inject('BASE_URL') public baseUrl: string) {
        http.get(baseUrl + 'api/SECReport/GetCompanies').subscribe(result => {
            this.companies = result.json() as CIKInfo[];
        }, error => console.error(error));
    }
} 