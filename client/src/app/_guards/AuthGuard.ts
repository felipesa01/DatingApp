import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const AuthGuard: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot) => {

    const accountService = inject(AccountService);
    const toastr = inject(ToastrService);


    return accountService.curentUser$.pipe(
      map(user => {
        if (user) return true;
        else {
          toastr.error('You shall not pass!');
          return false;
        }
      })
    );
  }
