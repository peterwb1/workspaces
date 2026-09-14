import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnsureComponent } from './components/unsure/unsure.component';

const routes: Routes = [
  { path: '', component: UnsureComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
