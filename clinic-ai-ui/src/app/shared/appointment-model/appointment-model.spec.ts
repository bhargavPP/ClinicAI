import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentModel } from './appointment-model';

describe('AppointmentModel', () => {
  let component: AppointmentModel;
  let fixture: ComponentFixture<AppointmentModel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppointmentModel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentModel);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
