import { BrowserModule } from '@angular/platform-browser'
import { NgModule } from '@angular/core'
import { FormsModule, FormBuilder, ReactiveFormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http'
import { RouterModule } from '@angular/router'

import { AppComponent } from './app.component'
import { NavMenuComponent } from './components/nav-menu/nav-menu.component'
import { HomeComponent } from './components/home/home.component'
import { RepositoriesComponent } from './components/repositories/repositories.component'
import { DependenciesComponent } from './components/dependencies/dependencies.component'
import { NugetFeedsComponent } from './components/nugetfeeds/nugetfeeds.component'
import { SquidService } from './services/squid-service'
import { SettingsComponent } from './components/settings/settings.component'

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    RepositoriesComponent,
    DependenciesComponent,
    NugetFeedsComponent,
    SettingsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'repositories', component: RepositoriesComponent },
      { path: 'dependencies', component: DependenciesComponent },
      { path: 'nugetfeeds', component: NugetFeedsComponent },
      { path: 'settings', component: SettingsComponent }
    ])
  ],
  providers: [SquidService],
  bootstrap: [AppComponent]
})
export class AppModule {}
