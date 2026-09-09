import { TestBed } from '@angular/core/testing';
import { CurrentUserService } from './current-user.service';

const STORAGE_KEY = 'fondsSocial.currentAuteur';

describe('CurrentUserService', () => {
  beforeEach(() => {
    localStorage.removeItem(STORAGE_KEY);
    TestBed.configureTestingModule({});
  });

  afterEach(() => localStorage.removeItem(STORAGE_KEY));

  it('starts empty when nothing is stored', () => {
    const service = TestBed.inject(CurrentUserService);
    expect(service.auteur()).toBe('');
  });

  it('trims and persists the auteur to localStorage', () => {
    const service = TestBed.inject(CurrentUserService);
    service.setAuteur('  RH - Nom Prenom  ');
    expect(service.auteur()).toBe('RH - Nom Prenom');
    expect(localStorage.getItem(STORAGE_KEY)).toBe('RH - Nom Prenom');
  });

  it('reflects a value already in localStorage for a fresh instance (page reload)', () => {
    localStorage.setItem(STORAGE_KEY, 'Comité');
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    const service = TestBed.inject(CurrentUserService);
    expect(service.auteur()).toBe('Comité');
  });
});
