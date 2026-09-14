import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { UnsureComponent } from './components/unsure/unsure.component';
import { UnsureService } from './service/unsure.service';

@NgModule({
  declarations: [
    UnsureComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    CommonModule,
    FormsModule,
     ReactiveFormsModule

  ],
  providers: [UnsureService],
  bootstrap:[UnsureComponent]
})
export class AppModule { }
