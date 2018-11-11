export interface RepositoryConfig {
  id?: string
  name: string
  teamName: string
  repoSlug: string
  branch: string
  username: string
  projectFile: string
  password: string
  valid: boolean
  errorMessage: string
}

export interface NugetFeed {
  id?: string
  name: string
  feedURL: string
  apiKeyHeaderName?: string
  apiKey: string
  valid: boolean
  errorMessage: string
}

export interface Dependencies {
  packageDependencies: Dependency[]
  repository: RepositoryConfig
  hidden: boolean
}

export interface Dependency {
  packageId: string
  version: string
  latestVersion: string
  feedName: string
  feedId: string
}

export interface DepUpdateModel {
  repositoryId: string
  updates: Array<DepUpdate>
}

export interface DepUpdate {
  packageId: string
  version: string
}
