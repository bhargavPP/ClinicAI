import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Confirm } from './confirm.component';

describe('Confirm', () => {
  let component: Confirm;
  let fixture: ComponentFixture<Confirm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Confirm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Confirm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
