import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { SceneComponent } from './scene/scene.component';
import { TorusFormComponent, TorusFormData } from './torus-form/torus-form.component';
import { LoadingOverlayComponent } from './loading/loading-overlay.component';
import { Torus } from '../models/torus';
import { TorusResponse } from '../models/torusResponse';

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

  onPanelToggle(isVisible: boolean): void {
    console.log('Panel visibility:', isVisible);
  }

  onFormSubmit(formData: TorusFormData): void {
    // Генерация файлов
    if (formData.generateFilesCount !== undefined && formData.generateFilesCount > 0) {
      this.generateFiles(formData.generateFilesCount);
      return;
    }

    // Обычный GetList
    this.isLoading = true;
    const requestBody = {
      cubeEdge: formData.cubeEdge,
      maxTorusRadius: formData.max_R,
      minTorusRadius: formData.min_R,
      torusThicknessCoefficient: formData.thicknessCoefficient,
      targetTorusCount: formData.targetTorusCount,
      generationType: formData.generationMethod
    };

    this.http.post<TorusResponse>('https://localhost:7000/api/TorusGeneration/GetList', requestBody)
      .subscribe({
        next: (res) => {
          this.torusList = res.torusList;
          this.isLoading = false;

          if (this.sceneComponent) {
            this.sceneComponent.vizualizeTori(this.torusList, formData.cubeEdge);
          }

          if (this.torusFormComponent) {
            this.torusFormComponent.torusCount = res.totalCount;
            this.torusFormComponent.totalConcentration = res.concentration.toFixed(3);
            this.torusFormComponent.elapsedTime = res.elapsedTime;
            this.torusFormComponent.maxRetries = res.maxRetries;
            this.torusFormComponent.showInfo = true;
          }
        },
        error: (err) => {
          this.isLoading = false;
          if (this.torusFormComponent) this.torusFormComponent.setError(err || 'Ошибка');
        }
      });
  }

  private generateFiles(filesCount: number) {
    if (!this.torusFormComponent) return;

    this.isLoading = true;
    this.torusFormComponent.fileProgress = '';
    this.torusFormComponent.isGeneratingFiles = true;

    const form = this.torusFormComponent.form.value;
    const requestBody = {
      cubeEdge: Number(form.cubeEdge),
      maxTorusRadius: Number(form.max_R),
      minTorusRadius: Number(form.min_R),
      torusThicknessCoefficient: Number(form.thicknessCoefficient),
      targetTorusCount: Number(form.targetTorusCount),
      generationType: Number(form.generationMethod)
    };

    this.http.post<string[]>(
      `https://localhost:7000/api/TorusGeneration/GenerateFiles?filesCount=${filesCount}`,
      requestBody
    ).subscribe({
      next: (cacheKeys: string[]) => {
        this.isLoading = false;
        if (!cacheKeys || cacheKeys.length === 0) return;
        this.trackFileGeneration(cacheKeys, Number(form.targetTorusCount));
      },
      error: (err) => {
        this.isLoading = false;
        if (this.torusFormComponent) this.torusFormComponent.setError(err || 'Ошибка генерации файлов');
      }
    });
  }

  private trackFileGeneration(keys: string[], torusCount: number) {
    const totalFiles = keys.length;
    const generatedCount: number[] = Array(totalFiles).fill(0);

    const intervalId = setInterval(() => {
      const requests = keys.map(key => this.http.get<number>(`https://localhost:7000/api/TorusGeneration/GetStatus?cacheKey=${key}`));

      Promise.all(requests.map(r => r.toPromise()))
        .then(statuses => {
          let completedCount = 0;

          statuses.forEach((status, i) => {
            if (status === -1) completedCount++;
            else generatedCount[i] = status ?? 0;
          });

          const sumGenerated = generatedCount.reduce((a, b) => a + b, 0);

          if (this.torusFormComponent) {
            this.torusFormComponent.fileProgress = `${sumGenerated}/${totalFiles * torusCount}`;
          }

          if (completedCount === totalFiles) {
            if (this.torusFormComponent) {
              this.torusFormComponent.fileProgress = `Done! (${sumGenerated}/${totalFiles * torusCount})`;
              this.torusFormComponent.isGeneratingFiles = false;
            }
            clearInterval(intervalId);
          }
        })
        .catch(err => console.error(err));
    }, 1000);
  }
}
