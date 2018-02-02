import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { isNumeric } from "rxjs/util/isNumeric";

@Component({
    selector: 'filingdata',
    templateUrl: './filingdata.component.html'
})

export class FilingDataComponent {
    public companies: CIKInfo[];
    result: string;

    private companyResult: SECFilingInfo;

    searchClick(searchInput: string)
    {
        if (isNumeric(searchInput))
        {
            this.http.get(this.baseUrl + 'api/SECReport/GetByCIK?cik=' + searchInput).subscribe(result => {
                this.companyResult = result.json() as SECFilingInfo;
            }, error => console.error(error));
            console.log("Search by CIK");
            console.log(this.companyResult);
        }
        else if (this.companies.find(x => x.ticker === searchInput))
        {
            this.http.get(this.baseUrl + 'api/SECReport/GetByTicker?ticker=' + searchInput).subscribe(result => {
                this.companyResult = result.json() as SECFilingInfo;
            }, error => console.error(error));

            console.log("Search by TICKER");
            console.log(this.companyResult);
        }
        else
        {
            console.log("Search by CompanyName=" + searchInput);
        }
    }

    constructor(public http: Http, @Inject('BASE_URL') public baseUrl: string) {
        http.get(baseUrl + 'api/SECReport/GetCompanies').subscribe(result => {
            this.companies = result.json() as CIKInfo[];
        }, error => console.error(error));
    }
}

export interface Filing
{
    new(): Filing;

    AccessionNunber: string;
    fileNumber: string;
    fileNumberHref: string;
    FilingDate: string;
    FilingHref: string;
    FilingType: string;
    DownloadReportPath: string;
    DownloadStatus: string;
}

export interface CIKInfo
{
    cik: number;
    ticker: string;
    name: string;
    exchange: string;
    sic: string;
    business: string;
    incorportated: string;
    irs: string;
}

export interface SECFilingInfo
{
    Id: string;
    CompanyInfo: CIKInfo;
    Filings: Filing[];
}