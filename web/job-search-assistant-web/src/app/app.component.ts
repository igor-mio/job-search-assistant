import { CommonModule } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Component, inject } from "@angular/core";
import { AuthService } from "@auth0/auth0-angular";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="p-6 max-w-2xl mx-auto">
      <h1 class="text-2xl font-bold mb-4">Job Search Assistant</h1>

      <div class="flex gap-2 mb-4">
        <button class="px-4 py-2 rounded bg-black text-white" (click)="login()">
          Login
        </button>
        <button class="px-4 py-2 rounded border" (click)="logout()">
          Logout
        </button>
        <button class="px-4 py-2 rounded bg-blue-600 text-white" (click)="callMe()">
          Call /api/me
        </button>
        <button class="px-4 py-2 rounded bg-purple-600 text-white" (click)="callAdminPing()">
        Call /api/admin/ping
        </button>
        <button class="px-4 py-2 rounded bg-emerald-600 text-white" (click)="callOwnershipTest()">
        Call /api/ownership/test (self)
        </button>
      </div>

      <div class="text-sm text-gray-600 mb-2" *ngIf="(auth.isAuthenticated$ | async) === false">
        Not authenticated
      </div>
      <div class="text-sm text-green-700 mb-2" *ngIf="(auth.isAuthenticated$ | async) === true">
        Authenticated ✅
      </div>

      <pre class="bg-gray-100 p-3 rounded overflow-auto text-xs" *ngIf="result">{{ result | json }}</pre>
    </div>
  `
})
export class AppComponent {
    auth = inject(AuthService);
    private http = inject(HttpClient);

    result: any;

    login() {
        this.auth.loginWithRedirect();
    }
    logout() {
        this.auth.logout({ logoutParams: { returnTo: window.location.origin } })
    }
    callMe() {
        this.http.get('/api/me').subscribe({
            next: (r) => (this.result = r),
            error: (e) => (this.result = e)
        });
    }
    callAdminPing() {
        this.http.get('/api/rbac/admin/ping').subscribe({
            next: (r) => (this.result = r),
            error: (e) => (this.result = e)
        });
    }

    callOwnershipTest() {
        // on teste avec TON sub (copie celui renvoyé par /api/me)
        const mySub = 'google-oauth2|113077350299733476401';
        this.http.get(`/api/rbac/ownership/test/${encodeURIComponent(mySub)}`).subscribe({
            next: (r) => (this.result = r),
            error: (e) => (this.result = e)
        });
    }
}