import { Component, Inject, OnInit } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Dependencies, Dependency, DepUpdate, DepUpdateModel } from '../../services/models'
import { SquidService } from '../../services/squid-service'
import { FormBuilder } from '@angular/forms'

@Component({
  selector: 'app-dependencies',
  templateUrl: './dependencies.component.html'
})
export class DependenciesComponent implements OnInit {
  public dependencies: Dependencies[]
  public depUpdates: { [id: string]: DepUpdateModel } = {}

  constructor(private squidService: SquidService, private formBuilder: FormBuilder) {}

  ngOnInit() {
    this.squidService.getDependencies().subscribe(
      result => {
        this.dependencies = result
        this.dependencies.sort(this.sortAlphabetically)
        this.dependencies.forEach(dep => {
          dep.hidden = true,
          dep.packageDependencies = dep.packageDependencies.sort(this.sortPackagesAlphabetically)
        })
      },
      error => console.error(error)
    )
  }

  isUpdateAvailableForRepo(dependency: Dependencies): boolean {
    let updateAvailable = false
    dependency.packageDependencies.forEach(element => {
      if (
        element.version &&
        element.latestVersion &&
        element.version !== element.latestVersion &&
        element.latestVersion !== ''
      ) {
        updateAvailable = true
      }
    })

    return updateAvailable
  }

  isUpdateAvailable(currentVersion: string, latestVersion: string): boolean {
    if (currentVersion && latestVersion) {
      return currentVersion === latestVersion || latestVersion === '' ? false : true
    }
    return false
  }

  isUpdateAvailableString(currentVersion: string, latestVersion: string): string {
    if (currentVersion && latestVersion) {
      return currentVersion === latestVersion || latestVersion === '' ? 'no' : 'yes'
    }
    return 'no'
  }

  toggleDisplay(dep: Dependencies): void {
    dep.hidden = !dep.hidden
  }

  toggleSelection(dep: Dependencies, pd: Dependency): void {
    if (this.depUpdates[dep.repository.id]) {
      const depUpdate = this.depUpdates[dep.repository.id]

      if (depUpdate.updates != null) {
        if (depUpdate.updates.some(u => u.packageId === pd.packageId)) {
          // Package is already in list, remove.
          depUpdate.updates = depUpdate.updates.filter(u => u.packageId !== pd.packageId)

          if (!depUpdate.updates.some(u => true)) {
            // If there are no updates left, remove the repo.
            this.depUpdates[dep.repository.id] = null
          }
        } else {
          depUpdate.updates.push({
            packageId: pd.packageId,
            version: pd.latestVersion
          })
        }
      }
    } else {
      const depUpdate = {
        repositoryId: dep.repository.id,
        updates: new Array<DepUpdate>()
      }
      depUpdate.updates.push({
        packageId: pd.packageId,
        version: pd.latestVersion
      })

      this.depUpdates[dep.repository.id] = depUpdate
    }
  }

  updateDependencies(dep: Dependencies): void {
    if (this.depUpdates[dep.repository.id]) {
      const depUpdate = this.depUpdates[dep.repository.id]

      this.squidService.updateDependencies(depUpdate).subscribe()
    }
  }

  private sortAlphabetically(n1: Dependencies, n2: Dependencies): number {
    if (n1 === null || n2 === null || n1.repository === null || n2.repository === null) {
      return 0
    }
    if (n1.repository.name.toLowerCase() < n2.repository.name.toLowerCase()) {
      return -1
    }
    if (n1.repository.name.toLowerCase() > n2.repository.name.toLowerCase()) {
      return 1
    }
    return 0
  }

  private sortPackagesAlphabetically(n1: Dependency, n2: Dependency): number {
    if (n1 === null || n2 === null) {
      return 0
    }
    if (n1.packageId.toLowerCase() < n2.packageId.toLowerCase()) {
      return -1
    }
    if (n1.packageId.toLowerCase() > n2.packageId.toLowerCase()) {
      return 1
    }
    return 0
  }
}
