import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { isNumeric } from "rxjs/util/isNumeric";
import { CIKInfo, SECFilingInfo } from "../../app.shared.module";
import { Subscription } from 'rxjs/Subscription';
import { Location } from "@angular/common";
@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    public filingresults: SECFilingInfo[];
    result: string;

    constructor(public http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        private location: Location) {
        http.get(baseUrl + 'api/SECReport/GetLatestCompanyFiling').subscribe(result => {
            this.filingresults = result.json() as SECFilingInfo[];
        }, error => console.error(error));

    }
}
