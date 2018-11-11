import { FormGroup } from '@angular/forms'

/**
 * Validates the form, if there is some errors, set the error messages per field
 * @param {FormGroup} form - The form to be validated
 * @param {Object} formErrors - The object with the actual error messages
 * @param {Object} validationMessages - The validation error messages available
 * @returns {boolean} - true if there is some error, false otherwise.
 */
export function showFormErrors(form: FormGroup, formErrors: Object, validationMessages: Object): boolean {
  let hasError = false

  for (const field of Object.keys(formErrors)) {
    // clear previous error message (if any)
    formErrors[field] = ''
    const control = form.get(field)

    if (control && !control.valid && control.errors) {
      hasError = true
      const messages = validationMessages[field]

      // Shows only the first error
      formErrors[field] += messages[Object.keys(control.errors)[0]]
    }
  }

  return hasError
}
