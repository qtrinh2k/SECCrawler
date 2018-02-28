import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { isNumeric } from "rxjs/util/isNumeric";
import { CIKInfo, SECFilingInfo } from "../../app.shared.module";
import { Subscription } from 'rxjs/Subscription'; 
import {Location} from "@angular/common";

@Component({
    selector: 'filingdata',
    templateUrl: './filingdata.component.html'
})

export class FilingDataComponent {
    public companies: CIKInfo[];
    public companyresults: SECFilingInfo[];
    result: string;

    constructor(public http: Http, 
                @Inject('BASE_URL') public baseUrl: string, 
                private location: Location) 
    {
        http.get(baseUrl + 'api/SECReport/GetCompanies').subscribe(result => {
            this.companies = result.json() as CIKInfo[];
        }, error => console.error(error));

    }

    searchClick(searchInput: string)
    {
        this.http.get(this.baseUrl + 'api/SECReport/GetFiling?query=' + searchInput).subscribe(result => {
            this.companyresults = result.json() as SECFilingInfo[];
        }, error => console.error(error));
        console.log("SearchInput=" + searchInput);
        console.log(this.companyresults);
    }
}

