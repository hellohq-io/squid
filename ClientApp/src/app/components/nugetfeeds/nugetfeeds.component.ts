import { Component, Inject, OnInit } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { NugetFeed } from '../../services/models'
import { FormGroup, FormBuilder, Validators } from '@angular/forms'
import { SquidService } from '../../services/squid-service'
import { showFormErrors } from '../../_shared/showFormError'

@Component({
  selector: 'app-nugetfeeds',
  templateUrl: './nugetfeeds.component.html'
})
export class NugetFeedsComponent implements OnInit {
  public nugetfeeds: NugetFeed[]
  feedForm: FormGroup
  currentFeed: NugetFeed
  currentFeedId: string

  formErrors = {
    name: '',
    feedURL: '',
    apiKey: ''
  }

  validationMessages = {
    name: {
      required: 'The name is required.'
    },
    feedURL: {
      required: 'The feed URL is required.'
    },
    apiKey: {
      required: 'The API key is required.'
    }
  }

  constructor(private squidService: SquidService, private formBuilder: FormBuilder) {}

  ngOnInit() {
    this.squidService.getNuGetFeeds().subscribe(
      result => {
        this.nugetfeeds = result
        this.nugetfeeds.sort(this.sortAlphabetically)
      },
      error => console.error(error)
    )

    this.buildForm()
  }

  private buildForm(): void {
    this.feedForm = this.formBuilder.group({
      name: ['', Validators.required],
      feedURL: ['', Validators.required],
      apiKey: ['', Validators.required]
    })
  }

  saveFeed(): void {
    if (!showFormErrors(this.feedForm, this.formErrors, this.validationMessages)) {
      this.currentFeed = {
        name: this.feedForm.get('name').value,
        feedURL: this.feedForm.get('feedURL').value,
        apiKey: this.feedForm.get('apiKey').value,
        valid: false,
        errorMessage: ''
      }

      if (!this.currentFeedId) {
        this.squidService.addNuGetFeed(this.currentFeed).subscribe(repo => {
          this.nugetfeeds.push(repo)
          this.nugetfeeds.sort(this.sortAlphabetically)

          this.feedForm.reset()
        })
      } else {
        this.currentFeed.id = this.currentFeedId
        this.squidService.updateNuGetFeed(this.currentFeed).subscribe(repo => {
          this.nugetfeeds = this.nugetfeeds.filter(r => r.id !== this.currentFeedId)
          this.nugetfeeds.push(repo)
          this.nugetfeeds.sort(this.sortAlphabetically)

          this.currentFeedId = null
          this.feedForm.reset()
        })
      }
    }
  }

  updateFeedForm(repo: NugetFeed): void {
    this.currentFeedId = repo.id
    this.feedForm.get('name').setValue(repo.name)
    this.feedForm.get('feedURL').setValue(repo.feedURL)
  }

  deleteFeedForm(repo: NugetFeed): void {
    this.squidService.deleteNugetFeed(repo).subscribe(() => {
      this.nugetfeeds = this.nugetfeeds.filter(r => r.id !== repo.id)
    })
  }

  clearForm(): void {
    this.currentFeedId = null
    this.feedForm.reset()
  }

  private sortAlphabetically(n1: NugetFeed, n2: NugetFeed): number {
    if (n1.name.toLowerCase() < n2.name.toLowerCase()) {
      return -1
    }
    if (n1.name.toLowerCase() > n2.name.toLowerCase()) {
      return 1
    }
    return 0
  }
}
