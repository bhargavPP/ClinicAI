import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Patient } from './patient';

describe('Patient', () => {
  let component: Patient;
  let fixture: ComponentFixture<Patient>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Patient]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Patient);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
