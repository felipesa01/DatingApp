import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public AccountService: AccountService, private router: Router, private toastr: ToastrService) {

  };

  ngOnInit() {

  }


  login() {

    this.AccountService.login(this.model).subscribe({
      next: _ => this.router.navigateByUrl("/members"),
      error: error => {
        console.log(error);
        this.toastr.error(error.error);
      }
    })

  }

  logout() {
    this.AccountService.logout();
    this.model = {};
    this.router.navigateByUrl("/");
  }
  
}
