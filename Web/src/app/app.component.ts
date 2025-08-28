import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SceneComponent } from './scene/scene.component';
import { HttpClient } from '@angular/common/http';
import { Torus } from '../models/torus';
import { TorusResponse } from '../models/torusResponse';
import { TorusFormComponent, TorusFormData } from './torus-form/torus-form.component';
import { LoadingOverlayComponent } from './loading/loading-overlay.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, SceneComponent, TorusFormComponent, LoadingOverlayComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  torusList!: Torus[];
  isPanelVisible = true;
  isLoading: boolean = false;

  @ViewChild(SceneComponent) sceneComponent!: SceneComponent;
  @ViewChild(TorusFormComponent) torusFormComponent!: TorusFormComponent;

  constructor(private http: HttpClient) { }

  togglePanel(): void {
    this.isPanelVisible = !this.isPanelVisible;
  }

  onFormSubmit(formData: TorusFormData): void {
    this.isLoading = true;

    const requestBody = {
      cubeEdge: formData.cubeEdge,
      maxTorusRadius: formData.max_R,
      minTorusRadius: formData.min_R,
      torusThicknessCoefficient: formData.thicknessCoefficient,
      targetTorusCount: formData.targetTorusCount,
      generationType: formData.generationMethod
    };

    this.http.post('https://localhost:7000/api/TorusGeneration/GetList', requestBody)
      .subscribe({
        next: (res) => {
          const response = res as TorusResponse;
          this.torusList = response.torusList;
          this.isLoading = false;

          if (this.sceneComponent) {
            this.sceneComponent.vizualizeTori(this.torusList, formData.cubeEdge);
          }
        },
        error: (error) => {
          this.isLoading = false;
          // Передаем ошибку в форму
          if (this.torusFormComponent) {
            this.torusFormComponent.setError(error || 'Произошла неизвестная ошибка');
          }
        }
      });
  }

  onPanelToggle(isVisible: boolean): void {
    // Можно добавить дополнительную логику при переключении панели
    console.log('Panel visibility:', isVisible);
  }
}
