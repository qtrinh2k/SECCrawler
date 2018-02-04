import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { isNumeric } from "rxjs/util/isNumeric";
import { CIKInfo, SECFilingInfo } from "../../app.shared.module";

@Component({
    selector: 'filingdata',
    templateUrl: './filingdata.component.html'
})

export class FilingDataComponent {
    public companies: CIKInfo[];
    result: string;

    public companyresults: SECFilingInfo[];

    searchClick(searchInput: string)
    {
        this.http.get(this.baseUrl + 'api/SECReport/GetFiling?q=' + searchInput).subscribe(result => {
            this.companyresults = result.json() as SECFilingInfo[];
        }, error => console.error(error));
        console.log("SearchInput=" + searchInput);
        console.log(this.companyresults);
    }

    constructor(public http: Http, @Inject('BASE_URL') public baseUrl: string) {
        http.get(baseUrl + 'api/SECReport/GetCompanies').subscribe(result => {
            this.companies = result.json() as CIKInfo[];
        }, error => console.error(error));
    }
}
