import { TestBed } from '@angular/core/testing';

import { PublicClinicService} from './public-clinic.service';

describe('PublicClinic', () => {
  let service: PublicClinicService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PublicClinicService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
