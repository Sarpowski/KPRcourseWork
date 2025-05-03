import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UrlShortenerService } from '../../services/url-shortener.service';

@Component({
  selector: 'app-url-shortener',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './url-shortener.component.html',
  styleUrls: ['./url-shortener.component.css']
})
export class UrlShortenerComponent {
  urlForm: FormGroup;
  isLoading = false;
  shortUrl: string | null = null;
  errorMessage: string | null = null;
  copied = false;
  
  constructor(
    private fb: FormBuilder,
    private urlService: UrlShortenerService
  ) {
    this.urlForm = this.fb.group({
      url: ['', [
        Validators.required,
        Validators.pattern('https?://.+')
      ]],
      ttlMinutes: [60, [
        Validators.required,
        Validators.min(1),
        Validators.max(1440)
      ]]
    });
  }
  
  shortenUrl(): void {
    if (this.urlForm.invalid) {
      return;
    }
    
    this.isLoading = true;
    this.errorMessage = null;
    this.shortUrl = null;
    
    this.urlService.shortenUrl(this.urlForm.value)
      .subscribe({
        next: (response) => {
          this.shortUrl = response.url;
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error || 'An error occurred while shortening the URL';
          console.error('Error shortening URL', error);
        }
      });
  }
  
  copyToClipboard(): void {
    if (this.shortUrl) {
      navigator.clipboard.writeText(this.shortUrl)
        .then(() => {
          this.copied = true;
          setTimeout(() => this.copied = false, 2000);
        })
        .catch(err => {
          console.error('Could not copy text: ', err);
        });
    }
  }
}