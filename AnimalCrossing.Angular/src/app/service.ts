import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Game } from './models';

@Injectable({
    providedIn: 'root'
})
export class GameService {
    constructor(private httpClient: HttpClient) {
    }

    getGame(id?: string): Observable<Game> {
        let requestUrl = '/api/Game';

        if (id)
            requestUrl += `/${id}`;

        return this.httpClient.get<Game>(requestUrl);
    }
}