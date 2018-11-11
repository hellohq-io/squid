import { Component, Inject, OnInit } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { RepositoryConfig } from '../../services/models'
import { SquidService } from '../../services/squid-service'
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { showFormErrors } from '../../_shared/showFormError'

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit {
  repoForm: FormGroup

  formErrors = {
    name: ''
  }

  validationMessages = {
    name: {
      required: 'The user name is required.'
    }
  }

  constructor(private squidService: SquidService, private formBuilder: FormBuilder) {}

  ngOnInit() {
    this.buildForm()

    const userName = this.getCookie('squid-bb-user')
    if (userName) {
      const decodedUserName = atob(userName)
      this.repoForm.get('name').setValue(decodedUserName)
    }
  }

  private buildForm(): void {
    this.repoForm = this.formBuilder.group({
      name: ['', Validators.required]
    })
  }

  saveRepo(): void {
    const userName = this.repoForm.get('name').value
    if (userName) {
      const encodedUserName = btoa(userName)
      document.cookie = 'squid-bb-user=' + encodedUserName
    } else {
      document.cookie = 'squid-bb-user='
    }
  }

  clearForm(): void {
    this.repoForm.reset()
  }

  getCookie(cname): string {
    const name = cname + '='
    const decodedCookie = decodeURIComponent(document.cookie)
    const ca = decodedCookie.split(';')
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i]
      while (c.charAt(0) === ' ') {
        c = c.substring(1)
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length)
      }
    }
    return ''
  }
}
