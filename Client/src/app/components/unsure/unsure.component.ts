import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { quoteResponse } from 'src/app/Model/QuoteResponse';
import { UnsureService } from 'src/app/service/unsure.service';

@Component({
  selector: 'unsure',
  templateUrl: './unsure.component.html',
  styleUrls: ['./unsure.component.scss'],
  standalone: false
})
export class UnsureComponent implements OnInit {

  constructor(private unsureService: UnsureService ) {
    this.touched = false;
   }

  touched: boolean;
  details: quoteDetail = new quoteDetail();
  Models: Array<string> = [];
  Quote: number= 0;
  QuoteReceived: boolean = false; 

  ngOnInit(): void {
    this.unsureService.getDetails<quoteDetail>().subscribe(s=>{
      this.details.makes = s.makes;
      this.details.models = s.models;
      this.details.insuranceTypes = s.insuranceTypes;
      this.setModels();
    });
  }

  
  dateOfBirthCtrl= new FormControl('', Validators.required);
  makeCtrl = new FormControl('',Validators.required);
  modelCtrl = new FormControl('',Validators.required);
  insuranceTypeCtrl = new FormControl('',Validators.required);
  
  quoteForm = new FormGroup({
    dateOfBirth: this.dateOfBirthCtrl,
    make: this.makeCtrl,
    model: this.modelCtrl,
    insuranceType: this.insuranceTypeCtrl,
  });

  public changeMake(e: any) {
    this.quoteForm.controls["make"].setValue(e.target.value, {  onlySelf: true  })
    this.setModels();
  }

  public changeModel(e: any) {
    this.quoteForm.controls["model"].setValue(e.target.value, {  onlySelf: true  })
  }

  public changeInsuranceType(e: any) {
    this.quoteForm.controls["insuranceType"].setValue(e.target.value, {  onlySelf: true  })
  }

  setModels(){
    let currentMake = this.quoteForm.controls["make"].value;
    this.Models = [];
    let makes = this.details.models.filter(function(m){return m.make == currentMake});
    if (makes.length > 0)
      this.Models = makes[0].models;
  }

  GetQuote()
  {
    this.touched = true;
    if (!this.quoteForm.invalid && this.touched)
    {
      this.unsureService.GetQuote<quoteResponse>(this.quoteForm.value).subscribe(s=>{
        this.QuoteReceived = true;
        this.Quote = s.quote;
        this.setModels();
      });
      }
  }

}
