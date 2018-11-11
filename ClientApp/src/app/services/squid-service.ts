import { Injectable, Inject } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs/Observable'
import { RepositoryConfig, NugetFeed, Dependencies, DepUpdateModel } from './models'

@Injectable()
export class SquidService {
  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

  /**
   * Repositories
   */

  getRepositoryConfigs(): Observable<RepositoryConfig[]> {
    return this.httpClient.get<RepositoryConfig[]>(this.baseUrl + 'api/repositoryconfig')
  }

  addRepositoryConfig(repoConfig: RepositoryConfig): Observable<RepositoryConfig> {
    return this.httpClient.post<RepositoryConfig>(this.baseUrl + 'api/repositoryconfig', repoConfig)
  }

  updateRepositoryConfig(repoConfig: RepositoryConfig): Observable<RepositoryConfig> {
    return this.httpClient.put<RepositoryConfig>(this.baseUrl + `api/repositoryconfig/${repoConfig.id}`, repoConfig)
  }

  deleteRepositoryConfig(repoConfig: RepositoryConfig): Observable<void> {
    return this.httpClient.delete<void>(this.baseUrl + `api/repositoryconfig/${repoConfig.id}`)
  }

  /**
   * NuGet Feeds
   */

  getNuGetFeeds(): Observable<NugetFeed[]> {
    return this.httpClient.get<NugetFeed[]>(this.baseUrl + 'api/nugetfeedconfig')
  }

  addNuGetFeed(feedConfig: NugetFeed): Observable<NugetFeed> {
    return this.httpClient.post<NugetFeed>(this.baseUrl + 'api/nugetfeedconfig', feedConfig)
  }

  updateNuGetFeed(feedConfig: NugetFeed): Observable<NugetFeed> {
    return this.httpClient.put<NugetFeed>(this.baseUrl + `api/nugetfeedconfig/${feedConfig.id}`, feedConfig)
  }

  deleteNugetFeed(feedConfig: NugetFeed): Observable<void> {
    return this.httpClient.delete<void>(this.baseUrl + `api/nugetfeedconfig/${feedConfig.id}`)
  }

  /**
   * Dependencies
   */

  getDependencies(): Observable<Dependencies[]> {
    return this.httpClient.get<Dependencies[]>(this.baseUrl + 'api/dependencies')
  }

  updateDependencies(depUpdate: DepUpdateModel): Observable<void> {
    return this.httpClient.post<void>(this.baseUrl + 'api/dependencies', depUpdate)
  }
}
