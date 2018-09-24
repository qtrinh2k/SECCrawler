import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { isNumeric } from "rxjs/util/isNumeric";
import { CIKInfo, SECFilingInfo } from "../../app.shared.module";
import { Subscription } from 'rxjs/Subscription';
import { Location } from "@angular/common";
import { PagerService } from "../services/PagerService";
@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    public filingresults: SECFilingInfo[];
    result: string;

    //constructor(public http: Http,
    //    @Inject('BASE_URL') public baseUrl: string,
    //    private location: Location,
    //    private pageService: PagerService) {
    //    http.get(baseUrl + 'api/SECReport/GetLatestCompanyFiling').subscribe(result => {
    //        this.filingresults = result.json() as SECFilingInfo[];
    //    }, error => console.error(error));

    //    this.setPage(1);
    //}

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/SECReport/GetLatestCompanyFiling').subscribe(result => {
            this.filingresults = result.json() as SECFilingInfo[];
            console.log("zzzlen" + this.filingresults.length);
        }, error => console.error(error));
    }
    pager: any = {};
    pagerItems: any[];

    setPage(page: number)
    {
        if (page < 1 || page > this.pager.totalPage)
            return;
        console.log("zzzlen" + this.filingresults.length);

        //this.pager = this.pageService.getPager(this.filingresults.length, page);

        //this.filingresults.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    getCount()
    {
        return this.filingresults.forEach(x => x.CompanyInfo.name);
    }
}
