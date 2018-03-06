import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { FilingDataComponent } from './components/filingdata/filingdata.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        FilingDataComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'app', component: AppComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'filing-data', component: FilingDataComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}

export interface Filing {
    new (): Filing;

    AccessionNunber: string;
    fileNumber: string;
    fileNumberHref: string;
    FilingDate: string;
    FilingHref: string;
    FilingType: string;
    ReportOriginalUrl: string;
    DownloadReportPath: string;
    DownloadStatus: string;
}

export interface CIKInfo {
    cik: number;
    ticker: string;
    name: string;
    exchange: string;
    sic: string;
    business: string;
    incorportated: string;
    irs: string;
}

export interface SECFilingInfo {
    Id: string;
    CompanyInfo: CIKInfo;
    Filings: Filing[];
}