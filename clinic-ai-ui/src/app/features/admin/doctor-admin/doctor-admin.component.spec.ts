import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorAdminComponent } from './doctor-admin.component';

describe('DoctorAdminComponent', () => {
  let component: DoctorAdminComponent;
  let fixture: ComponentFixture<DoctorAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DoctorAdminComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
