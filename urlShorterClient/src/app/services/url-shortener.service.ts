import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UrlDto, UrlShortResponseDto } from '../models/url.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UrlShortenerService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  shortenUrl(urlData: UrlDto): Observable<UrlShortResponseDto> {
    return this.http.post<UrlShortResponseDto>(`${this.apiUrl}/shorturl`, urlData);
  }
}