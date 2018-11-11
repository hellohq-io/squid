import { Component, Inject, OnInit } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { RepositoryConfig } from '../../services/models'
import { SquidService } from '../../services/squid-service'
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { showFormErrors } from '../../_shared/showFormError'

@Component({
  selector: 'app-repositories',
  templateUrl: './repositories.component.html'
})
export class RepositoriesComponent implements OnInit {
  public repositories: RepositoryConfig[]
  repoForm: FormGroup
  currentRepo: RepositoryConfig
  currentRepoId: string

  formErrors = {
    name: '',
    teamName: '',
    repoSlug: '',
    branch: '',
    username: '',
    projectFile: '',
    password: ''
  }

  validationMessages = {
    name: {
      required: 'The name is required.'
    },
    teamName: {
      required: 'The team name is required.'
    },
    repoSlug: {
      required: 'The repoSlug is required.'
    },
    branch: {
      required: 'The branch is required.'
    },
    username: {
      required: 'The username is required.'
    },
    projectFile: {
      required: 'The projectFile is required.'
    },
    password: {
      required: 'The password is required.'
    }
  }

  constructor(private squidService: SquidService, private formBuilder: FormBuilder) {}

  ngOnInit() {
    this.squidService.getRepositoryConfigs().subscribe(
      result => {
        this.repositories = result
        this.repositories.sort(this.sortAlphabetically)
      },
      error => console.error(error)
    )

    this.buildForm()
  }

  private buildForm(): void {
    this.repoForm = this.formBuilder.group({
      name: ['', Validators.required],
      teamName: ['', Validators.required],
      repoSlug: ['', Validators.required],
      branch: ['', Validators.required],
      username: ['', Validators.required],
      projectFile: ['', Validators.required],
      password: ['', Validators.required]
    })
  }

  saveRepo(): void {
    if (!showFormErrors(this.repoForm, this.formErrors, this.validationMessages)) {
      this.currentRepo = {
        name: this.repoForm.get('name').value,
        teamName: this.repoForm.get('teamName').value,
        branch: this.repoForm.get('branch').value,
        repoSlug: this.repoForm.get('repoSlug').value,
        projectFile: this.repoForm.get('projectFile').value,
        username: this.repoForm.get('username').value,
        password: this.repoForm.get('password').value,
        valid: false,
        errorMessage: ''
      }

      if (!this.currentRepoId) {
        this.squidService.addRepositoryConfig(this.currentRepo).subscribe(repo => {
          this.repositories.push(repo)
          this.repositories.sort(this.sortAlphabetically)

          this.repoForm.reset()
        })
      } else {
        this.currentRepo.id = this.currentRepoId
        this.squidService.updateRepositoryConfig(this.currentRepo).subscribe(repo => {
          this.repositories = this.repositories.filter(r => r.id !== this.currentRepoId)
          this.repositories.push(repo)
          this.repositories.sort(this.sortAlphabetically)

          this.currentRepoId = null
          this.repoForm.reset()
        })
      }
    }
  }

  updateRepoForm(repo: RepositoryConfig): void {
    this.currentRepoId = repo.id
    this.repoForm.get('name').setValue(repo.name)
    this.repoForm.get('teamName').setValue(repo.teamName)
    this.repoForm.get('branch').setValue(repo.branch)
    this.repoForm.get('repoSlug').setValue(repo.repoSlug)
    this.repoForm.get('projectFile').setValue(repo.projectFile)
  }

  deleteRepoForm(repo: RepositoryConfig): void {
    this.squidService.deleteRepositoryConfig(repo).subscribe(() => {
      this.repositories = this.repositories.filter(r => r.id !== repo.id)
    })
  }

  clearForm(): void {
    this.currentRepoId = null
    this.repoForm.reset()
  }

  private sortAlphabetically(n1: RepositoryConfig, n2: RepositoryConfig): number {
    if (n1.name.toLowerCase() < n2.name.toLowerCase()) {
      return -1
    }
    if (n1.name.toLowerCase() > n2.name.toLowerCase()) {
      return 1
    }
    return 0
  }
}
