import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { QuoteComponent } from './components/quote/quote.component';
import { QuoteService } from './service/quote.service';

@NgModule({
  declarations: [
    QuoteComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    CommonModule,
    FormsModule,
     ReactiveFormsModule

  ],
  providers: [QuoteService],
  bootstrap:[QuoteComponent]
})
export class AppModule { }
