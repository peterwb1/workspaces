import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UnsureComponent } from './unsure.component';


describe('UnsureComponent', () => {
  let component: UnsureComponent;
  let fixture: ComponentFixture<UnsureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UnsureComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UnsureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
