import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../core';
import { Router, ActivatedRoute } from '@angular/router';
import { finalize, Subscription } from 'rxjs';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css'],
})

export class SigninComponent implements OnInit {
  signinForm: FormGroup;
  submitted = false;
  loginError = false;
  busy = false;
  errorMessage ='';
  private subscription: Subscription | null = null;
  
  get f(): { [key: string]: AbstractControl } {
    return this.signinForm.controls;
  }

  constructor( public fb: FormBuilder, public authService: AuthService,  private router: Router,  public route: ActivatedRoute) 
  {
    this.signinForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [ Validators.required]],      
    });
  }

  ngOnInit(): void {
    this.subscription = this.authService.user$.subscribe((x) => {
      if (this.route.snapshot.url[0].path === 'login') {
        const accessToken = localStorage.getItem('access_token');
        const refreshToken = localStorage.getItem('refresh_token');
        if (x && accessToken && refreshToken) {
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
          this.router.navigate([returnUrl]);
         
        }
      } // optional touch-up: if a tab shows login page, then refresh the page to reduce duplicate login
    });
  }
  

  loginUser() 
  {
    this.submitted = true;
    if (this.signinForm.invalid) { return; }
    
    const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
    this.authService
      .login(this.signinForm.value.username, this.signinForm.value.password)
      .pipe(finalize(() => (this.busy = false)))
      .subscribe(
        () => {
          if(returnUrl == null || returnUrl =='')
          {
            this.router.navigate(['dashboard']); 
          }
          else { this.router.navigate([returnUrl]); }
        },
        () => {
          this.loginError = true;
          this.errorMessage = 'Authentication Failed..Please check username or password';
        }
      );
  }
  reloadPage(): void {
    window.location.reload();
  }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

}
