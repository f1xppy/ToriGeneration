import { Component, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

export interface TorusFormData {
  cubeEdge: number;
  min_R: number;
  max_R: number;
  thicknessCoefficient: number;
  targetTorusCount: number;
  generationMethod: number;
}

@Component({
  selector: 'app-torus-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './torus-form.component.html',
  styleUrls: ['./torus-form.component.css']
})
export class TorusFormComponent {
  form: FormGroup;
  errorMessage: string = '';

  @Input() isPanelVisible: boolean = true;
  @Output() formSubmit = new EventEmitter<TorusFormData>();
  @Output() panelToggle = new EventEmitter<boolean>();

  constructor(private fb: FormBuilder) {
    this.form = this.createForm();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      cubeEdge: ['20', [
        Validators.required,
        Validators.pattern(/^-?\d*\.?\d+$/),
        Validators.min(0.1)
      ]],
      min_R: ['0.5', [
        Validators.required,
        Validators.pattern(/^-?\d*\.?\d+$/),
        Validators.min(0.01)
      ]],
      max_R: ['1', [
        Validators.required,
        Validators.pattern(/^-?\d*\.?\d+$/),
        Validators.min(0.01)
      ]],
      thicknessCoefficient: ['0.25', [
        Validators.required,
        Validators.pattern(/^-?\d*\.?\d+$/),
        Validators.min(0.01),
        Validators.max(0.5)
      ]],
      targetTorusCount: ['1000', [
        Validators.required,
        Validators.pattern(/^-?\d*\.?\d+$/),
        Validators.min(1)
      ]],
      generationMethod: ['1', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formData = this.form.value;
      const formDataTyped: TorusFormData = {
        cubeEdge: Number(formData.cubeEdge),
        min_R: Number(formData.min_R),
        max_R: Number(formData.max_R),
        thicknessCoefficient: Number(formData.thicknessCoefficient),
        targetTorusCount: Number(formData.targetTorusCount),
        generationMethod: Number(formData.generationMethod)
      };

      this.formSubmit.emit(formDataTyped);
    } else {
      this.form.markAllAsTouched();
    }
  }

  fillInitialValues(): void {
    this.form = this.createForm();
    this.clearError();
  }

  setError(error: string): void {
    this.errorMessage = error;
  }

  clearError(): void {
    this.errorMessage = '';
  }

  onRadioChange(event: any): void {
    const value = event.target.value;
    if (value === '2' || value === '3') {
      event.preventDefault();
      this.form.get('generationMethod')?.setValue('1');
    }
  }
}
