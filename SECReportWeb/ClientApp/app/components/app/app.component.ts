import { Component, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { SearchService } from "./_service/search.service";

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    public stockSearch: Observable<any[]>;
    private searchTerm = new Subject<string>();
    public ticker: string = "";
    public flag: boolean = true;

    constructor(
        private stockSearchService: SearchService
    ) { }

    ngOnInit(): void {
        this.stockSearch = this.searchTerm
            .debounceTime(300)
            .distinctUntilChanged()
            .switchMap(term => term
                ? this.stockSearchService.searchStock(term)
                : Observable.of<any[]>([]))
            .catch(error => {
                console.log(error);
                return Observable.of<any[]>([]);
            });    
    }
    searchClick(term: string): void {
        this.flag = true;
        this.searchTerm.next(term);
    }

    onselectCompany(stockObj: any)
    {
        if (!stockObj)
        {
            console.log("Empty Object");
            return false;
        }
        else
        {
            this.searchTerm = stockObj;
            this.flag = false;
        }
        
    }
}
