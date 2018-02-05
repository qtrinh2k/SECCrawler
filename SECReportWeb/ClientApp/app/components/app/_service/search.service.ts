import { Component, Inject, Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { CIKInfo, SECFilingInfo } from "../../../app.shared.module";
@Injectable()

export class SearchService {
    public companies: CIKInfo[];
    public searchResults: string[];


    searchStock(searchInput: string): Observable<string[]> {
        var results = this.http.get(this.baseUrl + 'api/SECReport/SearchStock?term=' + searchInput)
            .map((r: Response) => {
                return r.json()
            });       

        return results;
    }

    constructor(public http: Http, @Inject('BASE_URL') public baseUrl: string) {
        http.get(baseUrl + 'api/SECReport/GetCompanies').subscribe(result => {
            this.companies = result.json() as CIKInfo[];
        }, error => console.error(error));
    }
} 