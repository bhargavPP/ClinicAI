import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClinicPublic } from './clinic-public.component';

describe('ClinicPublic', () => {
  let component: ClinicPublic;
  let fixture: ComponentFixture<ClinicPublic>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClinicPublic]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClinicPublic);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
