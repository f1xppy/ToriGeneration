import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-loading-overlay',
  templateUrl: './loading-overlay.component.html',
  styleUrls: ['./loading-overlay.component.css'],
  imports: [NgIf]
})
export class LoadingOverlayComponent {
  @Input() isLoading: boolean = false;
  message: string = 'Loading...';
}
